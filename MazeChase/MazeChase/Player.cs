using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MazeChase
{
    class Player
    {
        ContentManager contentManager;
        InputManager inputManager;
        MapManager mapManager;
        Texture2D texture;
        Vector2 position, currentFrame, sheetSize, frameSize, speed, movement, origin;
        Rectangle sourceRectangle;
        Color color;
        float timeSinceLastFrame = 0;
        float millisecondsPerFrame = 86;
        int playerSpeed = 2;
        bool isMoving;
        public bool isDead;
        enum direction { STILL, UP, DOWN, LEFT, RIGHT };
        direction movementDirection;
        SoundEffect playerNoise;
        SoundEffectInstance playerNoiseInstance;

        public Player(ContentManager contentManager, InputManager inputManager, MapManager mapManager, Vector2 origin)
        {
            this.contentManager = contentManager;
            this.inputManager = inputManager;
            this.mapManager = mapManager;
            this.origin = origin;
            position = origin;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            sheetSize = new Vector2(3, 1);
            frameSize = new Vector2(24, 24);
            movement = Vector2.Zero;
            speed = new Vector2(2, 2);
            playerSpeed = 2;
            texture = contentManager.Load<Texture2D>(@"PacMan");
            playerNoise = contentManager.Load<SoundEffect>(@"PlayerNoise2");
            playerNoiseInstance = playerNoise.CreateInstance();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (isMoving)
            {
                nextFrame(gameTime);
            }

            updateDirection();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(texture, currentPosition, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, layerDepth);
            if (speed.X > 0)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
            }
            else if (speed.X < 0)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipHorizontally, 0.1f);
            }
            else if (speed.X == 0)
            {
                if (speed.Y > 0)
                {
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(90.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
                }
                else if (speed.Y < 0)
                {
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(270.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipVertically, 0.1f);
                }
                else
                {
                    spriteBatch.Draw(texture, position, new Rectangle(0, 0, 24, 24), color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipVertically, 0.1f);
                }
            }
        }

        public void Move(float x, float y, bool playerMove, bool viewportMove)
        {
            if (playerMove == true || viewportMove == true)
            {
                isMoving = true;
            }
            else
                isMoving = false;

            speed = new Vector2(x, y);

            if (playerMove)
            {
                position.X += speed.X;
                position.Y += speed.Y;
            }
        }

        public void updateDirection()
        {
            // Player may turn around
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

            // Check for walls
            if (inputManager.getLastKeyPressed() == Keys.Up && mapManager.isIntersectionUnderLocation(position) && !mapManager.isWall(mapManager.tileAboveLocation(position)))
            {
                if ((position.X + mapManager.getViewport().X) % 24 == 0)// where 24 is the tile width (or height) +  an offset of 1/2 tile width (or height)
                {
                    movementDirection = direction.UP;
                    movement.X = 0;
                    movement.Y = -1;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Right && mapManager.isIntersectionUnderLocation(position) && !mapManager.isWall(mapManager.tileRightOfLocation(position)))
            {
                if ((position.Y + mapManager.getViewport().Y) % 24 == 0)
                {
                    movementDirection = direction.RIGHT;
                    movement.X = 1;
                    movement.Y = 0;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Down && mapManager.isIntersectionUnderLocation(position) && !mapManager.isWall(mapManager.tileBelowLocation(position)))
            {
                if ((position.X + mapManager.getViewport().X) % 24 == 0)
                {
                    movementDirection = direction.DOWN;
                    movement.X = 0;
                    movement.Y = 1;
                }
            }
            if (inputManager.getLastKeyPressed() == Keys.Left && mapManager.isIntersectionUnderLocation(position) && !mapManager.isWall(mapManager.tileLeftOfLocation(position)))
            {
                if ((position.Y + mapManager.getViewport().Y) % 24 == 0)
                {
                    movementDirection = direction.LEFT;
                    movement.X = -1;
                    movement.Y = 0;
                }
            }

            switch (movementDirection)
            {
                case direction.UP:
                    if (!mapManager.isWall(mapManager.tileAboveLocation(position)))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().Y > 0 && position.Y == origin.Y)
                        {
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, false, true);
                            mapManager.moveViewport(0, -playerSpeed);
                        }
                        else
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                        Move(0, 0, false, false);
                    }
                    break;
                case direction.DOWN:
                    if (!mapManager.isWall(mapManager.tileBelowLocation(position)))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().Y < mapManager.getMap().DisplayHeight - mapManager.getViewport().Height && position.Y == origin.Y)
                        {
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, false, true);
                            mapManager.moveViewport(0, playerSpeed);
                        }
                        else
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                        Move(0, 0, false, false);
                    }
                    break;
                case direction.LEFT:
                    if (!mapManager.isWall(mapManager.tileLeftOfLocation(position)))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().X > 0 && position.X == origin.X)
                        {
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, false, true);
                            mapManager.moveViewport(-playerSpeed, 0);
                        }
                        else
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                        Move(0, 0, false, false);
                    }
                    break;
                case direction.RIGHT:
                    if (!mapManager.isWall(mapManager.tileRightOfLocation(position)))
                    {
                        playerNoiseInstance.Play();
                        if (mapManager.getViewport().X < mapManager.getMap().DisplayWidth - mapManager.getViewport().Width && position.X == origin.X)
                        {
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, false, true);
                            mapManager.moveViewport(playerSpeed, 0);
                        }
                        else
                            Move(playerSpeed * movement.X, playerSpeed * movement.Y, true, false);
                    }
                    else
                    {
                        playerNoiseInstance.Stop();
                        Move(0, 0, false, false);
                    }
                    break;
                default:
                    Move(0, 0, false, false);
                    break;
            }
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void nextFrame(GameTime gameTime)
        {
            timeSinceLastFrame += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }

                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
            }
        }
    }
}
