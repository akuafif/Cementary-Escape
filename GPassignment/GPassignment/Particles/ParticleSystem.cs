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
    class ParticleSystem
    {
        public List<Emitter> EmitterList;
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { LastPos = position; position = value; }
        }
        public Vector2 LastPos;
        Random random;

        public ParticleSystem(Vector2 Position)
        {
            this.Position = Position;
            this.LastPos = Position;
            random = new Random();
            EmitterList = new List<Emitter>();
        }

        public void Update(float dt, float speed)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Update(dt, speed);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, float Scale, Vector2 Offset)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Draw(spriteBatch, Scale, Offset);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Clear();
                }
            }
        }

        public void AddEmitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, Texture2D ParticleSprite)
        {
            Emitter emitter = new Emitter(SecPerSpawn, SpawnDirection, SpawnNoiseAngle,
                                        StartLife, StartScale, EndScale, StartColor1,
                                        StartColor2, EndColor1, EndColor2, StartSpeed,
                                        EndSpeed, Budget, RelPosition, ParticleSprite, this.random, this);
            EmitterList.Add(emitter);
        }
    }
}
