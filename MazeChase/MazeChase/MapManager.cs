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
        class intersection
        {
            public intersection(int x, int y)
            {
                xCoord = x;
                yCoord = y;
            }

            int xCoord;
            int yCoord;

            Vector2 getCoords()
            {
                return new Vector2(xCoord, yCoord);
            }
        }

        ContentManager contentManager;
        GraphicsDevice graphicsDevice;
        ScoreManager scoreManager;
        Layer layer;
        int[,] intersections;
        int[,] adjMatrix;
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

        public direction floyd(int[,] adjMatrix)
        {
            

            return direction.STILL;
        }

        public Map getMap()
        {
            return map;
        }

        public int[,] getIntersections()
        {
            return intersections;
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
            if (intersections[(int)(viewport.X + location.X) / 16, (int)(viewport.Y + location.Y) / 16] == 1)
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
                                {
                                    intersections[i, j] = 1;
                                    currentTile.Properties.Add("Intersection",1);
                                }
                                else
                                {
                                    intersections[i, j] = 2;
                                }
                            }
                            else if (left.TileIndexProperties.Count == 0 || right.TileIndexProperties.Count == 0)
                            {
                                if (above.TileIndexProperties.Count == 0 || below.TileIndexProperties.Count == 0)
                                {
                                    intersections[i, j] = 1;
                                    currentTile.Properties.Add("Intersection",1);
                                }
                                else
                                {
                                    intersections[i, j] = 2;
                                }
                            }
                        }
                        else
                            intersections[i, j] = 2;
                    }
                }
            }

            // 0 is wall, 1 is intersection, 2 is pathway
            defineAdjMatrix();

        }

        void defineAdjMatrix()
        {
            Tile currentTile;
            int tempIndex = 0;
            int tempDistance = 0;
            bool hasWall = false;

            for (int i = 0; i < layer.LayerHeight; i++)
            {
                for (int j = 0; j < layer.LayerWidth; j++)
                {
                    currentTile = layer.Tiles[j,i];
                    if (currentTile.Properties.ContainsKey("Intersection"))
                    {
                        intList.Add(new Vector2(j,i));
                    }
                }
            }

            adjMatrix = new int[intList.Count, intList.Count];

            for (int i = 0; i < intList.Count; i++)
            {
                for (int j = 0; j < intList.Count; j++)
                {

                    adjMatrix[i,j] = (i == j) ? 0 : -1;
                }
            }
            
            // Tested up to this point

            for (int i = 0; i < intList.Count; i++)
            {
                hasWall = false;
                tempDistance = 0;

                if (i < intList.Count - 1)
                {
                    if (intList[i].Y == intList[i + 1].Y)
                    {
                        for (int k = (int)intList[i].X; k < (int)intList[i + 1].X; k++)
                        {
                            if (layer.Tiles[k,(int)intList[i].Y].TileIndexProperties.ContainsKey("Wall"))
                            {
                                hasWall = true;
                            }
                            else
                            {
                                tempDistance++;
                            }
                        }

                        // Either assign 9999 if a wall between or the distance if not
                        adjMatrix[i, i + 1] = adjMatrix[i + 1, i] = (hasWall == true) ? -1 : tempDistance;
                    }
                    else
                    {
                        // Different x levels
                        adjMatrix[i, i + 1] = -1;
                    }
                }

                hasWall = false;
                tempDistance = 0;

                if (i < intList.Count - 1)
                {
                    for (int j = i + 1; j < intList.Count; j++)
                    {
                        if (intList[i].X == intList[j].X)
                        {
                            for (int m = (int)intList[i].Y; m < (int)intList[j].Y; m++)
                            {
                                if (layer.Tiles[(int)intList[i].X, m].TileIndexProperties.ContainsKey("Wall"))
                                {
                                    hasWall = true;
                                }
                                else
                                {
                                    tempDistance++;
                                }
                            }

                            adjMatrix[i, j] = adjMatrix[j, i] = (hasWall == true) ? -1 : tempDistance;
                            break;
                        }
                        else
                        {

                        }
                    }
                }


            }
        }
    }
}
