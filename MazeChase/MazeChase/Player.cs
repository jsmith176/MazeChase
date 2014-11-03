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
        Vector2 position;
        Vector2 speed;
        Rectangle sourceRectangle;
        Color color;

        public Player(Texture2D texture, Vector2 startingPosition)
        {
            this.texture = texture;
            position = startingPosition;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(texture, currentPosition, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, layerDepth);
            if (speed.X > 0)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(90.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
            }
            else if (speed.X < 0)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(270.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipHorizontally, 0.1f);
            }
            else if (speed.X == 0)
            {
                if (speed.Y > 0)
                {
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(180.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
                }
                else if (speed.Y < 0)
                {
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(180.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipVertically, 0.1f);
                }
                else
                {
                    spriteBatch.Draw(texture, position, sourceRectangle, color, MathHelper.ToRadians(180.0f), new Vector2(12, 12), 1f, SpriteEffects.FlipVertically, 0.1f);
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
    }
}
