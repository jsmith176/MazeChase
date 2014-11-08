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
        Vector2 startingPosition;
        Random rand;

        public Ghost(Texture2D texture, Vector2 startingPosition)
        {
            this.texture = texture;
            this.startingPosition = startingPosition;
        }

        public virtual void Update(GameTime gameTime)
        {
            Wander();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

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
