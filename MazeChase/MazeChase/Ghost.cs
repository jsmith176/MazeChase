using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MazeChase
{
    class Ghost
    {
        MapManager mapManager;
        ScoreManager scoreManager;
        Player player;
        Texture2D texture;
        Rectangle sourceRectangle;
        Vector2 position, lastInt, viewportPosition, previousTile, targetPosition, scatterLocation, cagePosition, currentFrame, firstFrame, lastFrame, scaredFrame, frameSize;
        Color color;
        SoundEffect playerDeathSound, ghostDeathSound;
        SoundEffectInstance playerDeathSoundInstance, ghostDeathSoundInstance;
        int speed = 1;
        float timeSinceLastFrame = 0;
        int millisecondsPerFrame = 250;
        mode currentMode;
        direction movementDirection;
        Random rand;
        bool previouslyEaten;

        public Ghost(ContentManager content, MapManager mapManager, ScoreManager scoreManager, Player player, Texture2D texture, Vector2 firstFrame, Vector2 lastFrame, Vector2 spawnPosition, Vector2 defensePosition)
        {
            this.mapManager = mapManager;
            this.scoreManager = scoreManager;
            this.player = player;
            this.texture = texture;
            this.firstFrame = firstFrame;
            scaredFrame = new Vector2(0, 5);
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
            cagePosition = mapManager.getCagePosition();
            playerDeathSound = content.Load<SoundEffect>(@"pacman_death");
            ghostDeathSound = content.Load<SoundEffect>(@"pacman_eatghost");
            playerDeathSoundInstance = playerDeathSound.CreateInstance();
            ghostDeathSoundInstance = ghostDeathSound.CreateInstance();
            previouslyEaten = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            //Console.WriteLine(currentMode);

            viewportPosition = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);

            if (currentMode == mode.REGENERATE)
            {
                if (viewportPosition.X % 2 != 0)
                {
                    viewportPosition.X++;
                }

                if (viewportPosition.Y % 2 != 0)
                {
                    viewportPosition.Y++;
                }

                speed = 2;

                Console.WriteLine(viewportPosition.X + " " + viewportPosition.Y);
            }

            if (player.canEatGhosts == true)
            {
                if (previouslyEaten)
                {
                    
                }
                else
                {
                    currentMode = mode.FLEE;
                    speed = 1;
                }
            }
            else
            {
                if (previouslyEaten)
                {
                    currentMode = mode.SCATTER;
                }
                else if(currentMode != mode.REGENERATE)
                {
                    currentMode = mode.ATTACK;
                }

                if (viewportPosition.X % 2 != 0)
                {
                    viewportPosition.X++;
                }

                if (viewportPosition.Y % 2 != 0)
                {
                    viewportPosition.Y++;
                }

                speed = 2;
            }

            if (mapManager.isIntersectionUnderLocation(viewportPosition))
            {
                lastInt = mapManager.getTileMapUnderLocation(viewportPosition);
                if ((viewportPosition.X + mapManager.getViewport().X + 8) % 16 == 0 && (viewportPosition.Y + mapManager.getViewport().Y + 8) % 16 == 0)
                {
                    pickDirection();
                }
                else
                {
                    if (currentMode == mode.REGENERATE)
                    {
                        Console.Write("");
                    }
                }
            }
            else if (movementDirection == direction.STILL)
                pickDirection();
            else
            {
                if (currentMode == mode.REGENERATE)
                {
                    Console.Write("");
                }
            }

            move();
            nextFrame(gameTime);

            if (intersectsWithPlayer())
            {
                if (currentMode == mode.ATTACK || currentMode == mode.SCATTER)
                {
                    playerDeathSoundInstance.Play();
                    player.isDead = true;
                    //previouslyEaten = false;
                }
                else if(currentMode == mode.FLEE)
                {
                    scoreManager.increaseScore(100);
                    ghostDeathSoundInstance.Play();
                    currentMode = mode.REGENERATE;
                    previouslyEaten = true;
                }
                else if (currentMode == mode.REGENERATE)
                {

                }
            }

            if (currentMode == mode.ATTACK)
            {
                speed = 2;
                targetPosition = player.getPosition();
            }
            else if (currentMode == mode.REGENERATE)
            {
                speed = 4;
                targetPosition = cagePosition;
            }
            else if (currentMode == mode.SCATTER)
            {
                speed = 2;
                targetPosition = scatterLocation;
            }
            else
            {
                targetPosition = player.getPosition();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (currentMode == mode.FLEE)
            {
                spriteBatch.Draw(texture, viewportPosition, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
            }
            else
            {
                spriteBatch.Draw(texture, viewportPosition, sourceRectangle, color, MathHelper.ToRadians(0.0f), new Vector2(12, 12), 1f, SpriteEffects.None, 0.1f);
            }
        }

        void nextFrame(GameTime gameTime)
        {
            if (currentMode == mode.FLEE)
            {
                if (currentFrame.Y != 5)
                {
                    currentFrame = scaredFrame;
                }

                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    timeSinceLastFrame = 0;
                    if (player.lowEatTime())
                    {
                        currentFrame.X++;
                        if (currentFrame.X > 3.0f)
                        {
                            currentFrame.X = 0;
                        }
                    }
                    else
                    {
                        if (currentFrame.X == 0.0f)
                        {
                            currentFrame.X = 1.0f;
                        }
                        else if (currentFrame.X == 1.0f)
                        {
                            currentFrame.X = 0.0f;
                        }
                    }
                }

                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
            }
            else if (currentMode == mode.REGENERATE)
            {
                if (movementDirection == direction.UP)
                {
                    currentFrame = new Vector2(0, 4);
                }
                else if (movementDirection == direction.DOWN)
                {
                    currentFrame = new Vector2(2, 4);
                }
                else if (movementDirection == direction.LEFT)
                {
                    currentFrame = new Vector2(4, 4);
                }
                else if (movementDirection == direction.RIGHT)
                {
                    currentFrame = new Vector2(6, 4);
                }

                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
            }
            else
            {
                currentFrame.Y = firstFrame.Y;
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    timeSinceLastFrame = 0;
                    if (currentFrame.X % 2 == 0)
                    {
                        currentFrame.X++;
                    }
                    else
                    {
                        currentFrame.X--;
                    }
                    
                }

                sourceRectangle = new Rectangle((int)currentFrame.X * (int)frameSize.X,
                        (int)currentFrame.Y * (int)frameSize.Y,
                        (int)frameSize.X, (int)frameSize.Y);
            }
        }

        void pickDirection()
        {
            switch (currentMode)
            {
                case mode.ATTACK:
                    //targetPosition = player.getPosition();
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    //Console.WriteLine(movementDirection);
                    break;
                case mode.FLEE:
                    //targetPosition = player.getPosition();
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    pickFleeDirection();
                    break;
                case mode.REGENERATE:
                    //targetPosition = cagePosition;
                    movementDirection = mapManager.getFloydDirection(lastInt, cagePosition, player.getDirection());
                    if (mapManager.isCageUnderLocation(viewportPosition))
                    {
                        currentMode = mode.ATTACK;
                        while (viewportPosition.X % 2 != 0)
                        {
                            viewportPosition.X++;
                        }
                        while (viewportPosition.Y % 2 != 0)
                        {
                            viewportPosition.Y++;
                        }

                        Console.Write("");
                    }
                    break;
                case mode.SCATTER:
                    //targetPosition = scatterLocation;
                    movementDirection = mapManager.getFloydDirection(lastInt, player.getLastInt(), player.getDirection());
                    break;
            }
        }

        void move()
        {
            if (movementDirection == direction.UP)
            {
                position.Y -= speed;
                firstFrame.X = 0;
            }
            else if (movementDirection == direction.RIGHT)
            {
                position.X += speed;
                firstFrame.X = 6;
            }
            else if (movementDirection == direction.DOWN)
            {
                position.Y += speed;
                firstFrame.X = 2;
            }
            else if (movementDirection == direction.LEFT)
            {
                position.X -= speed;
                firstFrame.X = 4;
            }
        }

        public void pickFleeDirection()
        {
            switch (movementDirection)
            {
                case direction.UP:
                    if (mapManager.isWall(mapManager.tileBelowLocation(viewportPosition)))
                    {
                        if (mapManager.isWall(mapManager.tileLeftOfLocation(viewportPosition)))
                        {
                            movementDirection = direction.RIGHT;
                        }
                        else
                        {
                            movementDirection = direction.LEFT;
                        }
                    }
                    else
                    {
                        movementDirection = direction.DOWN;
                    }
                    break;
                case direction.DOWN:
                    if (mapManager.isWall(mapManager.tileAboveLocation(viewportPosition)))
                    {
                        if (mapManager.isWall(mapManager.tileRightOfLocation(viewportPosition)))
                        {
                            movementDirection = direction.LEFT;
                        }
                        else
                        {
                            movementDirection = direction.RIGHT;
                        }
                    }
                    else
                    {
                        movementDirection = direction.UP;
                    }
                    break;
                case direction.LEFT:
                    if (mapManager.isWall(mapManager.tileRightOfLocation(viewportPosition)))
                    {
                        if (mapManager.isWall(mapManager.tileBelowLocation(viewportPosition)))
                        {
                            movementDirection = direction.UP;
                        }
                        else
                        {
                            movementDirection = direction.DOWN;
                        }
                    }
                    else
                    {
                        movementDirection = direction.RIGHT;
                    }
                    break;
                case direction.RIGHT:
                    if (mapManager.isWall(mapManager.tileLeftOfLocation(viewportPosition)))
                    {
                        if (mapManager.isWall(mapManager.tileAboveLocation(viewportPosition)))
                        {
                            movementDirection = direction.DOWN;
                        }
                        else
                        {
                            movementDirection = direction.UP;
                        }
                    }
                    else
                    {
                        movementDirection = direction.LEFT;
                    }
                    break;
            }
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
