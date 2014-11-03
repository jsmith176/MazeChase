using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeChase
{
    class Player
    {
        Texture2D texture;
        Vector2 position, currentFrame, sheetSize, frameSize;
        Vector2 speed;
        Rectangle sourceRectangle;
        Color color;
        float timeSinceLastFrame = 0;
        float millisecondsPerFrame = 64;

        public Player(Texture2D texture, Vector2 startingPosition)
        {
            this.texture = texture;
            position = startingPosition;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            sheetSize = new Vector2(3, 1);
            frameSize = new Vector2(24, 24);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (speed.X != 0 || speed.Y != 0)
                nextFrame(gameTime);
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
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipVertically, 0.1f);
                }
            }
        }

        public void Move(int x, int y, bool playerMove)
        {
            speed = new Vector2(x, y);

            if (playerMove)
            {
                position.X += speed.X;
                position.Y += speed.Y;
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
