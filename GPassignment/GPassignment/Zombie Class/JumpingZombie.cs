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

///the jumping zombie
///the jump interval can be set when calling the LoadContent of this class

namespace Cemetery_Escape
{
    class JumpingZombie : Zombie 
    {
        public bool jump;
        public float jumpSpeed;
        public float jumpTime;
        float jumpInterval = 4300;

        //ctor
        public override void LoadContent(ContentManager Content, int startX, int startYY, float speed2, float jmpInter)
        {
            texture = Content.Load<Texture2D>("Images/Zombie/JumpingZombie");
            rows = 1;
            columns = 4;
            totalFrames = rows * columns;

            cameraPosition = new Vector2(startX, startYY);
            worldPosition = new Vector2(startX, startYY);

            startY = worldPosition.Y;
            inputSpeed = speed2;
            jumpInterval = jmpInter;
            jump = false;
        }

        public override void Update(GameTime gameTime)
        {
            chasePlayer();

            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            jumpTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            cameraPosition.Y += gravity;
            worldPosition.Y += gravity;

            if (time > interval)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;
       
                time = 0f;
            }

            if (jumpTime > jumpInterval)
            {
                jumpSpeed -= 20f;
                jump = true;
                gravity = 0f;

                jumpTime = 0f;
            }

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

            if (state == "fall")
            {
                gravity = 5f;
                speed = 0.1f;
            }

            if (jump)
            {
                cameraPosition.Y += jumpSpeed;
                worldPosition.Y += jumpSpeed;
                jumpSpeed += 1f;
                if (jumpSpeed == 0)
                {
                    jumpSpeed = 0;
                    jump = false;
                    state = "fall";
                }

            }

            if (worldPosition.Y > startY)
            {
                state = "fall";
            }


            rectangle = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, 25, 25);
            footBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);
            rightRec = new Rectangle(rectangle.Center.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
            leftRec = new Rectangle(rectangle.Left, rectangle.Y, 1, rectangle.Height);
            headBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);

            ZombieLocation = "facing: " + facing + "\nCameraX: " + cameraPosition.X + "\nCameraY: " + cameraPosition.Y + "\nWorldX: " + worldPosition.X + "\nWorldY: " + worldPosition.Y;

        }

        public override void chasePlayer()
        {
            if ((UInt32)(worldPosition.X - Global.player.worldPosition.X) < 200 && ((Global.player.worldPosition.Y - worldPosition.Y < 100) && (worldPosition.Y - Global.player.worldPosition.Y < 100)))
            {
                facing = "left";
                if (speed < (Global.player.speed - 1f))
                    speed += 0.06f;
                chasing = true;
            }
            else if ((UInt32)(Global.player.worldPosition.X - worldPosition.X) < 200 && ((Global.player.worldPosition.Y - worldPosition.Y < 100) && (worldPosition.Y - Global.player.worldPosition.Y < 100)))
            {
                facing = "right";
                if (speed < (Global.player.speed - 1f))
                    speed += 0.06f;
                chasing = true;
            }
            else
            {
                speed = inputSpeed;
                chasing = false;
            }
        }
    }
}
