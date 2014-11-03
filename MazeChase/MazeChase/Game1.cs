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

// xTile engine namespaces
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;

namespace MazeChase
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager inputManager;
        Player player;
        Vector2 origin;
        Layer layer;
        Tile tileUnderPlayer, tileAbovePlayer, tileRightOfPlayer, tileBelowPlayer, tileLeftOfPlayer;

        // Commit Test Comment

        // Player speed
        int speed = 2;

        // xTile map, display device reference, and rendering viewport
        Map map;
        IDisplayDevice mapDisplayDevice;
        xTile.Dimensions.Rectangle viewport;

        // Textures
        Texture2D playerTexture, ghostTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            // Initialise xTile map display device
            mapDisplayDevice = new XnaDisplayDevice(this.Content, this.GraphicsDevice);

            // Initialise xTile map resources
            map.LoadTileSheets(mapDisplayDevice);

            // Initialise xTile rendering viewport at the center of the map
            viewport = new xTile.Dimensions.Rectangle(new Size(800, 480));
            viewport.X = map.DisplaySize.Width / 2 - viewport.Width / 2;
            viewport.Y = map.DisplaySize.Height / 2 - viewport.Height / 2;

            // Initialise InputManager
            inputManager = new InputManager();

            // Initialise Map Layer
            layer = map.GetLayer("maze layer");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load xTile map from content pipeline
            map = Content.Load<Map>("Maze");

            // Load textures
            playerTexture = Content.Load<Texture2D>(@"PacMan");

            // Initialise Player
            origin = new Vector2(400, 240);

            player = new Player(playerTexture, origin);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            // TODO: Add your update logic here

            // Update xTile map for animations etc.
            // and update viewport for camera movement
            map.Update(gameTime.ElapsedGameTime.Milliseconds);

            // Determine tiles around player
            tileUnderPlayer = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + player.getPosition().X), (int)(viewport.Y + player.getPosition().Y)))];
            tileAbovePlayer = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + player.getPosition().X), (int)(viewport.Y + player.getPosition().Y - 12)))];
            tileRightOfPlayer = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + player.getPosition().X + 12), (int)(viewport.Y + player.getPosition().Y)))];
            tileBelowPlayer = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + player.getPosition().X), (int)(viewport.Y + player.getPosition().Y + 12)))];
            tileLeftOfPlayer = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + player.getPosition().X - 12), (int)(viewport.Y + player.getPosition().Y)))];

            // Check for input
            inputManager.Update(gameTime);

            if (inputManager.getLastKeyPressed() == Keys.Up && !isWall(tileAbovePlayer))
            {
                if (viewport.Y > 0 && player.getPosition().Y == origin.Y) {
                    viewport.Y -= speed;
                    player.Move(0, -speed, false);
                }

                else if (player.getPosition().Y > 0)
                {
                    player.Move(0, -speed, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Right && !isWall(tileRightOfPlayer))
            {
                if (viewport.X < map.DisplayWidth - viewport.Width && player.getPosition().X == origin.X)
                {
                    viewport.X += speed;
                    player.Move(speed, 0, false);
                }
                    
                else if (player.getPosition().X < viewport.Width)
                {
                    player.Move(speed, 0, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Down && !isWall(tileBelowPlayer))
            {
                if (viewport.Y < map.DisplayHeight - viewport.Height && player.getPosition().Y == origin.Y)
                {
                    viewport.Y += speed;
                    player.Move(0, speed, false);
                }
                else if (player.getPosition().Y < viewport.Height)
                {
                    player.Move(0, speed, true);
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Left && !isWall(tileLeftOfPlayer))
            {
                if (viewport.X > 0 && player.getPosition().X == origin.X)
                {
                    viewport.X -= speed;
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Render xTile map
            map.Draw(mapDisplayDevice, viewport);

            // Draw sprites
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        bool isWall(Tile tile)
        {
            for (int i = 0; i < tile.TileIndexProperties.Count; ++i)
            {
                if (tile.TileIndexProperties.ElementAt(i).Key.Equals("Wall"))
                {
                    if (tile.TileIndexProperties.ElementAt(i).Value == 1)
                        return true;
                }
            }
            return false;
        }
    }
}
