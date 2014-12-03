using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MazeChase
{

    class InputManager
    {
        Keys lastKeyPressed = Keys.F1; // An unused key for initialization purposes

        public virtual void Update(GameTime gameTime)
        {
            // Stores the last key pressed by the user in lastKeyPressed
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lastKeyPressed = Keys.Up;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lastKeyPressed = Keys.Right;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lastKeyPressed = Keys.Down;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lastKeyPressed = Keys.Left;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                lastKeyPressed = Keys.S;
            }
        }

        public Keys getLastKeyPressed()
        {
            return lastKeyPressed;
        }

        public void setLastKeyPressed(Keys k)
        {
            lastKeyPressed = k;
        }
    }
}
