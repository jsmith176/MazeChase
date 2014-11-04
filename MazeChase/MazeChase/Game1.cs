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
        Player player;
        Vector2 origin, direction;
        bool playerMove = false;

        // Player speed
        int speed = 2;

        // Textures
        Texture2D playerTexture, ghostTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize mapManager
            mapManager = new MapManager(this.Content, this.GraphicsDevice);

            // Load map content
            mapManager.LoadContent();
            mapManager.Initialize();

            // Initialize InputManager
            inputManager = new InputManager();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            playerTexture = Content.Load<Texture2D>(@"PacMan");

            // Initialise Player
            origin = new Vector2(400, 240);

            player = new Player(playerTexture, origin);

            direction = Vector2.Zero;

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

            // Check for input
            player.Move(speed * (int)direction.X, speed * (int)direction.Y, true);

            if (inputManager.getLastKeyPressed() == Keys.Up && mapManager.isIntersection((int)player.getPosition().X, (int)player.getPosition().Y))
            {
                if (mapManager.getViewport().Y > 0 && player.getPosition().Y == origin.Y)
                {
                    mapManager.moveViewport(0, -speed);
                    direction.X = 0;
                    direction.Y = -1;
                    playerMove = false;
                }

                else if (player.getPosition().Y > 0)
                {
                    direction.X = 0;
                    direction.Y = -1;
                    playerMove = true;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Right && mapManager.isIntersection((int)player.getPosition().X, (int)player.getPosition().Y))
            {
                if (mapManager.getViewport().X < mapManager.getMap().DisplayWidth - mapManager.getViewport().Width && player.getPosition().X == origin.X)
                {
                    mapManager.moveViewport(speed, 0);
                    direction.X = 1;
                    direction.Y = 0;
                    playerMove = false;
                }

                else if (player.getPosition().X < mapManager.getViewport().Width)
                {
                    direction.X = 1;
                    direction.Y = 0;
                    playerMove = true;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Down && mapManager.isIntersection((int)player.getPosition().X, (int)player.getPosition().Y))
            {
                if (mapManager.getViewport().Y < mapManager.getMap().DisplayHeight - mapManager.getViewport().Height && player.getPosition().Y == origin.Y)
                {
                    mapManager.moveViewport(0, speed);
                    direction.X = 0;
                    direction.Y = 1;
                    playerMove = false;
                }
                else if (player.getPosition().Y < mapManager.getViewport().Height)
                {
                    direction.X = 0;
                    direction.Y = 1;
                    playerMove = true;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Left && mapManager.isIntersection((int)player.getPosition().X, (int)player.getPosition().Y))
            {
                if (mapManager.getViewport().X > 0 && player.getPosition().X == origin.X)
                {
                    mapManager.moveViewport(-speed, 0);
                    direction.X = -1;
                    direction.Y = 0;
                    playerMove = false;
                }
                else if (player.getPosition().X > 0)
                {
                    direction.X = -1;
                    direction.Y = 0;
                    playerMove = true;
                }
            }

            // Update player
            player.Update(gameTime);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
