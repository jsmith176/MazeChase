using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MazeChase
{
    class Level
    {
        // Current level
        int level;

        // First number is when Pinky leaves cage (in seconds), second number is when Inky leaves cage, third is when Clyde leaves cage
        List<int> ghost_active_level1 = new List<int>() { 5, 10, 15 };
        List<int> ghost_active_level2_4 = new List<int>() { 4, 8, 12 };
        List<int> ghost_active_level5_plus = new List<int>() { 2, 4, 6 };

        // Lengths of alternating chase/scatter cycle for ghosts, beginning on scatter. -1 means, infinitely continue chase mode at this point
        List<int> timing_level1 = new List<int>() { 7, 27, 34, 54, 59, 79, 84, -1 };
        List<int> timing_level2_4 = new List<int>() { 7, 27, 34, 54, 59, -1 };
        List<int> timing_level5_plus = new List<int>() { 5, 25, 30, 50, 55, -1 };

        // The length of the ghost flee mode in seconds
        List<int> flee_length = new List<int>() { 6, 5, 4, 3, 2, 5, 2, 2, 1 };

        Level(int level, int startTime)
        {
            this.level = level;
        }

        public void Update(GameTime gameTime)
        {

        }

        public List<int> getGhostExitTimes()
        {
            if (level == 1)
            {
                return ghost_active_level1;
            }
            else if (level >= 5)
            {
                return ghost_active_level5_plus;
            }
            else
            {
                return ghost_active_level2_4;
            }
        }
    }
}