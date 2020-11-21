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
    class Button
    {
        #region Variables la

        Rectangle rectangle;
        Texture2D currentTex;
        Texture2D image1;
        Texture2D image2;
        Texture2D image3;
        public Vector2 position;
        public bool clicked;

        #endregion

        #region main methods

        public Button(Texture2D tex1, Texture2D tex2, Texture2D tex3, Vector2 pos)
        {
            image1 = tex1;
            image2 = tex2;
            image3 = tex3;
            currentTex = image1;
            position = pos;
            rectangle = new Rectangle((int)position.X, (int)position.Y, tex1.Width, tex1.Height - 30);
            clicked = false;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Rectangle mouseRec = new Rectangle((int)mouse.X, (int)mouse.Y, 5, 5);
            if (mouseRec.Intersects(rectangle)) //if hovers
            {
                currentTex = image2;
                if (mouse.LeftButton == ButtonState.Pressed) //if clicked
                {
                    clicked = true;
                    currentTex = image3;
                }
            }
            else
            {
                currentTex = image1;
            }

        }

        public void Draw(SpriteBatch sp)
        {
            sp.Begin();
            sp.Draw(currentTex, position, Color.White);
            sp.End();
        }

        #endregion
    }
}


