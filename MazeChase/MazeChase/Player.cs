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
            spriteBatch.Draw(texture, position, sourceRectangle, color, 0.0f, new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
        }

        public void Move(int x, int y)
        {
            position.X += x;
            position.Y += y;
        }

        public Vector2 getPosition()
        {
            return position;
        }
    }
}
