using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace MazeChase
{
    class ScoreManager
    {
        SpriteFont spriteFont;
        Vector2 position;
        string text;
        int score;
        int lives;
        SoundEffectInstance newLifeNoise;

        public ScoreManager(SpriteFont font, SoundEffectInstance newLifeInstance)
        {
            spriteFont = font;
            position = new Vector2(10, 10);
            score = 0;
            lives = 3;
            newLifeNoise = newLifeInstance;
        }

        public virtual void Update()
        {
            if (score > 0 && score % 10000 == 0)
            {
                addLive();
            }
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

        public int getRemainingLives()
        {
            return lives;
        }

        public void addLive()
        {
            lives++;
            newLifeNoise.Play();            
        }

        public void removeLive()
        {
            lives--;
        }
    }
}
