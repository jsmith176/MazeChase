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

        // Beginning time for the timing cycle
        int levelStartTime;

        // Active time in game ticks
        int activeTime;

        // Elapsed time for comparison to clock
        int elapsedTime;

        // Suspend time incrementation
        bool suspendTime;

        // First number is when Pinky leaves cage (in seconds), second number is when Inky leaves cage, third is when Clyde leaves cage
        List<int> ghost_active_level1 = new List<int>() { 5, 10, 15 };
        List<int> ghost_active_level2_4 = new List<int>() { 4, 8, 12 };
        List<int> ghost_active_level5_plus = new List<int>() { 2, 4, 6 };

        // Lengths of alternating chase/scatter cycle for ghosts, beginning on scatter. -1 means, infinitely continue chase mode at this point
        List<int> timing_level1 = new List<int>() { 7, 27, 34, 54, 59, 79, 84, -1 };
        List<int> timing_level2_4 = new List<int>() { 7, 27, 34, 54, 59, -1 };
        List<int> timing_level5_plus = new List<int>() { 5, 25, 30, 50, 55, -1 };

        // First number is normal (not eating) speed, second number is eating speed, third number is ghost flee mode speed
        List<float> player_speed_level1 = new List<float>() { 0.8f, 0.71f, 0.9f };
        List<float> player_speed_level2_4 = new List<float>() { 0.9f, 0.79f, 0.95f };
        List<float> player_speed_level5_plus = new List<float>() { 1.0f, 0.87f, 1.0f };

        // First number is chase/scatter speed, second number is flee speed, third number is regenerate speed
        List<float> ghost_speed_level1 = new List<float>() { 0.75f, 0.5f, 0.7f };
        List<float> ghost_speed_level2_4 = new List<float>() { 0.85f, 0.55f, 0.85f };
        List<float> ghost_speed_level5_plus = new List<float>() { 0.95f, 0.6f, 1.0f };

        // The length of the ghost flee mode in seconds
        List<int> flee_length = new List<int>() { 6, 5, 4, 3, 2, 5, 2, 2, 1 };

        Level(int level, int startTime)
        {
            this.level = level;
            this.levelStartTime = startTime;
            this.activeTime = 0;
            this.elapsedTime = 0;
            this.suspendTime = false;
        }

        public void Update(GameTime gameTime)
        {
            if (suspendTime == false)
            {
                activeTime++;

                // Seconds since level began
                elapsedTime = gameTime.TotalGameTime.Seconds - levelStartTime;

            }
        }

        public List<float> getPlayerSpeed()
        {
            if (level == 1)
            {
                return player_speed_level1;
            }
            else if (level >= 5)
            {
                return player_speed_level5_plus;
            }
            else
            {
                return player_speed_level2_4;
            }
        }

        public List<float> getGhostSpeed()
        {
            if (level == 1)
            {
                return ghost_speed_level1;
            }
            else if (level >= 5)
            {
                return ghost_speed_level5_plus;
            }
            else
            {
                return ghost_speed_level2_4;
            }
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

        public void stopTime()
        {
            suspendTime = true;
        }

        public void restoreTime()
        {
            suspendTime = false;
        }

        public mode getCurrentMode()
        {
            int returnThis = 0;

            // activeTime / 17 gets the total time in seconds
            if (level == 1)
            {
                foreach (int i in timing_level1)
                {
                    if (i > 0)
                    {
                        if (activeTime / 17 > i)
                        {
                            returnThis++;
                        }
                    }
                }
            }
            else if (level >= 5)
            {
                foreach (int i in timing_level5_plus)
                {
                    if (i > 0)
                    {
                        if (activeTime / 17 > i)
                        {
                            returnThis++;
                        }
                    }
                }
            }
            else
            {
                foreach (int i in timing_level2_4)
                {
                    if (i > 0)
                    {
                        if (activeTime / 17 > i)
                        {
                            returnThis++;
                        }
                    }
                }

            }

            if (returnThis % 2 == 0)
            {
                return mode.SCATTER;
            }
            else
            {
                return mode.ATTACK;
            }
            
        }
    }
}