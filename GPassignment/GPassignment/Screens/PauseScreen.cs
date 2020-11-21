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
    class PauseScreen
    {
        #region Variables

        public Button resume;
        public Button exitLevel;
        public Button quitGame;

        Texture2D bckgrdTexture;
        Rectangle bckgrdRectangle; 

        #endregion

        public void LoadContent(ContentManager Content)
        {
            bckgrdTexture = Content.Load<Texture2D>("Images/Buttons/PauseScreen");

            Texture2D resume1 = Content.Load<Texture2D>("Images/Buttons/Resume");
            Texture2D resume2 = Content.Load<Texture2D>("Images/Buttons/Resume_Hover");
            Texture2D resume3 = Content.Load<Texture2D>("Images/Buttons/Resume");
            Texture2D exitLevel1 = Content.Load<Texture2D>("Images/Buttons/MainMenu");
            Texture2D exitLevel2 = Content.Load<Texture2D>("Images/Buttons/MainMenu_Hover");
            Texture2D exitLevel3 = Content.Load<Texture2D>("Images/Buttons/MainMenu");
            Texture2D quitGame1 = Content.Load<Texture2D>("Images/Buttons/Exit");
            Texture2D quitGame2 = Content.Load<Texture2D>("Images/Buttons/Exit_Hover");
            Texture2D quitGame3 = Content.Load<Texture2D>("Images/Buttons/Exit");

            bckgrdRectangle = new Rectangle(0, 0, 1000, 650);
            resume = new Button(resume1, resume2, resume3, new Vector2(175, 145));
            exitLevel = new Button(exitLevel1, exitLevel2, exitLevel3, new Vector2(175, 275));
            quitGame = new Button(quitGame1, quitGame2, quitGame3 , new Vector2(175, 410));
        }

        public void Update()
        {
            resume.Update();
            exitLevel.Update();
            quitGame.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bckgrdTexture, bckgrdRectangle, Color.White);
            spriteBatch.End();

            resume.Draw(spriteBatch);
            exitLevel.Draw(spriteBatch);
            quitGame.Draw(spriteBatch);
        }
    }
}
