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

        int[,] adjMatrix;
        int[,] next;
        List<Vector2> intList = new List<Vector2>();

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

            // Initialize intersections, adjacencies, distances, and Floyd's algorithm stuff
            defineIntersections();
            defineAdjMatrix();
            floyd();
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

        public void moveViewport(float x, float y)
        {
            viewport.X += (int)x;
            viewport.Y += (int)y;
        }

        public void setViewportPosition(int x, int y)
        {
            viewport.X = x;
            viewport.Y = y;
        }

        public bool isWall(Vector2 location)
        {
            Tile tile = layer.Tiles[layer.GetTileLocation(new Location((int)(viewport.X + location.X), (int)(viewport.Y + location.Y)))];

            if (tile.TileIndexProperties.ContainsKey("Wall"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isIntersectionUnderLocation(Vector2 location)
        {
            if (layer.Tiles[(int)(viewport.X + location.X) / 16, (int)(viewport.Y + location.Y) / 16].Properties.ContainsKey("Intersection"))
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
            else if (tile.TileIndex == 16 || tile.TileIndex == 19)
            {
                AnimatedTile aTile = (AnimatedTile)tile;
                aTile.TileFrames.ElementAt(0).TileIndex = 13;
                aTile.TileFrames.ElementAt(1).TileIndex = 13;
                scoreManager.increaseScore(100);
            }
        }

        public Vector2 tileUnderLocation(Vector2 location)
        {
            return new Vector2(location.X, location.Y);
        }

        public Vector2 getTileMapUnderLocation(Vector2 location)
        {
            int x = layer.GetTileLocation(new Location((int)(viewport.X + location.X), (int)(viewport.Y + location.Y))).X;
            int y = layer.GetTileLocation(new Location((int)(viewport.X + location.X), (int)(viewport.Y + location.Y))).Y;

            return new Vector2(x, y);
        }

        public Vector2 tileAboveLocation(Vector2 location)
        {
            return new Vector2(location.X, location.Y - 9);
        }

        public Vector2 tileRightOfLocation(Vector2 location)
        {
            return new Vector2(location.X + 9, location.Y);
        }

        public Vector2 tileBelowLocation(Vector2 location)
        {
            return new Vector2(location.X, location.Y + 9);
        }

        public Vector2 tileLeftOfLocation(Vector2 location)
        {
            return new Vector2(location.X - 9, location.Y);
        }

        public int calculate2TileDistance(Vector2 a, Vector2 b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                return 0;
            }
            else if (a.X == b.X)
            {
                for (int i = (int)a.Y; i < b.Y; i++)
                {
                    if (layer.Tiles[(int)a.X, i].TileIndexProperties.ContainsKey("Wall"))
                    {
                        return 9999;
                    }
                    else
                    {

                    }
                }

                return (int)Math.Abs(a.Y - b.Y);
            }
            else if (a.Y == b.Y)
            {
                for (int i = (int)a.X; i < b.X; i++)
                {
                    if (layer.Tiles[i,(int)a.Y].TileIndexProperties.ContainsKey("Wall"))
                    {
                        return 9999;
                    }
                    else
                    {

                    }
                }

                return (int)Math.Abs(a.X - b.X);
            }
            else
            {
                return 9999;
            }
        }

        public direction getFloydDirection(Vector2 fromVector, Vector2 toVector, direction playerDir)
        {
            int from = intList.IndexOf(fromVector);
            int to = intList.IndexOf(toVector);

            //Console.WriteLine(fromVector.X + " " + fromVector.Y);
            //Console.WriteLine(toVector.X + " " + toVector.Y);
            //Console.WriteLine(from + " " + to);

            if (from == to)
            {
                return playerDir;
            }
            else
            {
                if (intList[from].X == intList[next[from, to]].X)
                {
                    return (intList[from].Y > intList[next[from, to]].Y) ? direction.UP : direction.DOWN;
                }
                else if (intList[from].Y == intList[next[from, to]].Y)
                {
                    return (intList[from].X > intList[next[from, to]].X) ? direction.LEFT : direction.RIGHT;
                }
                else
                {
                    return playerDir;
                }
            }
        }

        void defineIntersections()
        {
            int[,] intersections = new int[layer.LayerWidth, layer.LayerHeight];
            Tile currentTile, above, right, below, left;
            for (int j = 0; j < layer.LayerHeight; j++)
            {
                for (int i = 0; i < layer.LayerWidth; i++)
                {
                    currentTile = layer.Tiles[i, j];

                    above = (j > 0) ? layer.Tiles[i, j - 1] : null;

                    below = (j < layer.LayerHeight - 1) ? layer.Tiles[i, j + 1] : null;

                    right = (i < layer.LayerWidth - 1) ? layer.Tiles[i + 1, j] : null;

                    left = (i > 0) ? layer.Tiles[i - 1, j] : null;

                    if (!currentTile.TileIndexProperties.ContainsKey("Wall") && !currentTile.Properties.ContainsKey("Cage"))
                    {
                        if ((!above.TileIndexProperties.ContainsKey("Wall")) || (!below.TileIndexProperties.ContainsKey("Wall")))
                        {
                            if ((!left.TileIndexProperties.ContainsKey("Wall")) || (!right.TileIndexProperties.ContainsKey("Wall")))
                            {
                                currentTile.Properties.Add("Intersection", 1);
                                intList.Add(new Vector2(i, j));
                            }
                        }
                    }
                }
            }
        }

        void defineAdjMatrix()
        {
            //adjMatrix = new int[intList.Count, intList.Count];

            //for (int i = 0; i < intList.Count; i++)
            //{
            //    for (int j = 0; j < intList.Count; j++)
            //    {
            //        adjMatrix[i, j] = calculate2TileDistance(intList[i], intList[j]);
            //    }
            //}

            Tile currentTile;
            int tempDistance = 0;
            bool hasWall = false;

            adjMatrix = new int[intList.Count, intList.Count];

            for (int i = 0; i < intList.Count; i++)
            {
                for (int j = 0; j < intList.Count; j++)
                {
                    adjMatrix[i, j] = (i == j) ? 0 : 9999;
                }
            }

            for (int i = 0; i < intList.Count; i++)
            {
                hasWall = false;
                tempDistance = 0;

                if (i < intList.Count - 1)
                {
                    adjMatrix[i, i + 1] = adjMatrix[i + 1, i] = calculate2TileDistance(intList[i], intList[i + 1]);
                }

                hasWall = false;
                tempDistance = 0;

                if (i < intList.Count - 1)
                {
                    for (int j = i + 1; j < intList.Count; j++)
                    {
                        if (intList[i].X == intList[j].X)
                        {
                            adjMatrix[i, j] = adjMatrix[j, i] = calculate2TileDistance(intList[i], intList[j]);
                            break;
                        }
                    }
                }
            }
        }

        public void floyd()
        {
            next = new int[intList.Count, intList.Count];

            for (int i = 0; i < intList.Count; i++)
            {
                for (int j = 0; j < intList.Count; j++)
                {
                    next[i, j] = j;
                }
            }

            for (int k = 0; k < intList.Count; k++)
            {
                for (int i = 0; i < intList.Count; i++)
                {
                    for (int j = 0; j < intList.Count; j++)
                    {
                        if (adjMatrix[i,k] + adjMatrix[k,j] < adjMatrix[i,j])
                        {
                            adjMatrix[i, j] = adjMatrix[i, k] + adjMatrix[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }
        }
        
        public Map getMap()
        {
            return map;
        }

        public int[,] getAdjMatrix()
        {
            return adjMatrix;
        }

        public xTile.Dimensions.Rectangle getViewport()
        {
            return viewport;
        }
    }
}
