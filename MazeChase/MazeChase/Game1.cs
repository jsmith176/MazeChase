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

namespace MazeChase
{
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

        bool pause = false;

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
            player = new Player(this.Content, inputManager, mapManager, origin);

            // Initialize Ghost
            red = new Ghost(mapManager, player, ghostTexture, new Vector2((28 * 16) + 8, (19 * 16) + 8));
            blue = new Ghost(mapManager, player, ghostTexture, new Vector2((37 * 16) + 8, (22 * 16) + 8));
            pink = new Ghost(mapManager, player, ghostTexture, new Vector2((28 * 16) + 8, (25 * 16) + 8));
            orange = new Ghost(mapManager, player, ghostTexture, new Vector2((37 * 16) + 8, (25 * 16) + 8));
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize ScoreManager
            scoreManager = new ScoreManager(Content.Load<SpriteFont>("SpriteFont"));

            // Viewport Origin
            origin = new Vector2(400, 240);

            // Load Textures
            ghostTexture = Content.Load<Texture2D>(@"PacMan");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                pause = true;

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                pause = false;

            if (!pause && !player.isDead)
            {
                // Check for input
                inputManager.Update(gameTime);

                // Update map
                mapManager.Update(gameTime);
                mapManager.isFoodUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y);

                // Update player
                player.Update(gameTime);
                // Update ghosts
                red.Update(gameTime);
                //blue.Update(gameTime);
                pink.Update(gameTime);
                orange.Update(gameTime);
                //player.isDead = false;
            }
            else if (player.isDead)
                player.deathAnimation(gameTime);

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
