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
    class Root
    {
        #region
        public Texture2D rootTexture;
        public Rectangle rootRectangle;
        public Rectangle worldPositionRectangle;

        //for placement of sprite
        public Vector2 cameraPosition;
        public Vector2 worldPosition;

        public float popUpInterval = 0f;
        public float interval = 0f;
        public bool rooting = false, goDown = false;

        #endregion

        #region Main Method

       public Root(Vector2 worldPos)
       {
           cameraPosition = worldPos;
           worldPosition = worldPos;
       }

        public void LoadContent(ContentManager Content)
        {
            rootTexture = Content.Load<Texture2D>("Images/WorldMap/hand_root");
            rootRectangle = new Rectangle(0, 0, rootTexture.Width, rootTexture.Height);
            worldPositionRectangle = new Rectangle((int)worldPosition.X /* + (rootTexture.Width/2)*/, (int)worldPosition.Y + (rootTexture.Height), rootTexture.Width, rootTexture.Height);
        }

        public void Update(GameTime gameTime)
        {
            interval += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!(rooting))
            {
                // every random interval
                if (interval >= popUpInterval)
                {
                    interval = 0f;
                    popUpInterval = Global.GetRandomNumber(1, 3);
                    rooting = true;
                    goDown = false;
                }
            }
            else
            {
                if (rootRectangle.Y <= 0 && rootRectangle.Y >= -40 && !goDown)
                {
                    if (interval > 0.01125f)
                    {
                        interval = 0f;
                        rootRectangle.Y--;
                    }
                }
                else
                {
                    goDown = true;

                    if (interval > 0.01125f)
                    {
                        interval = 0f;
                        rootRectangle.Y++;
                    }

                    if (rootRectangle.Y >= -1)
                    {
                        goDown = false;
                        rooting = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rootTexture, cameraPosition, rootRectangle, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0);
           // spriteBatch.Draw(rootTexture, rootRectangle, null, Color.White, 0.1f, new Vector2(0, 0), SpriteEffects.None, 0f);
        }

        #endregion
    }
}
