using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
// xTile engine namespaces
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;

namespace MazeChase
{
    class MapManager
    {
        ContentManager contentManager;
        GraphicsDevice graphicsDevice;
        InputManager inputManager;
        Layer layer;
        //Tile tileUnderPlayer, tileAbovePlayer, tileRightOfPlayer, tileBelowPlayer, tileLeftOfPlayer;
        //int[,] intersections;

        // xTile map, display device reference, and rendering viewport
        Map map;
        IDisplayDevice mapDisplayDevice;
        xTile.Dimensions.Rectangle viewport;
        
        public MapManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
            this.inputManager = inputManager;
        }

        public virtual void Initialize()
        {
            // Initialise xTile map display device
            mapDisplayDevice = new XnaDisplayDevice(contentManager, graphicsDevice);

            // Initialise xTile map resources
            map.LoadTileSheets(mapDisplayDevice);

            // Initialise xTile rendering viewport at the center of the map
            viewport = new xTile.Dimensions.Rectangle(new Size(800, 480));
            viewport.X = map.DisplaySize.Width / 2 - viewport.Width / 2;
            viewport.Y = map.DisplaySize.Height / 2 - viewport.Height / 2;

            // Initialise Map Layer
            layer = map.GetLayer("maze layer");
            //defineIntersections();
        }

        public virtual void LoadContent()
        {
            // Load xTile map from content pipeline
            map = contentManager.Load<Map>("Maze");
        }

        public virtual void Update(GameTime gameTime)
        {
            // Update xTile map for animations etc.
            // and update viewport for camera movement
            map.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public virtual void Draw()
        {
            // Render xTile map
            map.Draw(mapDisplayDevice, viewport);
        }

        public Map getMap()
        {
            return map;
        }

        public xTile.Dimensions.Rectangle getViewport()
        {
            return viewport;
        }

        public void moveViewport(int x, int y)
        {
            viewport.X += x;
            viewport.Y += y;
        }

        public bool isWall(float x, float y)
        {
            Tile tile = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + x), (int)(viewport.Y + y)))];

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

        //void defineIntersections()
        //{
        //    intersections = new int[layer.LayerWidth, layer.LayerHeight];
        //    Tile currentTile, above, right, below, left;
        //    for (int i = 0; i < layer.LayerWidth; i++)
        //    {
        //        for (int j = 0; j < layer.LayerHeight; j++)
        //        {
        //            currentTile = layer.Tiles[new Location(i * 16, j * 16)];

        //            if (i > 0)
        //                above = layer.Tiles[new Location(i * 16, (j * 16) - 16)];
        //            else
        //                above = null;
        //            if (j < layer.DisplayWidth - 16)
        //                right = layer.Tiles[new Location((i * 16) + 16, j * 16)];
        //            else
        //                right = null;
        //            if (i < layer.DisplayHeight)
        //                below = layer.Tiles[new Location(i * 16, (j * 16) + 16)];
        //            else
        //                below = null;
        //            if (j > 0)
        //                left = layer.Tiles[new Location((i * 16) - 16, j * 16)];
        //            else
        //                left = null;

        //            if (above != null && below != null)
        //            {
        //                if (left != null || right != null)
        //                {
        //                    intersections[i, j] = 1;
        //                }
        //            }
        //            else if (left != null && right != null)
        //            {
        //                if (above != null || below != null)
        //                {
        //                    intersections[i, j] = 1;
        //                }
        //            }
        //            else
        //                intersections[i, j] = 0;
        //        }
        //    }
        //}
    }
}
