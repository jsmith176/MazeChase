using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeChase
{
    class ScoreManager
    {
        SpriteFont spriteFont;
        Vector2 position;
        string text;
        int score;
        public int lives;

        public ScoreManager(SpriteFont font)
        {
            spriteFont = font;
            position = new Vector2(10, 10);
            score = 0;
            lives = 3;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            text = "Score: " + score + " Lives: " + lives;
            spriteBatch.DrawString(spriteFont, text, new Vector2(position.X - 1, position.Y - 1), Color.Gray);
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);
        }

        public void increaseScore(int s)
        {
            score += s;
        }
    }
}
