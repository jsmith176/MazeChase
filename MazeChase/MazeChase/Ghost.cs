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
        enum direction { UP, DOWN, LEFT, RIGHT };
        direction movementDirection;
        Random rand;

        public Ghost(Texture2D texture, Vector2 startingPosition)
        {
            this.texture = texture;
            this.position = new Vector2(startingPosition.X * 16 + 8, startingPosition.Y * 16 + 8);
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            rand = new Random();
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

        public Vector2 getPosition()
        {
            return renderedPosition;
        }

        public void newDirection()
        {
            int r = rand.Next(1, 4);
            switch(r)
            {
                case 1:
                    movementDirection = direction.UP;
                    break;
                case 2:
                    movementDirection = direction.RIGHT;
                    break;
                case 3:
                    movementDirection = direction.DOWN;
                    break;
                case 4:
                    movementDirection = direction.LEFT;
                    break;
            }
        }

        public void Wander()
        {
            // Move randomly at intersections
            if (movementDirection == direction.UP)
                position.Y -= 1;
            else if (movementDirection == direction.RIGHT)
                position.X += 1;
            else if (movementDirection == direction.DOWN)
                position.Y += 1;
            else if (movementDirection == direction.LEFT)
                position.X -= 1;
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
            // Floyds shortest path psuedocode in i drive
        }
    }
}
