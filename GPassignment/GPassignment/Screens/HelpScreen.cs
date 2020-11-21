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
    class HelpScreen
    {
        //loadconten
        ///
        /// This is the class for the help menu screen
        ///
        #region Variable

        Texture2D bckgrdTexture;
        Rectangle bckgrdRectangle;

        public bool buto = false; //splash ended = false
        #endregion


        #region Main Methods

        ///
        ///Load you content here. 
        ///
        public void LoadContent(ContentManager content)
        {
            bckgrdTexture = content.Load<Texture2D>("Images/Buttons/ControlMenu");

            bckgrdRectangle = new Rectangle(0, 0, 1000, 650);
        }

        public void Update()
        {
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                buto = true; 
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bckgrdTexture, bckgrdRectangle, Color.White);
            spriteBatch.End();
        }
        #endregion
    }
}
