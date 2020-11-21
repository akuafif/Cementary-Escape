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

///<Summary>
///This class will manage the HUD for the player.
///Such as the life.
///</Summary>
namespace Cemetery_Escape
{
    class HUD
    {
        public Texture2D heartEmptyTexture, heartFullTexture, pointerTexture, hudFrameTexture;
        protected Rectangle heartRetangle, pointerRectangle;
        public SpriteFont currentScore, highScore;
        public float seconds;
        public int minutes;
        public string timePassed;

        // pointer location
        float pointerRotation;

        Vector2 Drawlocation;

        public HUD(ContentManager Content)
        {
            Drawlocation = new Vector2(50, 16);
            hudFrameTexture = Content.Load<Texture2D>("Images/WorldMap/HUD");
            heartFullTexture = Content.Load<Texture2D>("Images/Player/Heart");
            heartEmptyTexture = Content.Load<Texture2D>("Images/Player/Heart_Empty");
            pointerTexture = Content.Load<Texture2D>("Images/WorldMap/pointer");
            currentScore = Content.Load<SpriteFont>("FontForXY");
            highScore = Content.Load<SpriteFont>("FontForXY");

            heartRetangle = new Rectangle(0, 0, heartFullTexture.Width, 0);
            pointerRectangle = new Rectangle(0, 0, pointerRectangle.Width, pointerRectangle.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(hudFrameTexture, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(heartFullTexture, Drawlocation, null, Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0);        
           
            spriteBatch.Draw(heartEmptyTexture, Drawlocation, heartRetangle, Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0);
            //Showing the time that has passed
            spriteBatch.DrawString(currentScore, "" + Global.highscore.getCurrentScore(), new Vector2(475, 45), Color.White);
            //Showing the highscore
            spriteBatch.DrawString(highScore, "" + Global.highscore.loadHighscore(), new Vector2(890, 45), Color.White);

            //draw pointer
            spriteBatch.Draw(pointerTexture, new Vector2(Global.player.cameraPosition.X, (Global.player.cameraPosition.Y - 30)), null, Color.White, pointerRotation, new Vector2(pointerTexture.Width / 2, pointerTexture.Height / 2), 0.28f, SpriteEffects.None, 0);        
            //spriteBatch.Draw(pointerTexture, new Vector2(460, 80), null, Color.White, pointerRotation, new Vector2(pointerTexture.Width / 2, pointerTexture.Height / 2), 0.28f, SpriteEffects.None, 0);        
        }

        public void Update(float health)
        {
           if (heartRetangle.Height < heartEmptyTexture.Height)
           {
                   heartRetangle.Height = (int)health; //the less health, the less height.
           }

           if (heartRetangle.Height > heartEmptyTexture.Height)
           {
               heartRetangle.Height = heartEmptyTexture.Height; //die
           }
        }

        public void Update(GameTime gametime)
        {
            seconds += (float)gametime.ElapsedGameTime.TotalSeconds;
            if ((int)seconds == 60)
            {
                ++minutes;
                seconds = 0;
            }
            timePassed = minutes + ":" + (int)seconds;
            if ((int)seconds < 10)
                timePassed = minutes + ":0" + (int)seconds;
        }

        public void UpdatePointer(Vector2 finish)
        {
            Vector2 currentPosition = Global.player.worldPosition;

            Vector2 direction = finish - currentPosition;
            pointerRotation = (float)Math.Atan2(direction.Y, direction.X);
        }


        public int getMin()
        {
            return minutes;
        }

        public int getSec()
        {
            return (int)seconds;
        }

        public bool playerAlive()
        {
            return heartRetangle.Height <= heartEmptyTexture.Height;
        }
    }
}
