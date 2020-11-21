using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    class onScreenText
    {
        public string text;
        public Vector2 cameraPosition;
        public Vector2 worldPosition;

        public onScreenText(string text, Vector2 cameraPosition)
        {
            this.text = text;
            this.cameraPosition = cameraPosition;
            this.worldPosition = cameraPosition;
        }
    }
}
