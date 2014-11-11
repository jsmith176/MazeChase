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
        Ghost blue;
        Vector2 origin;
        
        bool inIntersection = false;

        // Textures
        Texture2D playerTexture;

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
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize ScoreManager
            scoreManager = new ScoreManager(Content.Load<SpriteFont>("SpriteFont"));

            // Viewport Origin
            origin = new Vector2(400, 240);

            // Initialize Ghosts
            playerTexture = Content.Load<Texture2D>(@"PacMan");
            blue = new Ghost(playerTexture, new Vector2(1, 1));

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Check for input
            inputManager.Update(gameTime);

            // Update map
            mapManager.Update(gameTime);
            mapManager.isFoodUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y);

            // Update player
            player.Update(gameTime);

            // Update ghosts
            //blue.Update(gameTime);
            blue.reposition(mapManager.getViewport().X, mapManager.getViewport().Y);

            if (mapManager.isIntersectionUnderLocation((int)blue.getPosition().X, (int)blue.getPosition().Y))
            {
                if (!inIntersection)
                {
                    blue.newDirection();
                    inIntersection = true;
                }
            }
            else
                inIntersection = false;

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
            blue.Draw(spriteBatch);
            scoreManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
