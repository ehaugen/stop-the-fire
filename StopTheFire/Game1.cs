#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using StopTheFire.Particles;
#endregion

namespace StopTheFire
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;

        //Texture2D building;
        Vector2 buildingPosition;
        Texture2D buildingSwatch;
        Building building;
        Texture2D window;

        Texture2D truck;
        Vector2 truckPosition;
        Vector2 truckVelocity;

        Texture2D waterSprayParticleBase;
        ParticleSystem waterSprayParticleSystem;
        Vector2 waterSprayStartPosition;
        Vector2 waterSprayDistance;
        Vector2 waterSprayDistanceMax;
        Vector2 waterSprayDistanceMin;
        bool spawnNewParticle = false;

        Texture2D fireParticleBase;
        ParticleSystem fireParticleSystem;
        Vector2 fireStartPosition;
        Vector2 fireHeight;
        Vector2 fireHeightMax;
        Vector2 fireHeightStart;
        bool spawnNewFireParticle = true;

        //Texture2D smokeParticleBase;
        //ParticleSystem smokeParticleSystem;
        //Vector2 smokeStartPosition;
        //Vector2 smokeHeight;
        //Vector2 smokeHeightMax;
        //Vector2 smokeHeightStart;


        public static Game1 Instance; 
        public Vector3 MapSize; 
        public QuadTree RootQuadTree; 
        List<Particle> particles; 
        Random rand; 
        int cycle = 0;
        //float maxActorSpeed = 100f; 
        //int actorSpriteSize = 8; 
        //FrameRateCounter fps; 
        //int collisionCount; 
        //bool isQuadTreeCollisionDetectionEnabled = false; 
        //KeyboardState prevKeyboardState; 


        public Game1() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Instance = this;

            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            //IsMouseVisible = true;
            //Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            rand = new Random(); 
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            MapSize = new Vector3(viewport.Width, viewport.Height, 0);

            particles = new List<Particle>();

            RootQuadTree = new QuadTree(Vector3.Zero, MapSize);
            //fps = new FrameRateCounter(); 

            buildingPosition = new Vector2(350, 120);                  
            truckPosition = new Vector2(400, 400);
            truckVelocity = new Vector2(100f, 0);

            waterSprayStartPosition = new Vector2(476, 407);            
            waterSprayParticleSystem = new ParticleSystem(waterSprayStartPosition, "WATER");
            waterSprayDistance = waterSprayDistanceMin = new Vector2(0.25f, 0.375f);
            waterSprayDistanceMax = new Vector2(0.75f, 1.25f);

            fireStartPosition = new Vector2(0,0);
            fireParticleSystem = new ParticleSystem(fireStartPosition, "FIRE");
            fireHeight = fireHeightStart = new Vector2(0.1f, 0.15f);
            fireHeightMax = new Vector2(0.25f, 0.375f);

            //smokeStartPosition = new Vector2(365, 205);
            //smokeParticleSystem = new ParticleSystem(smokeStartPosition);
            //smokeHeight = fireHeightStart = new Vector2(0.2f, 0.3f);
            //smokeHeightMax = new Vector2(0.5f, 0.75f);            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("Sprites/skyline");
            //building = Content.Load<Texture2D>("Sprites/BuildingTall");
            buildingSwatch = Content.Load<Texture2D>("Sprites/BuildingSwatch");
            window = Content.Load<Texture2D>("Sprites/window");
            truck = Content.Load<Texture2D>("Sprites/FireTruck");
            waterSprayParticleBase = Content.Load<Texture2D>("Sprites/waterspray");
            fireParticleBase = Content.Load<Texture2D>("Sprites/fire");
            //smokeParticleBase = Content.Load<Texture2D>("Sprites/fire");

            building = new Building(buildingSwatch, buildingPosition, 250, 150, new Window(window, 1));

            waterSprayParticleSystem.AddEmitter( new Vector2(0.001f, 0.0015f)
                                                ,new Vector2(0, -1)
                                                ,new Vector2(0.01f * MathHelper.Pi, 0.01f * -MathHelper.Pi)
                                                ,waterSprayDistance
                                                ,new Vector2(3, 4)
                                                ,new Vector2(1, 1.5f)
                                                ,Color.LightBlue
                                                ,Color.White
                                                ,new Color(Color.LightBlue, 0)
                                                ,new Color(Color.LightBlue, 0)
                                                ,new Vector2(400, 500)
                                                ,new Vector2(100, 120)
                                                ,1000
                                                ,Vector2.Zero
                                                ,waterSprayParticleBase
                                                ,true
                                                ,false);

            //random seed of fires
            var rand = new Random();
            var windowIds = Enumerable.Range(0, building.Windows.Count).OrderBy(x => rand.Next()).ToArray();

            //TODO: adjust numFires by difficulty, level, etc.
            var numFires = 4; //number of starting fires
            for(int i=0; i< numFires; i++)
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

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }       

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //To quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            //Keyboard input to move truck
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (truckPosition.X > 0)
                {
                    truckPosition -= Vector2.Multiply(truckVelocity, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    waterSprayStartPosition -= Vector2.Multiply(truckVelocity, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (truckPosition.X < (graphics.PreferredBackBufferWidth - truck.Width))
                {
                    truckPosition += Vector2.Multiply(truckVelocity, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    waterSprayStartPosition += Vector2.Multiply(truckVelocity, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            //Keyboard input to move water canon
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (waterSprayParticleSystem.EmitterList[0].StartLife.X < waterSprayDistanceMax.X && waterSprayParticleSystem.EmitterList[0].StartLife.Y < waterSprayDistanceMax.Y)
                {
                    waterSprayParticleSystem.EmitterList[0].StartLife.X += 0.005f;
                    waterSprayParticleSystem.EmitterList[0].StartLife.Y += 0.0075f;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (waterSprayParticleSystem.EmitterList[0].StartLife.X > waterSprayDistanceMin.X && waterSprayParticleSystem.EmitterList[0].StartLife.Y > waterSprayDistanceMin.Y)
                {
                    waterSprayParticleSystem.EmitterList[0].StartLife.X -= 0.005f;
                    waterSprayParticleSystem.EmitterList[0].StartLife.Y -= 0.0075f;
                }
            }

            //Keyboard input to spray water canon
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                waterSprayParticleSystem.EmitterList[0].SpawnNew = true;
            }
            else
            {
                waterSprayParticleSystem.EmitterList[0].SpawnNew = false;
            }

            //if(fireHeight.X < .025f)
            //{
            //    spawnNewFireParticle = false;
            //    fireParticleSystem.Clear();
            //}


            waterSprayParticleSystem.Position = waterSprayStartPosition;
            waterSprayParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, ref RootQuadTree);



            //smokeParticleSystem.Position = smokeStartPosition;
            //smokeParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, true);

            var spreadFire = false;
            int spreadWindowId = -1;

            //CollisionDetection();




            foreach (Emitter emitter in fireParticleSystem.EmitterList)
            {
                var windowPoint = building.Windows[emitter.MiscId.Value];
                var windowWidthTenth = building.Window.Width / 10;
                var window = new Rectangle(Convert.ToInt32(windowPoint.X + windowWidthTenth), Convert.ToInt32(windowPoint.Y), Convert.ToInt32(building.Window.Width - (windowWidthTenth * 2)), Convert.ToInt32(building.Window.Height));

                foreach (Particle particle in waterSprayParticleSystem.EmitterList[0].ActiveParticles)
                {
                    if (particle.BoundingBox.Intersects(window))
                    {
                        emitter.StartLife.X -= .00005f;
                        emitter.StartLife.Y -= .00005f;
                        //fireHeight.X -= .000001f;
                        //fireHeight.Y -= .000001f;

                        emitter.CanSpread = true; //if the fire ever goes below max height, then water was sprayed on it and it can spread again

                        particle.Kill = true;

                        if (emitter.StartLife.X < .025f)
                        {
                            emitter.SpawnNew = false;
                            emitter.Clear();
                            goto BreakForEach;
                        }
                    }
                }


                if (emitter.StartLife.X < fireHeightMax.X)
                {
                    emitter.StartLife.X += .0001f;
                    emitter.StartLife.Y += .0001f;
                }
                else if (emitter.Parent.EmitterList.Count >= building.Windows.Count / 2)
                {
                    //end game
                }
                else if (emitter.CanSpread)
                {
                    //spread to a neighboring window if not already on fire
                    float topY = building.Windows[0].Y + building.Window.Height;
                    float bottomY = building.Windows[building.Windows.Count - 1].Y + building.Window.Height;
                    float leftX = building.Windows[0].X + building.Window.Width / 2;
                    float rightX = building.Windows[building.Windows.Count - 1].X + building.Window.Width / 2;

                    bool isTop = false;
                    bool isBottom = false;
                    bool isLeft = false; ;
                    bool isRight = false;

                    if (emitter.RelPosition.X.Equals(leftX)) { isLeft = true; }
                    if (emitter.RelPosition.X.Equals(rightX)) { isRight = true; }
                    if (emitter.RelPosition.Y.Equals(topY)) { isTop = true; }
                    if (emitter.RelPosition.Y.Equals(bottomY)) { isBottom = true; }

                    List<int> possibleWindowIds = new List<int>();

                    //TODO: check to see if fire is already in window; if so, don't add to possibleWindowIds
                    if (isTop == false) { possibleWindowIds.Add(emitter.MiscId.Value - 1); }
                    if (isBottom == false) { possibleWindowIds.Add(emitter.MiscId.Value + 1); }
                    if (isLeft == false) { possibleWindowIds.Add(emitter.MiscId.Value - Convert.ToInt32(building.Rows)); }
                    if (isRight == false) { possibleWindowIds.Add(emitter.MiscId.Value + Convert.ToInt32(building.Rows)); }

                    //randomly choose one of the possible window ids
                    var rand = new Random();
                    int index = rand.Next(possibleWindowIds.Count);
                    spreadWindowId = possibleWindowIds[index];

                    //spread the fire!
                    spreadFire = true;
                    emitter.CanSpread = false;
                }
            }




            if (spreadFire)
            {

                fireParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f),
                                        new Vector2(0, -1),
                                        new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                                        fireHeight,
                                        new Vector2(12, 14), new Vector2(6, 7f),
                                        Color.Orange, Color.Gray,
                                        new Color(Color.Orange, 0),
                                        new Color(Color.Orange, 0),
                                        new Vector2(400, 500),
                                        new Vector2(100, 120),
                                        1000,
                                        building.Windows[spreadWindowId] + new Vector2(building.Window.Width / 2, building.Window.Height),
                                        fireParticleBase,
                                        true,
                                        true,
                                        spreadWindowId);
                spreadFire = false;
            }

        BreakForEach:

            fireParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, ref RootQuadTree);

            base.Update(gameTime);
        }

        //List<QuadTree> leavesInsideFrustum;

        //void CollisionDetection()
        //{
        //    collisionCount = 0;

        //    leavesInsideFrustum = RootQuadTree.GetLeavesInsideFrustrum(new BoundingFrustum(Matrix.Identity));

        //    foreach (QuadTree leaf in leavesInsideFrustum)
        //    {
        //        CollisionDetection(leaf.Particles);
        //    }

        //}

        //void CollisionDetection(List<Particle> particles)
        //{
        //    int count = particles.Count;
        //    Particle particle1, particle2;

        //    for (int i = 0; i < count; i++)
        //    {
        //        particle1 = particles[i];
                
        //        for (int j = i + 1; j < count; j++)
        //        {
        //            particle2 = particles[j];

        //            //TODO: Checking particle ParentId should be sufficient since particles from different fire emitters should never cross--
        //            //if that ever becomes possible, the following will fail, so should be updated to account for that at some point
        //            if (!particle1.ParentId.Equals(particle2.ParentId) && particle1.BoundingBox.Intersects(particle2.BoundingBox))
        //            {
        //                Particle fireParticle, waterParticle;

        //                //determine water particle
        //                if (particle1.Parent.Type.Equals("WATER"))
        //                {
        //                    waterParticle = particle1;
        //                    fireParticle = particle2;
        //                }
        //                else
        //                {
        //                    fireParticle = particle1;
        //                    waterParticle = particle2;
        //                }
                            
        //                //shrink fire
        //                foreach(Emitter emitter in fireParticleSystem.EmitterList)
        //                {
        //                    if (emitter.MiscId.Equals(fireParticle.ParentId))
        //                    {
        //                        emitter.StartLife.X -= .000001f;
        //                        emitter.StartLife.Y -= .000001f;

        //                        if (emitter.StartLife.X < .025f)
        //                        {
        //                            emitter.SpawnNew = false;
        //                            emitter.Clear();
        //                        }

        //                        break;
        //                    }
        //                }

        //                //remove particles
        //                particles.Remove(particle1);
        //                particles.Remove(particle2);

        //                ++collisionCount;
        //            }
        //        }
        //  } 

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            //spriteBatch.Draw(building, buildingPosition, Color.White);
            spriteBatch.End();

            building.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(truck, truckPosition, Color.White);            
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            //smokeParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
            fireParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
            waterSprayParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    
}
