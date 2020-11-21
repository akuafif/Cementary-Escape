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

/// Parent class of all the zombie class
/// does not have special ability
/// unlike the child, this zombie just move normally

namespace Cemetery_Escape
{
    class Zombie
    {
        #region Player Variable

        protected Texture2D texture;

        //for placement of sprite
        public Vector2 cameraPosition;
        public Vector2 worldPosition;

        //for collision
        public Rectangle rectangle;
        public Rectangle headBounds;
        public Rectangle footBounds;
        public Rectangle leftRec;
        public Rectangle rightRec;

        //for movement
        public string facing, state;
        public float gravity;
        public float startY;
        public float speed;
        public float inputSpeed;

        //for sprite changing speed
        protected float time;
        protected float interval;
        
        //rows column
        protected int rows, columns, currentFrame, totalFrames;

        //on screen debug
        public string ZombieLocation;
        
        //chasing var
        public bool chasing;

        #endregion

        public Zombie()
        {   
            facing = "right";
            state = "stand";
            time = 0f;
            interval = 100f;

            currentFrame = 0;

            gravity = 0f;
            speed = 0f; //can change speed dependin on situation
            inputSpeed = 2f; //default speed
        }

        public virtual void LoadContent(ContentManager Content, int startX, int startYY, float speed2)
        {
            texture = Content.Load<Texture2D>("Images/Zombie/NormalZombie");
            rows = 1;
            columns = 4;
            totalFrames = rows * columns;

            cameraPosition = new Vector2(startX, startYY);
            worldPosition = new Vector2(startX, startYY);

            startY = worldPosition.Y;
            inputSpeed = speed2;
        }

        public virtual void LoadContent(ContentManager Content, int startX, int startYY, float speed2, float dump)
        { }


        public virtual void Update(GameTime gameTime)
        {
            chasePlayer(); //methid for chase player
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            cameraPosition.Y += gravity;
            worldPosition.Y += gravity;

            if (time > interval)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;

                time = 0f;
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

            //if (worldPosition.Y > startY)
            //{
            //    state = "fall";
            //}

            rectangle = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, 25, 25);
            footBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);
            rightRec = new Rectangle(rectangle.Right, rectangle.Y, 1, rectangle.Height); //discovered source of bug!!! should be 1 instead of /2
            leftRec = new Rectangle(rectangle.Left, rectangle.Y, 1, rectangle.Height);
            headBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);

            ZombieLocation = "facing: " + facing + "\nCameraX: " + cameraPosition.X + "\nCameraY: " + cameraPosition.Y + "\nWorldX: " + worldPosition.X + "\nWorldY: " + worldPosition.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            int width = texture.Width / columns; //width of sprite in spirte sheet
            int height = texture.Height / rows; //height of sprite in sprite sheet

            int row = (int)((float)currentFrame / (float)columns);
            int column = currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

            if (facing == "right")
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 0.28f, SpriteEffects.None, 0);
            
            if(facing == "left")
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 0.28f, SpriteEffects.FlipHorizontally, 0);
        }

        public virtual void chasePlayer()
        {
            if ((UInt32)(worldPosition.X - Global.player.worldPosition.X) < 200 && ((Global.player.worldPosition.Y - worldPosition.Y < 100) && (worldPosition.Y - Global.player.worldPosition.Y < 100)))
            {
                facing = "left";
                if (speed < (Global.player.speed - 2f))
                    speed += 0.06f;
                chasing = true;
            }
            else if ((UInt32)(Global.player.worldPosition.X - worldPosition.X) < 200 && ((Global.player.worldPosition.Y - worldPosition.Y < 100) && (worldPosition.Y - Global.player.worldPosition.Y < 100)))
            {
                facing = "right";
                if (speed < (Global.player.speed - 2f))
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
