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
        Vector2 origin;

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

            if (inputManager.getLastKeyPressed() == Keys.Up && !mapManager.isWall(player.getPosition().X, player.getPosition().Y - 10))
            {
                if (mapManager.getViewport().Y > 0 && player.getPosition().Y == origin.Y)
                {
                    mapManager.moveViewport(0, -speed);
                    player.Move(0, -speed, false);
                }

                else if (player.getPosition().Y > 0)
                {
                    player.Move(0, -speed, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Right && !mapManager.isWall(player.getPosition().X + 10, player.getPosition().Y))
            {
                if (mapManager.getViewport().X < mapManager.getMap().DisplayWidth - mapManager.getViewport().Width && player.getPosition().X == origin.X)
                {
                    mapManager.moveViewport(speed, 0);
                    player.Move(speed, 0, false);
                }

                else if (player.getPosition().X < mapManager.getViewport().Width)
                {
                    player.Move(speed, 0, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Down && !mapManager.isWall(player.getPosition().X, player.getPosition().Y + 10))
            {
                if (mapManager.getViewport().Y < mapManager.getMap().DisplayHeight - mapManager.getViewport().Height && player.getPosition().Y == origin.Y)
                {
                    mapManager.moveViewport(0, speed);
                    player.Move(0, speed, false);
                }
                else if (player.getPosition().Y < mapManager.getViewport().Height)
                {
                    player.Move(0, speed, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Left && !mapManager.isWall(player.getPosition().X - 10, player.getPosition().Y))
            {
                if (mapManager.getViewport().X > 0 && player.getPosition().X == origin.X)
                {
                    mapManager.moveViewport(-speed, 0);
                    player.Move(-speed, 0, false);
                }
                else if (player.getPosition().X > 0)
                {
                    player.Move(-speed, 0, true);
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
