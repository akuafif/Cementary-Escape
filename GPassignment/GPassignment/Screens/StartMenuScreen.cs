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
    class StartMenuScreen
    {
        ///
        /// This is the class for the start menu screen
        ///
        #region Variable

        public Button Play;
        public Button Controls;
        public Button Credits;
        public Button Exit;

        Texture2D bckgrdTexture;
        Rectangle bckgrdRectangle;

        #endregion


        #region Main Methods
      
        ///
        ///Load you content here. 
        ///
        public void LoadContent(ContentManager content) 
        {
            bckgrdTexture = content.Load<Texture2D>("Images/Buttons/PauseScreen");
                    
            Texture2D play1 = content.Load<Texture2D>("Images/Buttons/Play");
            Texture2D play2 = content.Load<Texture2D>("Images/Buttons/Play_Hover");
            Texture2D play3 = content.Load<Texture2D>("Images/Buttons/Play");
            Texture2D exit1 = content.Load<Texture2D>("Images/Buttons/Exit");
            Texture2D exit2 = content.Load<Texture2D>("Images/Buttons/Exit_Hover");
            Texture2D exit3 = content.Load<Texture2D>("Images/Buttons/Exit");
            Texture2D Controls1 = content.Load<Texture2D>("Images/Buttons/Control");
            Texture2D Controls2 = content.Load<Texture2D>("Images/Buttons/Control_Hover");
            Texture2D Controls3 = content.Load<Texture2D>("Images/Buttons/Control");

            Texture2D Credits1 = content.Load<Texture2D>("Images/Buttons/Credits");
            Texture2D Credits2 = content.Load<Texture2D>("Images/Buttons/Credits_Hover");
            Texture2D Credits3 = content.Load<Texture2D>("Images/Buttons/Credits");

            bckgrdRectangle = new Rectangle(0, 0, 1000, 650);
            Play = new Button(play1, play2, play3, new Vector2(175, 145));
            Controls = new Button(Controls1, Controls2, Controls3, new Vector2(175, 265));
            Exit = new Button(exit1, exit2, exit3, new Vector2(175, 390));


            Credits = new Button(Credits1, Credits2, Credits3, new Vector2(888, 0));
        }

        public void Update() 
        {
            Play.Update();
            Controls.Update();
            Exit.Update();
            Credits.Update();
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bckgrdTexture, bckgrdRectangle, Color.White); 
            spriteBatch.End();
            
            Play.Draw(spriteBatch);
            Controls.Draw(spriteBatch);
            Exit.Draw(spriteBatch);
            Credits.Draw(spriteBatch);
        }

        #endregion
    }
}
