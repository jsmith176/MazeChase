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
        Vector2 origin, movement, tileUnderPlayer, tileAbovePlayer, tileBelowPlayer, tileLeftOfPlayer, tileRightOfPlayer;
        enum direction { STILL, UP, DOWN, LEFT, RIGHT};
        direction movementDirection;

        SoundEffect playerNoise;
        SoundEffectInstance playerNoiseInstance;

        // Player speed
        int speed = 2;

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
            playerNoise = Content.Load<SoundEffect>(@"PlayerNoise");

            playerNoiseInstance = playerNoise.CreateInstance();

            // Initialise Player
            origin = new Vector2(400, 240);

            player = new Player(playerTexture, origin);

            movement = Vector2.Zero;

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

            // Update tiles around player
            tileUnderPlayer = new Vector2(player.getPosition().X, player.getPosition().Y);
            tileAbovePlayer = new Vector2(player.getPosition().X, player.getPosition().Y - 9);
            tileBelowPlayer = new Vector2(player.getPosition().X, player.getPosition().Y + 9);
            tileLeftOfPlayer = new Vector2(player.getPosition().X - 9, player.getPosition().Y);
            tileRightOfPlayer = new Vector2(player.getPosition().X + 9, player.getPosition().Y);

            // Check for input

            if (movementDirection == direction.DOWN)
            {
                if (inputManager.getLastKeyPressed() == Keys.Up)
                {
                    movementDirection = direction.UP;
                    movement.X = 0;
                    movement.Y = -1;
                }
            }
            else if (movementDirection == direction.UP)
            {
                if (inputManager.getLastKeyPressed() == Keys.Down)
                {
                    movementDirection = direction.DOWN;
                    movement.X = 0;
                    movement.Y = 1;
                }
            }
            else if (movementDirection == direction.LEFT)
            {
                if (inputManager.getLastKeyPressed() == Keys.Right)
                {
                    movementDirection = direction.RIGHT;
                    movement.X = 1;
                    movement.Y = 0;
                }
            }
            else if (movementDirection == direction.RIGHT)
            {
                if (inputManager.getLastKeyPressed() == Keys.Left)
                {
                    movementDirection = direction.LEFT;
                    movement.X = -1;
                    movement.Y = 0;
                }
            }
            
            if (inputManager.getLastKeyPressed() == Keys.Up && mapManager.isIntersectionUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) && !mapManager.isWall(tileAbovePlayer.X, tileAbovePlayer.Y))
            {
                if ((player.getPosition().X + mapManager.getViewport().X) % 24 == 0)// where 24 is the tile width (or height) +  an offset of 1/2 tile width (or height)
                {
                    movementDirection = direction.UP;
                    movement.X = 0;
                    movement.Y = -1;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Right && mapManager.isIntersectionUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) && !mapManager.isWall(tileRightOfPlayer.X, tileRightOfPlayer.Y))
            {
                if ((player.getPosition().Y + mapManager.getViewport().Y) % 24 == 0)
                {
                    movementDirection = direction.RIGHT;
                    movement.X = 1;
                    movement.Y = 0;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Down && mapManager.isIntersectionUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) && !mapManager.isWall(tileBelowPlayer.X, tileBelowPlayer.Y))
            {
                if ((player.getPosition().X + mapManager.getViewport().X) % 24 == 0)
                {
                    movementDirection = direction.DOWN;
                    movement.X = 0;
                    movement.Y = 1;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Left && mapManager.isIntersectionUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) && !mapManager.isWall(tileLeftOfPlayer.X, tileLeftOfPlayer.Y))
            {
                if ((player.getPosition().Y + mapManager.getViewport().Y) % 24 == 0)
                {
                    movementDirection = direction.LEFT;
                    movement.X = -1;
                    movement.Y = 0;
                }
            }

            // Check for walls
            switch (movementDirection)
            {
                case direction.UP:
                    if (!mapManager.isWall(tileAbovePlayer.X, tileAbovePlayer.Y))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().Y > 0 && player.getPosition().Y == origin.Y)
                        {
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, false, true);
                            mapManager.moveViewport(0, -speed);
                        }
                        else
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                    }
                    break;
                case direction.DOWN:
                    if (!mapManager.isWall(tileBelowPlayer.X, tileBelowPlayer.Y))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().Y < mapManager.getMap().DisplayHeight - mapManager.getViewport().Height && player.getPosition().Y == origin.Y)
                        {
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, false, true);
                            mapManager.moveViewport(0, speed);
                        }
                        else
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                    }
                    break;
                case direction.LEFT:
                    if (!mapManager.isWall(tileLeftOfPlayer.X, tileLeftOfPlayer.Y))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().X > 0 && player.getPosition().X == origin.X)
                        {
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, false, true);
                            mapManager.moveViewport(-speed, 0);
                        }
                        else
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                    }
                    break;
                case direction.RIGHT:
                    if (!mapManager.isWall(tileRightOfPlayer.X, tileRightOfPlayer.Y))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().X < mapManager.getMap().DisplayWidth - mapManager.getViewport().Width && player.getPosition().X == origin.X)
                        {
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, false, true);
                            mapManager.moveViewport(speed, 0);
                        }
                        else
                            player.Move(speed * (int)movement.X, speed * (int)movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                    }
                    break;
                default:
                    player.Move(0, 0, false, false);
                    break;
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
