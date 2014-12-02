using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#region REQUIREMENTS

/*
General:
•	The maze must be complex and scrollable (e.g., bigger than the display)
•	It must be challenging for the player to evade the chase bots, but not impossible.
•	Score will be kept.  The player’s score will nominally increase the longer it can evade the chase bots.  
•	The game will have appropriate sound.

Main Character:
•	The main character must be controlled by a joystick, keyboard, or mouse (strongly prefer joystick).  The ability of the user to easily control the character’s movement will be very important.
•	As in PacMan, the main character should have some associated animation that (at a minimum) should change with its direction (up, down, left, right).

Playfield:
•	The playfield must consist of a large, scrollable maze.  
•	Use TIDE to create the maze and it’s the xTile engine to render it at runtime.  
•	The main character’s position should determine what part of the maze is displayed at any given time.  Whenever possible, the display should be centered on the main character’s position.  
•	You will need an internal representation of the maze (recommend a graph)

Chase Bots:
•	The chase bots (obviously) chase the player.  However, they need to do so in a manner that both intelligent and flawed.  
•	Intelligent refers to the method (e.g., sensor) for estimating the player’s position and/or direction.  Flawed implies that the method should not be perfect—the player should be able to employ some strategy or countermeasure to evade a given bot.
•	There need to be at least four (4) different flavors of chase bot.  Each type of chase bot must employ a different chase strategy.  Chase bots may also cooperate with each other (share information) to a limited degree.
•	There should be some mechanism for the player to either kill or disable the chase bots.  For example, the player may eat a “power pill” which gives him this power for a period of time. 
•	Chase bots should have no restrictions on where they can travel in the maze.
•	The chase bots must have some associated animation.

Algorithms:
•	Each type of chase bot must use a different algorithm (based on its unique capabilities) to estimate the position and direction of the player.
•	Because you are dealing with a maze, the chase bots must employ some navigation planning algorithm to find its way to the player’s estimated position.  This algorithm must be reasonably efficient.

Extra Credit:
•	An introduction and/or high score display for the game.
•	Implementation of levels
•	Implementation of animation into the Maze structure.  For example, passages that randomly open and close.
•	Animated reconstitution of chase bots from a centralized location
•	Use of TIDE special effects:  parallax and/or tile animation.
•	Other non-trivial ideas (please consult with me)


Tips:
•	Recommend that you implement the maze first and then construct the internal (graph) representation.  This should be done before introducing any of the characters.
•	Use a build a little, test a little development approach.  Do not bet the farm on a “grand design” that you are unable to implement.

*/

#endregion

namespace MazeChase
{
    public enum direction { STILL, UP, DOWN, LEFT, RIGHT };

    enum mode { SCATTER, ATTACK, FLEE, REGENERATE };

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager inputManager;
        MapManager mapManager;
        ScoreManager scoreManager;
        Player player;
        Ghost red, blue, pink, orange;
        Vector2 origin;

        SoundEffect deathSound, newLifeSound, eatSound, beginningMusic;
        SoundEffectInstance deathSoundInstance, newLifeInstance, eatInstance, musicInstance;

        bool pause = true;

        // Textures
        Texture2D ghostTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize mapManager
            mapManager = new MapManager(this.Content, this.GraphicsDevice, this.scoreManager);

            // Load map content
            mapManager.LoadContent();
            mapManager.Initialize();

            // Initialize InputManager
            inputManager = new InputManager();

            // Initialize Player
            player = new Player(this.Content, inputManager, mapManager, scoreManager, origin);          

            // Initialize Ghost
            red = new Ghost(this.Content, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((61 * 16) + 8, (1 * 16) + 8));// top right
            blue = new Ghost(this.Content, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((61 * 16) + 8, (43 * 16) + 8));// bottom right
            pink = new Ghost(this.Content, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((1 * 16) + 8, (1 * 16) + 8));// top left
            orange = new Ghost(this.Content, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((1 * 16) + 8, (43 * 16) + 8));// botttom left
        }

        protected override void LoadContent()
        {
            deathSound = Content.Load<SoundEffect>(@"pacman_death");
            deathSoundInstance = deathSound.CreateInstance();

            newLifeSound = Content.Load<SoundEffect>(@"pacman_extrapac");
            newLifeInstance = newLifeSound.CreateInstance();  

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize ScoreManager
            scoreManager = new ScoreManager(Content.Load<SpriteFont>("SpriteFont"), newLifeInstance);

            // Viewport Origin
            origin = new Vector2(400, 336);

            // Load Textures
            ghostTexture = Content.Load<Texture2D>(@"Ghosts2");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            scoreManager.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                pause = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                pause = false;
            }

            if (!pause && !player.isDead)
            {
                // Check for input
                inputManager.Update(gameTime);

                // Update map
                mapManager.Update(gameTime);
                if (mapManager.isFoodUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) == 20)
                {
                    player.modeSwap(500);
                }

                // Update player
                player.Update(gameTime);

                // Update ghosts
                red.Update(gameTime);
                blue.Update(gameTime);
                pink.Update(gameTime);
                orange.Update(gameTime);
            }
            else if (player.isDead)
            {
                player.deathAnimation(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw map
            mapManager.Draw();

            // Draw sprites
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            player.Draw(spriteBatch);
            if (!player.isDead)
            {
                red.Draw(spriteBatch);
                blue.Draw(spriteBatch);
                pink.Draw(spriteBatch);
                orange.Draw(spriteBatch);
            }

            scoreManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
