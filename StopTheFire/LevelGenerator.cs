using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using StopTheFire.Particles;

namespace StopTheFire
{
    public class LevelGenerator
    {
        Texture2D buildingSwatch;
        Texture2D window;
        Vector2 fireHeight;
        Texture2D fireParticleBase;

        public int Level { get; set; }
        public List<Building> Buildings { get; set; }

        public LevelGenerator(int level, Texture2D buildingSwatch, Texture2D window, ref ParticleSystem fireParticleSystem, Vector2 fireHeight, Texture2D firePaticleBase)
        {
            if (level < 1)
                Level = 1;
            else
                Level = level;

            this.buildingSwatch = buildingSwatch;
            this.window = window;
            this.fireHeight = fireHeight;
            this.fireParticleBase = firePaticleBase;

            GenerateBuildings(ref fireParticleSystem);
        }

        private void GenerateBuildings(ref ParticleSystem fireParticleSystem)
        {
            var buildingPosition = new Vector2(350, 120);
            var building = new Building(buildingSwatch, buildingPosition, 250, 150, new Window(window, 1));

            if (Buildings == null)
                Buildings = new List<Building>();
            Buildings.Add(building);

            //random seed of fires
            var rand = new Random();
            var windowIds = Enumerable.Range(0, building.Windows.Count).OrderBy(x => rand.Next()).ToArray();

            //TODO: adjust numFires by difficulty, level, etc.
            var numFires = 2; //number of starting fires
            for (int i = 0; i < numFires; i++)
            {
                fireParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f)
                                                , new Vector2(0, -1)
                                                , new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi)
                                                , fireHeight
                                                , new Vector2(12, 14)
                                                , new Vector2(6, 7f)
                                                , Color.Orange, Color.Gray
                                                , new Color(Color.Orange, 0)
                                                , new Color(Color.Orange, 0)
                                                , new Vector2(400, 500)
                                                , new Vector2(100, 120)
                                                , 1000
                                                , building.Windows[windowIds[i]] + new Vector2(building.Window.Width / 2, building.Window.Height)
                                                , fireParticleBase
                                                , true
                                                , true
                                                , windowIds[i]);
            }

            //smokeParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f),
            //                            new Vector2(0, -1), new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
            //                            smokeHeight,
            //                            new Vector2(12, 14), new Vector2(6, 7f),
            //                            Color.White, Color.Gray, new Color(Color.Gray, 0), new Color(Color.Black, 0),
            //                            new Vector2(400, 500), new Vector2(100, 120), 1000, Vector2.Zero, smokeParticleBase);
        }
    }
}
