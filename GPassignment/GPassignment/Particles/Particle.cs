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
    class Particle
    {
        public Vector2 Position;
        Vector2 StartDirection;
        Vector2 EndDirection;
        float LifeLeft;
        float StartingLife;
        float ScaleBegin;
        float ScaleEnd;
        Color StartColor;
        Color EndColor;
        Emitter Parent;
        float lifePhase;

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, Emitter Yourself)
        {
            this.Position = Position;
            this.StartDirection = StartDirection;
            this.EndDirection = EndDirection;
            this.StartingLife = StartingLife;
            this.LifeLeft = StartingLife;
            this.ScaleBegin = ScaleBegin;
            this.ScaleEnd = ScaleEnd;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.Parent = Yourself;
        }

        public bool Update(float dt, float speed)
        {
            LifeLeft -= dt;
            if (LifeLeft <= 0)
                return false;
            lifePhase = LifeLeft / StartingLife;      // 1 means newly created 0 means dead.
            Position += MathLib.LinearInterpolate(EndDirection, StartDirection, lifePhase) * dt;

            Position.X += speed;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, float Scale, Vector2 Offset)
        {
            float currScale = MathLib.LinearInterpolate(ScaleEnd, ScaleBegin, lifePhase);
            Color currCol = MathLib.LinearInterpolate(EndColor, StartColor, lifePhase);
            spriteBatch.Draw(Parent.ParticleSprite,
                             new Rectangle((int)((Position.X)),
                                           (int)((Position.Y)),
                                           (int)(currScale * Scale),
                                           (int)(currScale * Scale)),
                             new Rectangle(0, 0, Parent.ParticleSprite.Width, Parent.ParticleSprite.Height),
                             currCol,
                             0,
                             new Vector2(Parent.ParticleSprite.Width / 2, Parent.ParticleSprite.Height / 2),
                             SpriteEffects.None,
                             0);
        }
    }
}
