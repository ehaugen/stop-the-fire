using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StopTheFire.Particles
{
    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle BoundingBox{ get; set; }

        public bool Kill { get; set; }
        
        Vector2 StartDirection;
        Vector2 EndDirection;
        float LifeLeft;
        float StartingLife;
        float ScaleBegin;
        float ScaleEnd;
        Color StartColor;
        Color EndColor;
        public Emitter Parent{get; set;}
        public int ParentId { get; set; }

        float lifePhase;

        public float Radius { get; set; }

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, Emitter parent)
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
            this.Parent = parent;
            if(parent.MiscId != null)
                this.ParentId = parent.MiscId.Value;
            Kill = false;
        }

        public bool Update(float dt)
        {
            LifeLeft -= dt;
            if (LifeLeft <= 0)
                return false;
            lifePhase = LifeLeft / StartingLife;      // 1 means newly created 0 means dead.

            Vector2 prevPos = Position;

            Position += MathLib.LinearInterpolate(EndDirection, StartDirection, lifePhase) * dt;
            
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            float currScale = MathLib.LinearInterpolate(ScaleEnd, ScaleBegin, lifePhase);
            Color currCol = MathLib.LinearInterpolate(EndColor, StartColor, lifePhase);
            //Rectangle sourceRectangle = new Rectangle((int)((Position.X - 0.5f * currScale) * Scale + Offset.X), (int)((Position.Y - 0.5f * currScale) * Scale + Offset.Y), (int)(currScale * Scale), (int)(currScale * Scale));
            //Radius = sourceRectangle.Height / 2;
            //spriteBatch.Draw(Parent.ParticleSprite, sourceRectangle, null, currCol, 0, Vector2.Zero, SpriteEffects.None, 0);
            BoundingBox = new Rectangle((int)((Position.X - 0.5f * currScale) * Scale + Offset.X), (int)((Position.Y - 0.5f * currScale) * Scale + Offset.Y), (int)(currScale * Scale), (int)(currScale * Scale));
            Radius = BoundingBox.Height / 2;
            spriteBatch.Draw(Parent.ParticleSprite, BoundingBox, null, currCol, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}