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

/// <summary>
/// At the moment, this class will only contains methods/functions for the character to move, jump.
/// It does not controls any sort of collision.
/// 
/// </summary>

namespace Cemetery_Escape
{
    class Player
    {
        #region Player Variable

        public Texture2D texture;
        public Vector2 cameraPosition;
        public Vector2 worldPosition;
        public Rectangle rectangle;
        public Rectangle headBounds;
        public Rectangle footBounds;
        public Rectangle leftRec;
        public Rectangle rightRec;
        KeyboardState keystate;
        KeyboardState oldkeys;
        public string facing;
        public string state;
        public bool jump;
        public float jumpSpeed;
        public float gravity;
        public float startY;
        public float speed;
        float time;
        public string prevstate;
        public Point frameSize;
        Point sheetSize;
        Point currentFrame;
        public Rectangle source;
        float interval;

        //player location string
        public string XYlocation;

        //player HUD Health
        public HUD playerHUD;
        public float playerHealth;

        public bool canJump;

        //2D particles Fuckers!!!
        Texture2D ParticleBase;
        public ParticleSystem particleSystem;

        //root variable
        public bool rooted = false;

        #endregion

        #region Player Methods

        public Player()
        {
            cameraPosition = new Vector2(1,1);
            worldPosition = new Vector2(1,1);
            startY = worldPosition.Y;
            facing = "right";
            prevstate = state;
            state = "stand";
            jump = false;
            gravity = 0f;
            speed = 4f;
            frameSize = new Point(30, 30);
            sheetSize = new Point(6, 7);
            currentFrame = new Point(0, 0);
            time = 0f;
            interval = 100f;
            canJump = false;

            playerHealth = 0f;
        }

        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Images/Player/Player");

            playerHUD = new HUD(Content);

            // 2d particles load content la
            ParticleBase = Content.Load<Texture2D>("Images/Player/Particle");
            particleSystem = new ParticleSystem(new Vector2(4000, 3000));
            particleSystem.AddEmitter(new Vector2(0.021f, 0.05f),
                                        new Vector2(0, -1), new Vector2(0.001f * MathHelper.Pi, 0.001f * -MathHelper.Pi),
                                        new Vector2(0.2f, 0.65f),
                                        new Vector2(31, 70), new Vector2(6, 70),
                                        Color.Yellow, Color.Crimson, Color.OrangeRed, Color.Orange,
                                        new Vector2(30, 30), new Vector2(10, 10), 20, Vector2.Zero, ParticleBase);
        }

        public void Update(GameTime gameTime)
        {
            playerHUD.Update(playerHealth);
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            oldkeys = keystate;
            keystate = Keyboard.GetState();
            cameraPosition.Y += gravity;
            worldPosition.Y += gravity;

            if (!rooted)
            {
                speed = 4f;
            }
            else
            {
                speed = 0f;
            }

            if (keystate.IsKeyDown(Keys.Right) || keystate.IsKeyDown(Keys.D))
            {
                cameraPosition.X += speed;
                worldPosition.X += speed;
                facing = "right";
            }
            if (keystate.IsKeyDown(Keys.Left) || keystate.IsKeyDown(Keys.A))
            {
                cameraPosition.X -= speed;
                worldPosition.X -= speed;
                facing = "left";
            }
            if ( (oldkeys.IsKeyDown(Keys.Left) || oldkeys.IsKeyDown(Keys.A)) && ( !keystate.IsKeyDown(Keys.Left) || !keystate.IsKeyDown(Keys.A) ))
            {
                state = "stand";
            }

            if ( (oldkeys.IsKeyDown(Keys.Right) || oldkeys.IsKeyDown(Keys.D)) && (!keystate.IsKeyDown(Keys.Right) || !keystate.IsKeyDown(Keys.A)))
            {
                state = "stand";
            }

            if (state == "stand")
            {
                if (keystate.IsKeyDown(Keys.Left) || keystate.IsKeyDown(Keys.A))
                {
                    state = "walk";
                    MoveAnimation(gameTime);
                }
                if (keystate.IsKeyDown(Keys.Right) || keystate.IsKeyDown(Keys.D))
                {
                    state = "walk";
                    MoveAnimation(gameTime);
                }
            }

            if (state == "walk")
            {
                gravity = 4.5f;

            }
            if (state == "stand")
            {
                gravity = 4.5f;
                StandAnimation();
            }

            if (jump)
            {
                state = "jump";
                cameraPosition.Y += jumpSpeed;
                worldPosition.Y += jumpSpeed;
                jumpSpeed += 1f;
                if (jumpSpeed == 0)
                {
                    jumpSpeed = 0;
                    jump = false;
                    state = "fall";
                    canJump = false;
                }

            }
            else
            {
                if (worldPosition.Y == startY)
                {
                    if ((((keystate.IsKeyDown(Keys.Space) || keystate.IsKeyDown(Keys.W) || keystate.IsKeyDown(Keys.Up))) && ((state == "walk" || state == "stand")) && canJump && !rooted))//(oldkeys.IsKeyDown(Keys.Space) || oldkeys.IsKeyDown(Keys.W) || oldkeys.IsKeyDown(Keys.Up)) )
                    {
                        jumpSpeed -= 18f;
                        jump = true;
                        gravity = 0f;
                        canJump = false;
                    }
                }
            }

            if (state == "fall")
            {
                gravity = 5f;
                JumpAnimation();
            }
            if (state == "jump")
            {
                JumpAnimation();
            }

            //Particle la cb
            particleSystem.Position = new Vector2(cameraPosition.X, cameraPosition.Y);

            rectangle = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, 20, 25);
            footBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height / 2);
            rightRec = new Rectangle(rectangle.Center.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
            leftRec = new Rectangle(rectangle.Left, rectangle.Y, 1, rectangle.Height);
            headBounds = new Rectangle(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height);

            XYlocation = "CanJump:" + canJump + "\nState: " + state + "\nX: " + cameraPosition.X + "\nY: " + cameraPosition.Y + "\nWorldX: " + worldPosition.X + "\nWorldY: " + worldPosition.Y;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            particleSystem.Draw(spriteBatch, 0.2f, Vector2.Zero);
            if (facing == "right")
            {
                spriteBatch.Draw(texture, cameraPosition, source, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 1.0f, SpriteEffects.None, 0);
            }

            if (facing == "left")
            {   // flip the toon
                spriteBatch.Draw(texture, cameraPosition, source, Color.White, 0f, new Vector2(rectangle.Width / 2, rectangle.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 0);
            }

        }

        #endregion

        #region Moving Methods

        public void MoveAnimation(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            currentFrame.Y = 3;

            frameSize = new Point(33, 33);
            if (time > interval)
            {
                currentFrame.X++;
                if (currentFrame.X > 5)
                {
                    currentFrame.X = 1;
                }
                source = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * 25, frameSize.X, frameSize.Y);
                time = 0f;
            }
        }

        public void JumpAnimation()
        {

            currentFrame.Y = 1;
            currentFrame.X = 1;
            frameSize = new Point(33, 33);
            source = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

        }

        public void FallAnimation()
        {
            currentFrame.Y = 5;
            currentFrame.X = 2;
            frameSize = new Point(33, 33);
            source = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
        }
        public void StandAnimation()
        {
            currentFrame.Y = 0;
            currentFrame.X = 0;
            frameSize = new Point(33, 33);
            source = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * 19, frameSize.X, frameSize.Y);
        }

        public Boolean isHeDead()
        {
            if (playerHealth >= playerHUD.heartFullTexture.Height)
                return true;
            else
                return false;
        }

        #endregion
    }
}
