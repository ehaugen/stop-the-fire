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

        Texture2D fireParticleBase;
        ParticleSystem fireParticleSystem;
        Vector2 fireStartPosition;
        Vector2 fireHeight;
        Vector2 fireHeightStart;

        public int Level { get; set; }
        public List<Building> Buildings { get; set; }

        public LevelGenerator(int level, Texture2D buildingSwatch, Texture2D window, Texture2D fireParticleBase)
        {
            if (level < 1)
                Level = 1;
            else
                Level = level;

            this.buildingSwatch = buildingSwatch;
            this.window = window;
            this.fireParticleBase = fireParticleBase;

            GenerateBuildings(ref fireParticleSystem);
        }

        private void GenerateBuildings(ref ParticleSystem fireParticleSystem)
        {
            var rand = new Random();

            int minHeight = 18;
            int minWidth = 10;
            int maxHeight = 24;
            int maxWidth = 18;

            int numFires = 2; //number of starting fires

            int numBuildings = Level;

            if(Level > 3)
            {
                minHeight = 24;
                minWidth = 18;
                maxHeight = 28;
                maxWidth = 25;

                numFires = 2 + (Level - 4);

                numBuildings = 3;
            }

            Buildings = new List<Building>();
            for (int i = 0; i < numBuildings; i++)
            {
                var height = rand.Next(minHeight, maxHeight) * 10;
                var width = rand.Next(minWidth, maxWidth) * 10;
                var buildingPosition = new Vector2(350, 370 - height);


                if (i.Equals(1))
                    buildingPosition = new Vector2(350 - width - 10, 370 - height);
                else if (i.Equals(2))
                    buildingPosition = new Vector2(350 + Buildings[0].Width + 10, 370 - height);

                var building = new Building(buildingSwatch, buildingPosition, width, height, new Window(window, 1));



                //random seed of fires            
                var windowIds = Enumerable.Range(0, building.Windows.Count).OrderBy(x => rand.Next()).ToArray();


                if (building.Windows.Count.Equals(numFires))
                    numFires--;

                fireStartPosition = new Vector2(0, 0);
                fireParticleSystem = new ParticleSystem(fireStartPosition, "FIRE");
                building.FireHeight = fireHeightStart = new Vector2(0.1f, 0.15f);
                building.FireHeightMax = new Vector2(0.25f, 0.375f);

                //smokeStartPosition = new Vector2(365, 205);
                //smokeParticleSystem = new ParticleSystem(smokeStartPosition);
                //smokeHeight = fireHeightStart = new Vector2(0.2f, 0.3f);
                //smokeHeightMax = new Vector2(0.5f, 0.75f);  

                for (int j = 0; j < numFires; j++)
                {
                    fireParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f)
                                                    , new Vector2(0, -1)
                                                    , new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi)
                                                    , building.FireHeight
                                                    , new Vector2(12, 14)
                                                    , new Vector2(6, 7f)
                                                    , Color.Orange, Color.Gray
                                                    , new Color(Color.Orange, 0)
                                                    , new Color(Color.Orange, 0)
                                                    , new Vector2(400, 500)
                                                    , new Vector2(100, 120)
                                                    , 1000
                                                    , building.Windows[windowIds[j]] + new Vector2(building.Window.Width / 2, building.Window.Height)
                                                    , fireParticleBase
                                                    , true
                                                    , true
                                                    , windowIds[j]);
                }

                //smokeParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f),
                //                            new Vector2(0, -1), new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                //                            smokeHeight,
                //                            new Vector2(12, 14), new Vector2(6, 7f),
                //                            Color.White, Color.Gray, new Color(Color.Gray, 0), new Color(Color.Black, 0),
                //                            new Vector2(400, 500), new Vector2(100, 120), 1000, Vector2.Zero, smokeParticleBase);

                building.FireParticleSystem = fireParticleSystem;
                Buildings.Add(building);

            }
        }
    }
}
