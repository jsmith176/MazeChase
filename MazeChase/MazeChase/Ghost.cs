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
        Vector2 position, viewportPosition, previousTile;
        Color color;
        int speed = 1;
        enum direction { STILL, UP, DOWN, LEFT, RIGHT };
        direction movementDirection;
        Random rand;

        public Ghost(ContentManager content, MapManager mapManager, Player player)
        {
            this.mapManager = mapManager;
            this.player = player;
            texture = content.Load<Texture2D>(@"PacMan");
            position = new Vector2(31 * 16 + 8, 19 * 16 + 8);
            sourceRectangle = new Rectangle(0, 0, 24, 24);
            color = Color.White;
            movementDirection = direction.STILL;
            rand = new Random();
            previousTile = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);
        }

        public virtual void Update(GameTime gameTime)
        {
            viewportPosition = new Vector2(position.X - mapManager.getViewport().X, position.Y - mapManager.getViewport().Y);

            if (mapManager.isIntersectionUnderLocation(viewportPosition) && (viewportPosition.X + mapManager.getViewport().X) % 24 == 0 && (viewportPosition.Y + mapManager.getViewport().Y) % 24 == 0)
                pickDirection();

            move();

            if (intersectsWithPlayer())
                player.isDead = true;
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
    }
}
