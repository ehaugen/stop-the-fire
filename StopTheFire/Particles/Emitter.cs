﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StopTheFire.Particles
{
    public class Emitter
    {
        public Vector2 RelPosition;             // Position relative to collection.
        public int Budget;                      // Max number of alive particles.
        float NextSpawnIn;                      // This is a random number generated using the SecPerSpawn.
        float SecPassed;                        // Time pased since last spawn.
        public LinkedList<Particle> ActiveParticles { get; set; }   // A list of all the active particles.
        public Texture2D ParticleSprite;        // This is what the particle looks like.
        public Random random;                   // Pointer to a random object passed through constructor.

        public Vector2 SecPerSpawn;
        public Vector2 SpawnDirection;
        public Vector2 SpawnNoiseAngle;
        public Vector2 StartLife;
        public Vector2 StartScale;
        public Vector2 EndScale;
        public Color StartColor1;
        public Color StartColor2;
        public Color EndColor1;
        public Color EndColor2;
        public Vector2 StartSpeed;
        public Vector2 EndSpeed;
        public bool SpawnNew;
        public int? MiscId;
        public bool CanSpread;
        public string Type;
        public bool Kill = false;

        public ParticleSystem Parent;
        
        public Emitter(  Vector2 SecPerSpawn
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
                        ,Random random
                        ,ParticleSystem parent
                        ,bool spawnNew
                        ,bool canSpread
                        ,string type
                        ,int? miscId = null)
        {
            this.SecPerSpawn = SecPerSpawn;
            this.SpawnDirection = SpawnDirection;
            this.SpawnNoiseAngle = SpawnNoiseAngle;
            this.StartLife = StartLife;
            this.StartScale = StartScale;
            this.EndScale = EndScale;
            this.StartColor1 = StartColor1;
            this.StartColor2 = StartColor2;
            this.EndColor1 = EndColor1;
            this.EndColor2 = EndColor2;
            this.StartSpeed = StartSpeed;
            this.EndSpeed = EndSpeed;
            this.Budget = Budget;
            this.RelPosition = RelPosition;
            this.ParticleSprite = ParticleSprite;
            this.random = random;
            this.Parent = parent;
            ActiveParticles = new LinkedList<Particle>();
            this.NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            this.SecPassed = 0.0f;
            this.SpawnNew = spawnNew;
            this.CanSpread = canSpread;
            this.Type = type;
            if (miscId != null) { MiscId = miscId.Value; }
        }

        public void Update(float dt, ref QuadTree qt)
        {
            SecPassed += dt;
            while (SecPassed > NextSpawnIn)
            {
                if (ActiveParticles.Count < Budget && SpawnNew)
                {
                    // Spawn a particle
                    Vector2 StartDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                    StartDirection.Normalize();
                    Vector2 EndDirection = StartDirection * MathLib.LinearInterpolate(EndSpeed.X, EndSpeed.Y, random.NextDouble());
                    StartDirection *= MathLib.LinearInterpolate(StartSpeed.X, StartSpeed.Y, random.NextDouble());

                    Particle particle = new Particle(
                        RelPosition + MathLib.LinearInterpolate(Parent.LastPos, Parent.Position, SecPassed / dt),
                        StartDirection,
                        EndDirection,
                        MathLib.LinearInterpolate(StartLife.X, StartLife.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartScale.X, StartScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(EndScale.X, EndScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartColor1, StartColor2, random.NextDouble()),
                        MathLib.LinearInterpolate(EndColor1, EndColor2, random.NextDouble()),
                        this);
                    
                    ActiveParticles.AddLast(particle);
                    qt.AddParticle(particle);
                    ActiveParticles.Last.Value.Update(SecPassed);
                }
                SecPassed -= NextSpawnIn;
                NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            }

            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                bool isAlive = node.Value.Update(dt);
                if (node.Value.Kill)
                    isAlive = false;
                node = node.Next;
                if (!isAlive)
                {
                    if (node == null)
                    {
                        ActiveParticles.RemoveLast();
                    }
                    else
                    {
                        ActiveParticles.Remove(node.Previous);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                node.Value.Draw(spriteBatch, Scale, Offset);
                node = node.Next;
            }
        }

        public void Clear()
        {
            ActiveParticles.Clear();
            Kill = true;
        }

    }
}