using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

///This zombie will stop for awhile and slack.
///He will also turn from left to right at randomly when he wants to move after slacking
///He will also be active and chase the player if the player went near him.

namespace Cemetery_Escape
{
    class SlackerZombie : Zombie
    {
        public float stopInterval, stopTime;
        public bool stop;

        public override void LoadContent(ContentManager Content, int startX, int startYY, float speed2, float stopIter)
        {
            texture = Content.Load<Texture2D>("Images/Zombie/SlackerZombie");
            rows = 1;
            columns = 4;
            totalFrames = rows * columns;

            cameraPosition = new Vector2(startX, startYY);
            worldPosition = new Vector2(startX, startYY);

            startY = worldPosition.Y;
            inputSpeed = speed2;
            stopInterval = stopIter;
            stop = false;
            chasing = false;
        }

        public override void Update(GameTime gameTime)
        {
            chasePlayer();
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            stopTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            cameraPosition.Y += gravity;
            worldPosition.Y += gravity;

            if (time > interval & !stop)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;

                time = 0f;
            }

            if (stopTime > stopInterval && !chasing)
            {
                if (stop)
                {
                    stop = false;
                    if (Global.GetRandomNumber(0, 10000) > 7000)
                    {
                        if (facing == "left")
                            facing = "right";
                        else
                            facing = "left";
                    }
                }
                else
                    stop = true;

                stopTime = 0;
            }

            if (stop)
            {
                speed = 0;
            }
            else
            {
                if (state == "stand")
                {
                    gravity = 5f;
                    if (!chasing)
                        speed = inputSpeed;
                }

                if (facing == "right")
                {
                    cameraPosition.X += speed;
                    worldPosition.X += speed;
                }
                if (facing == "left")
                {
                    cameraPosition.X -= speed;
                    worldPosition.X -= speed;
                }
            }

            if (state == "fall")
            {
                gravity = 5f;
                speed = 0.1f;
            }

            if (worldPosition.Y > startY)
            {
                state = "fall";
            }

            if (chasing)
                stop = false;
            else
                stop = true;
            rectangle = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, 25, 25);
            footBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);
            rightRec = new Rectangle(rectangle.Center.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
            leftRec = new Rectangle(rectangle.Left, rectangle.Y, 1, rectangle.Height);
            headBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);

            ZombieLocation = "facing: " + facing + "\nCameraX: " + cameraPosition.X + "\nCameraY: " + cameraPosition.Y + "\nWorldX: " + worldPosition.X + "\nWorldY: " + worldPosition.Y;
        }
    }
}
