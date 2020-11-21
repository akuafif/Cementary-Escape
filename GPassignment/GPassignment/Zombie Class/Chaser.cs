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


namespace Cemetery_Escape
{
    class Chaser
    {
        #region Variables

        protected Texture2D texture;

        //for size of sprite
        public float scale;
        public Rectangle sourceRectangle;
        //for placement of sprite
        public Vector2 cameraPosition;
        public Vector2 worldPosition;

        //for collision
        public Rectangle rectangle;
        public Rectangle headBounds;
        public Rectangle footBounds;
        public Rectangle leftRec;
        public Rectangle rightRec;
        public bool flipOnce = false;

        //for movement
        public string facing;
        public float speed, gravity;

        //for sprite changing speed
        protected float time;
        protected float interval;

        //rows column
        protected int rows, columns, currentFrame, totalFrames;

        //on screen debug
        public string ZombieLocation;

        //2D particles
        Texture2D ParticleBase;
        public ParticleSystem particleSystem;

 

        #endregion

        public Chaser()
        {
            facing = "right";

            time = 0f;
            interval = 100f;

            currentFrame = 0;

            speed = 2.5f;
            gravity = 0.7f;

        }

        public virtual void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Images/Zombie/Chaser");
            rows = 1;
            columns = 6;
            totalFrames = rows * columns;
            scale = 1f;

            cameraPosition = new Vector2(0, 0);
            worldPosition = new Vector2(0, 0);

             sourceRectangle = new Rectangle();

            ParticleBase = Content.Load<Texture2D>("Images/Player/Particle");
            particleSystem = new ParticleSystem(new Vector2(4000, 3000));
            particleSystem.AddEmitter(new Vector2(0.021f, 0.0035f),
                                    new Vector2(0, -1), new Vector2(0.001f * MathHelper.Pi, 0.001f * -MathHelper.Pi),
                                    new Vector2(0.5f, 0.75f),
                                    new Vector2(150, 100), new Vector2(600, 700f),
                                        Color.AliceBlue, Color.CadetBlue, Color.CornflowerBlue, Color.DodgerBlue,
                                        new Vector2(0, 10), new Vector2(0, 10909), 20, Vector2.Zero, ParticleBase);
        }

        public virtual void Update(GameTime gameTime)
        {
            chasePlayer(gameTime);
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
           

            if (time > interval)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                    currentFrame = 0;

                time = 0f;
            }

            //Particle 
            particleSystem.Position = new Vector2(cameraPosition.X, cameraPosition.Y);

            rectangle = new Rectangle((int)(worldPosition.X), (int)(worldPosition.Y), sourceRectangle.Width, sourceRectangle.Height);
            footBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);
            rightRec = new Rectangle(rectangle.Center.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
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

            sourceRectangle = new Rectangle(width * column, height * row, width, height);

            particleSystem.Draw(spriteBatch, 0.2f, Vector2.Zero);
            if (facing == "right")
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), scale, SpriteEffects.None, 0);

            if (facing == "left")
                spriteBatch.Draw(texture, cameraPosition, sourceRectangle, Color.White, 0f, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), scale, SpriteEffects.FlipHorizontally, 0);
        }


        public virtual void chasePlayer(GameTime gameTime)
        {
            particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -speed);
         
            if (!(worldPosition.X == Global.player.worldPosition.X))
            {
                //behind the player
                if (worldPosition.X < Global.player.worldPosition.X)
                {
                    facing = "right";
                    //for chaser to move
                    worldPosition.X += speed;
                    cameraPosition.X += speed;
                }
                if (worldPosition.X > Global.player.worldPosition.X)
                {
                    facing = "left";
                    worldPosition.X -= speed;
                    cameraPosition.X -= speed;
                }

            }
            else
            {
                if (flipOnce != true)
                {
                    if (facing == "right")
                    {
                        facing = "left";
                        flipOnce = true;
                    }
                    else
                    {
                        facing = "right";
                        flipOnce = true;
                    }
                }
            }
            

            if (!((worldPosition.Y ) == Global.player.worldPosition.Y))
            {
                //above the player
                if ((worldPosition.Y ) < Global.player.worldPosition.Y)
                {
                    worldPosition.Y += gravity;
                    cameraPosition.Y += gravity;
                }
                //below the player
                if (worldPosition.Y  > Global.player.worldPosition.Y)
                {
                    worldPosition.Y -= gravity;
                    cameraPosition.Y -= gravity;
                }
            }
        }


        public virtual int spawnZombie()
        {
            Random random = new Random();
            int zombie = random.Next(1, 4);
            return zombie;
        }

    }
}