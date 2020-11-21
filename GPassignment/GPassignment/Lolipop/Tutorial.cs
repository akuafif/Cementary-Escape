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
    class Tutorial
    {
        #region variables

        public List<onScreenText> textList;
        SpriteFont spriteFont;

        #endregion

        #region Constructor, LoadContent, Update and Draw 

        public Tutorial()
        {
            textList = new List<onScreenText>();
        }

        public void LoadContent(ContentManager Content)
        {
            spriteFont = Content.Load<SpriteFont>("FontForXY");

            textList.Add(new onScreenText("Welcome to Tutorial", new Vector2(20, 520)));
            textList.Add(new onScreenText("Press Spacebar to jump", new Vector2(200,410)));

            textList.Add(new onScreenText("Run away from the Evil Spirit", new Vector2(666, 520)));
            textList.Add(new onScreenText("Go right ->> ->> ->>", new Vector2(1137, 520)));

            textList.Add(new onScreenText("Here", new Vector2(937, 380)));
            textList.Add(new onScreenText("Here", new Vector2(937, 261)));
            textList.Add(new onScreenText("Here", new Vector2(853, 179)));
            textList.Add(new onScreenText("Here", new Vector2(1005, 100)));

            textList.Add(new onScreenText("Do not let the Evil Spirit touch the tombstone", new Vector2(1727, 520)));
            textList.Add(new onScreenText("Follow the arrow above to that lead to the exit", new Vector2(2745, 520)));

            textList.Add(new onScreenText("Exit here ->>", new Vector2(4200, 520)));
            //2745 , 520
            ///90, 420
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (onScreenText OST in textList)
            {
                spriteBatch.DrawString(spriteFont, OST.text, OST.cameraPosition, Color.White);
            }
        }

        public void textCamera(float toAdd)
        {
            foreach (onScreenText OST in textList)
            {
                OST.cameraPosition.X += toAdd;
            }
        }

        #endregion
    }
}
