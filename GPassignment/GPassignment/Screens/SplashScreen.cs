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
    class SplashScreen
    {
        //loadconten
        ///
        /// This is the class for the story screen
        ///
        #region Variable

        public float time, interval;
        Texture2D storyTex, ngeeAnnTex, titleTex;
        Rectangle bckgrdRectangle;

        public bool buto; //splash end
        int index = 1;

        //video
        Video video;
        VideoPlayer player;
        Texture2D videoTexture;

        #endregion


        #region Main Methods

        ///
        ///Load you content here. 
        ///
        public void LoadContent(ContentManager content)
        {
            storyTex = content.Load<Texture2D>("Images/Buttons/Story");
            ngeeAnnTex = content.Load<Texture2D>("Images/WorldMap/ngeeann");
            titleTex = content.Load<Texture2D>("Images/WorldMap/titlescreen");

            bckgrdRectangle = new Rectangle(0, 0, 1000, 650);

            // Create a new SpriteBatch, which can be used to draw textures.
            video = content.Load<Video>("Images/Story/BackgroundStory");
            player = new VideoPlayer();
        }

        public bool Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            interval += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //no key pressed
            if (interval > 5) //if no keys pressed, change every 5 seconds
            {
                if (index <= 3)//3 screens
                {
                    index++;
                    interval = 0;

                    if (index == 4)
                    {
                        player.IsLooped = false; //setting first before playing video
                        player.Play(video);
                    }
                }
            }

            if (index == 1)
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0 && interval > 0.15f)
                {
                    index = 2;
                    interval = 0;
                }
            }
            if (index == 2)
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0 && interval > 0.15f)
                {
                    index = 3;
                    interval = 0;
                }
            }
            if (index == 3)
            {
                if (Keyboard.GetState().GetPressedKeys().Length > 0 && interval > 0.15f)
                {
                    index = 4;
                    interval = 0;

                    player.IsLooped = false;
                    player.Play(video);
                }
            }
            if (index == 4)
            {
                if ( (Keyboard.GetState().GetPressedKeys().Length > 0 && interval > 2f) || interval > 14.6f)
                {
                    buto = true; //done
                    interval = 0;
                    index = 1; //to reset, so that it returns to one on next play
                    player.Stop();
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (index == 1)
            {
                spriteBatch.Draw(ngeeAnnTex, bckgrdRectangle, Color.White);
            }
            else if (index == 2)
            {
                spriteBatch.Draw(titleTex, bckgrdRectangle, Color.White);
            }
            else if (index == 3)
            {
                spriteBatch.Draw(storyTex, bckgrdRectangle, Color.White);
            }
            else if (index == 4)
            {
                // Only call GetTexture if a video is playing or paused
                if (player.State != MediaState.Stopped)
                    videoTexture = player.GetTexture();

                // Draw the video, if we have a texture to draw.
                if (videoTexture != null)
                {
                    spriteBatch.Draw(videoTexture, bckgrdRectangle, Color.White);
                }
            }
            spriteBatch.End();
        }
        #endregion

    }
}
