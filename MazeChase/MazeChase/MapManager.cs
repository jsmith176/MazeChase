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
        ScoreManager scoreManager;
        Layer layer;
        int[,] intersections;

        // xTile map, display device reference, and rendering viewport
        Map map;
        IDisplayDevice mapDisplayDevice;
        xTile.Dimensions.Rectangle viewport;
        
        public MapManager(ContentManager contentManager, GraphicsDevice graphicsDevice, ScoreManager scoreManager)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
            this.scoreManager = scoreManager;

        }

        public virtual void Initialize()
        {
            // Initialize xTile map display device
            mapDisplayDevice = new XnaDisplayDevice(contentManager, graphicsDevice);

            // Initialize xTile map resources
            map.LoadTileSheets(mapDisplayDevice);

            // Initialize xTile rendering viewport at the players location
            // where 40 is the 40th 'x' tile and 22 is the 22nd 'y' tile
            viewport = new xTile.Dimensions.Rectangle(new Size(800, 480));
            viewport.X = (37 * 16 + 8) - viewport.Width / 2;
            viewport.Y = (22 * 16 + 8) - viewport.Height / 2;

            // Initialize Map Layer
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

        public void moveViewport(float x, float y)
        {
            viewport.X += (int)x;
            viewport.Y += (int)y;
        }

        public bool isWall(Vector2 location)
        {
            Tile tile = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + location.X), (int)(viewport.Y + location.Y)))];

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

        public bool isIntersectionUnderLocation(int x, int y)
        {
            if (intersections[(viewport.X + x) / 16, (viewport.Y + y) / 16] == 1)
                return true;
            return false;
        }

        public void isFoodUnderPlayer(int playerX, int playerY)
        {
            Tile tile = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + playerX), (int)(viewport.Y + playerY)))];

            if (tile.TileIndex == 22)
            {
                tile.TileIndex = 13;
                scoreManager.increaseScore(10);
            }
            else if (tile.TileIndex == 19)
            {
                tile.TileIndex = 13;
                scoreManager.increaseScore(100);
            }
        }

        public Vector2 tileUnderLocation(float x, float y)
        {
            return new Vector2(x, y);
        }

        public Vector2 tileAboveLocation(float x, float y)
        {
            return new Vector2(x, y - 9);
        }

        public Vector2 tileRightOfLocation(float x, float y)
        {
            return new Vector2(x + 9, y);
        }

        public Vector2 tileBelowLocation(float x, float y)
        {
            return new Vector2(x, y + 9);
        }

        public Vector2 tileLeftOfLocation(float x, float y)
        {
            return new Vector2(x - 9, y);
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
