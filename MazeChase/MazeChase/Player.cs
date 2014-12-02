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
        ScoreManager scoreManager;
        Texture2D texture;
        Vector2 position, lastInt, currentFrame, sheetSize, frameSize, speed, movement, origin;
        Rectangle sourceRectangle;
        Color color;
        float timeSinceLastFrame = 0;
        float millisecondsPerFrame = 86;
        float rotation = 0;
        int[] wall = { 0, 0, 0, 0 };
        float playerSpeed = 2;
        bool isMoving;
        int ghostEatTime;
        public bool canEatGhosts;
        public bool isDead;
        direction movementDirection;
        SoundEffect playerNoise;
        SoundEffect deathNoise;
        SoundEffectInstance playerNoiseInstance;
        SoundEffectInstance deathNoiseInstance;

        public Player(ContentManager contentManager, InputManager inputManager, MapManager mapManager, ScoreManager scoreManager, Vector2 origin)
        {
            this.contentManager = contentManager;
            this.inputManager = inputManager;
            this.mapManager = mapManager;
            this.scoreManager = scoreManager;
            this.origin = origin;
            position = new Vector2(origin.X, origin.Y + (1 * 16 + 8));
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            sheetSize = new Vector2(3, 4);
            frameSize = new Vector2(24, 24);
            movement = Vector2.Zero;
            speed = new Vector2(2, 2);
            playerSpeed = 2;
            texture = contentManager.Load<Texture2D>(@"PacMan");
            playerNoise = contentManager.Load<SoundEffect>(@"PlayerNoise2");
            playerNoiseInstance = playerNoise.CreateInstance();
            canEatGhosts = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            ghostEatTime--;

            if (ghostEatTime > 0)
            {
                canEatGhosts = true;
            }
            else
            {
                canEatGhosts = false;
            }

            if (mapManager.isWall(mapManager.tileAboveLocation(position)))
                wall[0] = 1;
            if (mapManager.isWall(mapManager.tileRightOfLocation(position)))
                wall[1] = 1;
            if (mapManager.isWall(mapManager.tileBelowLocation(position)))
                wall[2] = 1;
            if (mapManager.isWall(mapManager.tileLeftOfLocation(position)))
                wall[3] = 1;

            if (isMoving)
            {
                nextFrame(gameTime);
                playerNoiseInstance.Play();
            }
            else
                playerNoiseInstance.Stop();

            if (mapManager.isIntersectionUnderLocation(position))
            {
                lastInt = mapManager.getTileMapUnderLocation(position);

                if ((position.X + mapManager.getViewport().X + 8) % 16 == 0 && (position.Y + mapManager.getViewport().Y + 8) % 16 == 0)
                {
                    pickDirection();
                }
                
            }
            else if (movementDirection == direction.STILL)
                pickDirection();

            move();

            for (int i = 0; i < wall.Count(); i++)
                wall[i] = 0;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(rotation), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
        }

        void pickDirection()
        {
            switch(inputManager.getLastKeyPressed())
            {
                case Keys.Up:
                    if (wall[0] == 0)
                        movementDirection = direction.UP;
                    break;
                case Keys.Right:
                    if (wall[1] == 0)
                        movementDirection = direction.RIGHT;
                    break;
                case Keys.Down:
                    if (wall[2] == 0)
                        movementDirection = direction.DOWN;
                    break;
                case Keys.Left:
                    if (wall[3] == 0)
                        movementDirection = direction.LEFT;
                    break;
            }
        }

        void move()
        {
            if (movementDirection == direction.UP && wall[0] == 0)
            {
                if (inputManager.getLastKeyPressed() == Keys.Down)
                {
                    movementDirection = direction.DOWN;
                }
                else
                {
                    if (mapManager.getViewport().Y > 0 && position.Y == origin.Y)
                    {
                        mapManager.moveViewport(0, -playerSpeed);
                    }
                    else
                        position.Y -= playerSpeed;
                    rotation = 270;
                    isMoving = true;
                }
            }
            else if (movementDirection == direction.RIGHT && wall[1] == 0)
            {
                if (inputManager.getLastKeyPressed() == Keys.Left)
                {
                    movementDirection = direction.LEFT;
                }
                else
                {
                    if (mapManager.getViewport().X < mapManager.getMap().DisplayWidth - mapManager.getViewport().Width && position.X == origin.X)
                    {
                        mapManager.moveViewport(playerSpeed, 0);
                    }
                    else
                        position.X += playerSpeed;
                    rotation = 0;
                    isMoving = true;
                }
            }
            else if (movementDirection == direction.DOWN && wall[2] == 0)
            {
                if (inputManager.getLastKeyPressed() == Keys.Up)
                {
                    movementDirection = direction.UP;
                }
                else
                {
                    if (mapManager.getViewport().Y < mapManager.getMap().DisplayHeight - mapManager.getViewport().Height && position.Y == origin.Y)
                    {
                        mapManager.moveViewport(0, playerSpeed);
                    }
                    else
                        position.Y += playerSpeed;
                    rotation = 90;
                    isMoving = true;
                }
            }
            else if (movementDirection == direction.LEFT && wall[3] == 0)
            {
                if (inputManager.getLastKeyPressed() == Keys.Right)
                {
                    movementDirection = direction.RIGHT;
                }
                else
                {
                    if (mapManager.getViewport().X > 0 && position.X == origin.X)
                    {
                        mapManager.moveViewport(-playerSpeed, 0);
                    }
                    else
                        position.X -= playerSpeed;
                    rotation = 180;
                    isMoving = true;
                }
            }
            else
                isMoving = false;
        }

        public void modeSwap(int adjuster)
        {
            ghostEatTime = adjuster;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public direction getDirection()
        {
            return movementDirection;
        }

        public Vector2 getLastInt()
        {
            return lastInt;
        }

        public bool lowEatTime()
        {
            if (ghostEatTime <= 150)
                return true;
            return false;
        }

        void nextFrame(GameTime gameTime)
        {
            timeSinceLastFrame += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                    currentFrame.X = 0;

                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
            }
        }

        public void deathAnimation(GameTime gameTime)
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

            if (currentFrame == new Vector2(2, 3))
            {
                currentFrame *= 0;
                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
                //mapManager.setViewportPosition((6 * 16) + 8, (15 * 16));
                movementDirection = direction.STILL;
                inputManager.setLastKeyPressed(Keys.F1);
                //position = new Vector2(((31 * 16) + 8) - mapManager.getViewport().X, ((31 * 16) + 8) - mapManager.getViewport().Y);
                mapManager.setViewportPosition(((34 * 16 + 8) - mapManager.getViewport().Width / 2), (31 * 16 + 8) - mapManager.getViewport().Height / 2 - (1 * 16 + 8));
                scoreManager.removeLive();
                position = new Vector2(origin.X, origin.Y + (1 * 16 + 8));
                
                isDead = false;
            }
        }
    }
}
