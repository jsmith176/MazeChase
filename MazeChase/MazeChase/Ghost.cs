using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeChase
{
    class Ghost
    {
        Texture2D texture;
        Rectangle sourceRectangle;
        Vector2 position, renderedPosition;
        Color color;
        Random rand;

        public Ghost(Texture2D texture, Vector2 startingPosition)
        {
            this.texture = texture;
            this.position = new Vector2(startingPosition.X * 16 + 8, startingPosition.Y * 16 + 8);
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;

        }

        public virtual void Update(GameTime gameTime)
        {
            Wander();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, renderedPosition, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
        }

        public void reposition(int x, int y)
        {
            renderedPosition = new Vector2(position.X - x, position.Y - y);
        }

        public void Wander()
        {
            // Move randomly at intersections
        }

        public void Flee()
        {
            // Move in the direction that increases the distance the most from PacMan
            // assuming there is no obstacle in that direction
        }

        public void Chase()
        {
            // Move in the direction that decreases the distance the most from PacMan
            // assuming there is no obstacle in that direction
        }
    }
}
