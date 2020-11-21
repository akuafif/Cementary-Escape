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

///Zombie will disappear from user screen
///Zombie will able to move when he disappear

namespace Cemetery_Escape
{
    class DisappearZombie : Zombie
    {
        float disappearTime, disappearInterval;
        bool disappear = false;

        public override void LoadContent(ContentManager Content, int startX, int startYY, float speed2, float disIter)
        {
            texture = Content.Load<Texture2D>("Images/Zombie/DisappearZombie1");
            rows = 1;
            columns = 8;
            totalFrames = rows * columns;

            cameraPosition = new Vector2(startX, startYY);
            worldPosition = new Vector2(startX, startYY);

            startY = worldPosition.Y;
            inputSpeed = speed2;
            disappearInterval = disIter;
            disappear = false;
        }

        public override void Update(GameTime gameTime)
        {
            chasePlayer();
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            disappearTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            cameraPosition.Y += gravity;
            worldPosition.Y += gravity;

            if (time > (interval*2))
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;

                time = 0f;
            }

            //if (disappearTime > disappearInterval)
            //{
            //    if (disappear)
            //        disappear = false;
            //    else
            //        disappear = true;

            //    disappearTime = 0;
            //}

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            int width = texture.Width / columns; //width of sprite in spirte sheet
            int height = texture.Height / rows; //height of sprite in sprite sheet

            int row = (int)((float)currentFrame / (float)columns);
            int column = currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

            if (facing == "right" && !disappear)
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 0.28f, SpriteEffects.None, 0);

            if (facing == "left" && !disappear)
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 0.28f, SpriteEffects.FlipHorizontally, 0);
        }

        public override  void chasePlayer()
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
            
            ////jump
            //if (chasing)
            //{
            //    if ( ((UInt32)(Global.player.worldPosition.X - worldPosition.X) ) > 0)  && ((UInt32)(worldPosition.X - Global.player.worldPosition.X) < 200) )
            //    {

            //    }
            //}
        }
    }
}
