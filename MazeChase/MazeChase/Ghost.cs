﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MazeChase
{
    class Ghost
    {
        MapManager mapManager;
        Player player;
        Texture2D texture;
        Rectangle sourceRectangle;
        Vector2 position, viewportPosition, previousTile, targetPosition, scatterLocation, cagePosition, currentFrame, firstFrame, lastFrame, frameSize;
        Color color;
        int speed = 1;
        float timeSinceLastFrame = 0;
        int millisecondsPerFrame = 100;
        enum mode { SCATTER, ATTACK, FLEE, REGENERATE };
        mode currentMode;
        direction movementDirection;
        Random rand;

        SpriteFont font;
        string up, down, left, right;

        public Ghost(MapManager mapManager, Player player, Texture2D texture, Vector2 firstFrame, Vector2 lastFrame, Vector2 spawnPosition, Vector2 defensePosition, SpriteFont font)
        {
            this.mapManager = mapManager;
            this.player = player;
            this.texture = texture;
            this.firstFrame = firstFrame;
            currentFrame = new Vector2(0, 0);
            this.lastFrame = lastFrame;
            frameSize = new Vector2(24, 24);
            position = spawnPosition;
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            movementDirection = direction.STILL;
            rand = new Random();
            previousTile = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);
            targetPosition = scatterLocation = defensePosition;
            cagePosition = spawnPosition;

            this.font = font;
            up = "0";
            down = "0";
            left = "0";
            right = "0";
        }

        public virtual void Update(GameTime gameTime)
        {
            viewportPosition = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);

            if (mapManager.isIntersectionUnderLocation(viewportPosition) && (viewportPosition.X + mapManager.getViewport().X + 8) % 16 == 0 && (viewportPosition.Y + mapManager.getViewport().Y + 8) % 16 == 0)
                pickDirection();
            else if (movementDirection == direction.STILL)
                pickDirection();

            move();
            nextFrame(gameTime);

            if (intersectsWithPlayer())
            {
                player.isDead = true;
                position = cagePosition;
            }

            if (currentMode == mode.ATTACK)
            {
                targetPosition = player.getPosition();
            }
            else if (currentMode == mode.REGENERATE)
            {
                targetPosition = cagePosition;
            }
            else if (currentMode == mode.SCATTER)
            {
                targetPosition = scatterLocation;
            }
            else
            {
                targetPosition = player.getPosition();
            }

            //pathing();
        }

        void pickDirection()
        {
            int d = 0, r = 0;
            int[] wall = { 0, 0, 0, 0 };

            if (mapManager.isWall(mapManager.tileAboveLocation(viewportPosition)) || movementDirection == direction.DOWN)
                wall[0] = 1;
            if (mapManager.isWall(mapManager.tileRightOfLocation(viewportPosition)) || movementDirection == direction.LEFT)
                wall[1] = 1;
            if (mapManager.isWall(mapManager.tileBelowLocation(viewportPosition)) || movementDirection == direction.UP)
                wall[2] = 1;
            if (mapManager.isWall(mapManager.tileLeftOfLocation(viewportPosition)) || movementDirection == direction.RIGHT)
                wall[3] = 1;

            // If the ghost is near a gate set wall to 0;

            //if (mapManager.tileAboveLocation(position) == new Vector2((32 * 16) + 8, (21 * 16) + 8) ||
            //    position == new Vector2((33 * 16) + 8, (21 * 16) + 8))
            //    wall[0] = 0;
            //else if ((mapManager.tileBelowLocation(position) == new Vector2((32 * 16) + 8, (19 * 16) + 8) ||
            //    position == new Vector2((33 * 16) + 8, (19 * 16) + 8)))
            //    wall[2] = 0;

            up = wall[0].ToString();
            down = wall[2].ToString();
            left = wall[3].ToString();
            right = wall[1].ToString();

            while (d == 0)
            {
                r = rand.Next(0, 4);
                if (wall[r] == 0)
                    d = r + 1;
            }

            switch(d)
            {
                case 1:
                    movementDirection = direction.UP;
                    firstFrame.X = 0;
                    lastFrame.X = 1;
                    currentFrame = firstFrame;
                    break;
                case 2:
                    movementDirection = direction.RIGHT;
                    firstFrame.X = 6;
                    lastFrame.X = 7;
                    currentFrame = firstFrame;
                    break;
                case 3:
                    movementDirection = direction.DOWN;
                    firstFrame.X = 2;
                    lastFrame.X = 3;
                    currentFrame = firstFrame;
                    break;
                case 4:
                    movementDirection = direction.LEFT;
                    firstFrame.X = 4;
                    lastFrame.X = 5;
                    currentFrame = firstFrame;
                    break;
            }
        }

        void pathing()
        {
            // This function will implement the Floyd's algorithm to decide how the ghost should move
            // If mode is attack, perform the Floyd algorithm and chase, depending on the behavior
            // If mode is scatter, perform Floyd to go to scatter location. If player gets too close, then switch to flee
            // If mode is flee, then perform Floyd and do the opposite move
            // If mode is regenerate, perform Floyd to return to the cage and regenerate
            //

            int[,] tempInt = mapManager.getIntersections();
            int[,] results = new int[(int)Math.Sqrt(tempInt.Length), (int)Math.Sqrt(tempInt.Length)];

            int n = (int)Math.Sqrt(tempInt.Length);

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (tempInt[i, k] + tempInt[k, j] < tempInt[i, j])
                        {
                            results[i, j] = results[i, k] + results[k, j];
                        }
                    }
                }
            }

        }

        void move()
        {
            if (movementDirection == direction.UP)
                position.Y -= speed;
            if (movementDirection == direction.RIGHT)
                position.X += speed;
            if (movementDirection == direction.DOWN)
                position.Y += speed;
            if (movementDirection == direction.LEFT)
                position.X -= speed;
        }

        bool intersectsWithPlayer()
        {
            if (Math.Abs(viewportPosition.X - player.getPosition().X) < 8 &&
                Math.Abs(viewportPosition.Y - player.getPosition().Y) < 8)
                return true;
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, viewportPosition, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(font, up, new Vector2(viewportPosition.X - 5, viewportPosition.Y - 15), Color.Green);
            spriteBatch.DrawString(font, down, new Vector2(viewportPosition.X - 5, viewportPosition.Y + 5), Color.Green);
            spriteBatch.DrawString(font, left, new Vector2(viewportPosition.X - 15, viewportPosition.Y - 5), Color.Green);
            spriteBatch.DrawString(font, right, new Vector2(viewportPosition.X + 5, viewportPosition.Y - 5), Color.Green);
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
            // Floyds shortest path psuedocode in i drive
        }

        void nextFrame(GameTime gameTime)
        {
            timeSinceLastFrame += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X > lastFrame.X)
                {
                    currentFrame.X = firstFrame.X;
                }
            }

            sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                    (int)currentFrame.Y * (int)frameSize.Y,
                    (int)frameSize.X, (int)frameSize.Y);
        }
    }
}
