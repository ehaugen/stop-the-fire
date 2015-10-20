using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StopTheFire.Particles
{
    /// <summary>
    /// ParticleSystem modified from ParticleSystem class by Tigran, available via Tigran's Blog http://blog.tigrangasparian.com/2010/10/08/2d-particles-in-xna-part-1-of-3/
    /// </summary>
    public class ParticleSystem
    {
        public List<Emitter> EmitterList { get; set; }
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { LastPos = position; position = value; }
        }
        public Vector2 LastPos;
        Random random;
        String type;

        public ParticleSystem(Vector2 Position, string Type)
        {
            this.Position = Position;
            this.LastPos = Position;
            random = new Random();
            EmitterList = new List<Emitter>();
        }

        public void Update(float dt)
        {
            int emitterIdToKill = -1;

            for (int i = 0; i < EmitterList.Count; i++)
            {
                                
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Update(dt);
                }

                if (EmitterList[i].Kill)
                    emitterIdToKill = i;
            }

            if(emitterIdToKill > -1 && emitterIdToKill < EmitterList.Count)
                EmitterList.Remove(EmitterList[emitterIdToKill]);
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0 && !EmitterList[i].Kill)
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

        public void AddEmitter(  Vector2 SecPerSpawn
                                ,Vector2 SpawnDirection
                                ,Vector2 SpawnNoiseAngle
                                ,Vector2 StartLife
                                ,Vector2 StartScale
                                ,Vector2 EndScale
                                ,Color StartColor1
                                ,Color StartColor2
                                ,Color EndColor1
                                ,Color EndColor2
                                ,Vector2 StartSpeed
                                ,Vector2 EndSpeed
                                ,int Budget
                                ,Vector2 RelPosition
                                ,Texture2D ParticleSprite
                                ,bool spawnNew
                                ,bool canSpread
                                ,int? miscId = null)
        {
            Emitter emitter = new Emitter(SecPerSpawn, SpawnDirection, SpawnNoiseAngle,
                                        StartLife, StartScale, EndScale, StartColor1,
                                        StartColor2, EndColor1, EndColor2, StartSpeed,
                                        EndSpeed, Budget, RelPosition, ParticleSprite, this.random, this, spawnNew, canSpread, type, miscId);
            EmitterList.Add(emitter);
        }
    }
}