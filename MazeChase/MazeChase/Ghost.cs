using System;
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
        Vector2 position, lastInt, viewportPosition, previousTile, targetPosition, scatterLocation, cagePosition, currentFrame, firstFrame, lastFrame, frameSize;
        Color color;
        int speed = 2;
        float timeSinceLastFrame = 0;
        int millisecondsPerFrame = 100;
        enum mode { SCATTER, ATTACK, FLEE, REGENERATE };
        mode currentMode;
        direction movementDirection;
        Random rand;

        public Ghost(MapManager mapManager, Player player, Texture2D texture, Vector2 firstFrame, Vector2 lastFrame, Vector2 spawnPosition, Vector2 defensePosition)
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
        }

        public virtual void Update(GameTime gameTime)
        {
            viewportPosition = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);

            if (mapManager.isIntersectionUnderLocation(viewportPosition))
            {
                lastInt = mapManager.getTileMapUnderLocation(viewportPosition);
                if ((viewportPosition.X + mapManager.getViewport().X + 8) % 16 == 0 && (viewportPosition.Y + mapManager.getViewport().Y + 8) % 16 == 0)
                {
                    pickDirection();
                }
            }
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
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, viewportPosition, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
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

        void pickDirection()
        {
            currentMode = mode.ATTACK;
            switch (currentMode)
            {
                case mode.ATTACK:
                    targetPosition = player.getPosition();
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    Console.WriteLine(movementDirection);
                    break;
                case mode.FLEE:
                    targetPosition = player.getPosition();
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    switch (movementDirection)
                    {
                        case direction.UP:
                            movementDirection = direction.DOWN;
                            break;
                        case direction.DOWN:
                            movementDirection = direction.UP;
                            break;
                        case direction.LEFT:
                            movementDirection = direction.RIGHT;
                            break;
                        case direction.RIGHT:
                            movementDirection = direction.LEFT;
                            break;
                    }
                    break;
                case mode.REGENERATE:
                    targetPosition = cagePosition;
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    break;
                case mode.SCATTER:
                    targetPosition = scatterLocation;
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    break;
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
    }
}
