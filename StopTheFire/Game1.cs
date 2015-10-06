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
        private enum GameState { Loading, Running, GameOver }


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState gameState;

        //Fonts
        SpriteFont gameOverFont;
        SpriteFont instructionsFont;
        SpriteFont scoreFont;  

        Texture2D background;

        int score;

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
        //bool spawnNewParticle = false;

        Texture2D fireParticleBase;
        ParticleSystem fireParticleSystem;
        Vector2 fireStartPosition;
        Vector2 fireHeight;
        Vector2 fireHeightMax;
        Vector2 fireHeightStart;
        //bool spawnNewFireParticle = true;

        //Texture2D smokeParticleBase;
        //ParticleSystem smokeParticleSystem;
        //Vector2 smokeStartPosition;
        //Vector2 smokeHeight;
        //Vector2 smokeHeightMax;
        //Vector2 smokeHeightStart;

        public static Game1 Instance; 
        public Vector3 MapSize;
        //List<Particle> particles; 
        //KeyboardState prevKeyboardState; 

        List<int> spreadFireWindowIds = new List<int>();

        public Game1() : base()
        {
            gameState = GameState.Loading;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Instance = this;

            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            //IsMouseVisible = true;
            //Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
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

            gameOverFont = Content.Load<SpriteFont>("Fonts/GameOver");
            instructionsFont = Content.Load<SpriteFont>("Fonts/Instructions");
            scoreFont = Content.Load<SpriteFont>("Fonts/Score");

            ResetStartConditions();
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

        private void ResetStartConditions()
        {
            score = 0;

            buildingPosition = new Vector2(350, 120);
            truckPosition = new Vector2(350, 400);
            truckVelocity = new Vector2(100f, 0);

            waterSprayStartPosition = new Vector2(476, 407);
            waterSprayParticleSystem = new ParticleSystem(waterSprayStartPosition, "WATER");
            waterSprayDistance = waterSprayDistanceMin = new Vector2(0.25f, 0.375f);
            waterSprayDistanceMax = new Vector2(0.75f, 1.25f);

            fireStartPosition = new Vector2(0, 0);
            fireParticleSystem = new ParticleSystem(fireStartPosition, "FIRE");
            fireHeight = fireHeightStart = new Vector2(0.1f, 0.15f);
            fireHeightMax = new Vector2(0.25f, 0.375f);

            //smokeStartPosition = new Vector2(365, 205);
            //smokeParticleSystem = new ParticleSystem(smokeStartPosition);
            //smokeHeight = fireHeightStart = new Vector2(0.2f, 0.3f);
            //smokeHeightMax = new Vector2(0.5f, 0.75f);       


            building = new Building(buildingSwatch, buildingPosition, 250, 150, new Window(window, 1));

            waterSprayParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f)
                                                , new Vector2(0, -1)
                                                , new Vector2(0.01f * MathHelper.Pi, 0.01f * -MathHelper.Pi)
                                                , waterSprayDistance
                                                , new Vector2(3, 4)
                                                , new Vector2(1, 1.5f)
                                                , Color.LightBlue
                                                , Color.White
                                                , new Color(Color.LightBlue, 0)
                                                , new Color(Color.LightBlue, 0)
                                                , new Vector2(400, 500)
                                                , new Vector2(100, 120)
                                                , 1000
                                                , Vector2.Zero
                                                , waterSprayParticleBase
                                                , true
                                                , false);

            //random seed of fires
            var rand = new Random();
            var windowIds = Enumerable.Range(0, building.Windows.Count).OrderBy(x => rand.Next()).ToArray();

            //TODO: adjust numFires by difficulty, level, etc.
            var numFires = 5; //number of starting fires
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
            
            gameState = GameState.Running;
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

            if (fireParticleSystem.EmitterList.Count >= building.Windows.Count)
            {
                gameState = GameState.GameOver;

                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.GetPressedKeys().Length > 0)
                    ResetStartConditions();
            }
            else if (fireParticleSystem.EmitterList.Count.Equals(0))
            {
                //Next level
            }

            if (gameState.Equals(GameState.Running))
            {
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

                waterSprayParticleSystem.Position = waterSprayStartPosition;
                waterSprayParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);

                //smokeParticleSystem.Position = smokeStartPosition;
                //smokeParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, true);
            }

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

                    //Grab windowIds for every window already on fire
                    var windowsOnFire = new List<int>();
                    foreach (Emitter e in fireParticleSystem.EmitterList)
                    {
                        windowsOnFire.Add(e.MiscId.Value);
                    }

                    if (isTop == false)
                    {
                        var windowId = emitter.MiscId.Value - 1;
                        if (!windowsOnFire.Contains(windowId) && !spreadFireWindowIds.Contains(windowId))
                        {
                            possibleWindowIds.Add(windowId);
                        }
                    }
                    if (isBottom == false)
                    {
                        var windowId = emitter.MiscId.Value + 1;
                        if (!windowsOnFire.Contains(windowId) && !spreadFireWindowIds.Contains(windowId))
                        {
                            possibleWindowIds.Add(windowId);
                        }
                    }
                    if (isLeft == false)
                    {
                        var windowId = emitter.MiscId.Value - Convert.ToInt32(building.Rows);
                        if (!windowsOnFire.Contains(windowId) && !spreadFireWindowIds.Contains(windowId))
                        {
                            possibleWindowIds.Add(windowId);
                        }
                    }
                    if (isRight == false)
                    {
                        var windowId = emitter.MiscId.Value + Convert.ToInt32(building.Rows);
                        if (!windowsOnFire.Contains(windowId) && !spreadFireWindowIds.Contains(windowId))
                        {
                            possibleWindowIds.Add(windowId);
                        }
                    }

                    //randomly choose one of the possible window ids, if any
                    if (possibleWindowIds.Count > 0)
                    {
                        var rand = new Random();
                        int index = rand.Next(possibleWindowIds.Count);
                        spreadFireWindowIds.Add(possibleWindowIds[index]);
                    }
                    emitter.CanSpread = false;
                }
                else
                {
                    if (emitter.SpreadCounter == null)
                        emitter.SpreadCounter = 0;

                    if (emitter.SpreadCounter.Value > 1000)
                    {
                        emitter.CanSpread = true;
                        emitter.SpreadCounter = null;
                    }
                    else
                    {
                        emitter.SpreadCounter++;
                    }
                }
            }

            foreach (int windowId in spreadFireWindowIds)
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
                                        building.Windows[windowId] + new Vector2(building.Window.Width / 2, building.Window.Height),
                                        fireParticleBase,
                                        true,
                                        true,
                                        windowId);
            }
            spreadFireWindowIds.Clear();

        BreakForEach:

            fireParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.DrawString(scoreFont, "Score: " + score.ToString(), new Vector2(10, 10), Color.White);
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

            if (gameState.Equals(GameState.GameOver))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(gameOverFont, "Game Over!", new Vector2(200, 150), Color.OrangeRed);
                spriteBatch.DrawString(instructionsFont, "Press any key to play again. (Press Esc to quit.)", new Vector2(200, 370), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }

    
}
