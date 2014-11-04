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
        int[,] intersections;

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

            // Initialise xTile rendering viewport at the players location
            // where 40 is the 40th 'x' tile and 22 is the 22nd 'y' tile
            viewport = new xTile.Dimensions.Rectangle(new Size(800, 480));
            viewport.X = (40 * 16 + 8) - viewport.Width / 2;
            viewport.Y = (22 * 16 + 8) - viewport.Height / 2;

            // Initialise Map Layer
            layer = map.GetLayer("maze layer");
            defineIntersections();
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

        public bool isIntersectionUnderPlayer(int playerX, int playerY)
        {
            if (intersections[(viewport.X + playerX) / 16, (viewport.Y + playerY) / 16] == 1)
                return true;
            return false;
        }

        void defineIntersections()
        {
            intersections = new int[layer.LayerWidth, layer.LayerHeight];
            Tile currentTile, above, right, below, left;
            for (int i = 0; i < layer.LayerWidth; i++)
            {
                for (int j = 0; j < layer.LayerHeight; j++)
                {
                    currentTile = layer.Tiles[i, j];

                    if (j > 0)
                        above = layer.Tiles[i, j - 1];
                    else
                        above = null;
                    if (i < layer.LayerWidth - 1)
                        right = layer.Tiles[i + 1, j];
                    else
                        right = null;
                    if (j < layer.LayerHeight - 1)
                        below = layer.Tiles[i, j + 1];
                    else
                        below = null;
                    if (i > 0)
                        left = layer.Tiles[i - 1, j];
                    else
                        left = null;

                    if (currentTile.TileIndexProperties.Count > 0)
                    {
                        if (currentTile.TileIndexProperties.ElementAt(0).Key.Equals("Wall"))
                        {
                            intersections[i, j] = 0;
                        }
                    }
                    else
                    {
                        if (above != null && right != null && below != null && left != null)
                        {
                            if (above.TileIndexProperties.Count == 0 || below.TileIndexProperties.Count == 0)
                            {
                                if (left.TileIndexProperties.Count == 0 || right.TileIndexProperties.Count == 0)
                                    intersections[i, j] = 1;
                                else
                                    intersections[i, j] = 2;
                            }
                            else if (left.TileIndexProperties.Count == 0 || right.TileIndexProperties.Count == 0)
                            {
                                if (above.TileIndexProperties.Count == 0 || below.TileIndexProperties.Count == 0)
                                    intersections[i, j] = 1;
                                else
                                    intersections[i, j] = 2;
                            }
                        }
                        else
                            intersections[i, j] = 2;
                    }
                }
            }
        }
    }
}
