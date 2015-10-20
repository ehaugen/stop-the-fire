#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

using StopTheFire.Particles;
#endregion

namespace StopTheFire
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private enum GameState { Loading, Running, Paused, LevelComplete, GameOver }


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState gameState;

        //Fonts
        SpriteFont gameOverFont;
        SpriteFont instructionsFont;
        SpriteFont attributionFont;
        SpriteFont scoreFont;
        SpriteFont timerFont;
        SpriteFont statusFont;

        //Sounds
        SoundEffect fireSound;
        SoundEffectInstance fireSoundInstance;
        SoundEffect sirenSound;
        SoundEffectInstance sirenSoundInstance;

        Texture2D background;

        int scoreTotal;
        int scoreLevel;
        int scorePrevious;
        int timer;
        int counter;
        int timeThreshold;
        int level;

        //Texture2D building;
        Vector2 buildingPosition;
        Texture2D buildingSwatch;
        Building building;
        Texture2D window;

        Texture2D truck;
        Texture2D siren;
        Vector2 truckPosition;
        Vector2 truckVelocity;
        SpriteSheet sirenSheet;

        Texture2D waterSprayParticleBase;
        ParticleSystem waterSprayParticleSystem;
        Vector2 waterSprayStartPosition;
        Vector2 waterSprayDistance;
        Vector2 waterSprayDistanceMax;
        Vector2 waterSprayDistanceMin;
        //bool spawnNewParticle = false;

        Texture2D fireParticleBase;
        //ParticleSystem fireParticleSystem;
        //Vector2 fireStartPosition;
        //Vector2 fireHeight;
        //Vector2 fireHeightMax;
        //Vector2 fireHeightStart;
        //bool spawnNewFireParticle = true;

        //Texture2D smokeParticleBase;
        //ParticleSystem smokeParticleSystem;
        //Vector2 smokeStartPosition;
        //Vector2 smokeHeight;
        //Vector2 smokeHeightMax;
        //Vector2 smokeHeightStart;

        LevelGenerator levelGenerator;

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
            siren = Content.Load<Texture2D>("Sprites/Siren2");
            waterSprayParticleBase = Content.Load<Texture2D>("Sprites/waterspray");
            fireParticleBase = Content.Load<Texture2D>("Sprites/fire");
            //smokeParticleBase = Content.Load<Texture2D>("Sprites/fire");

            gameOverFont = Content.Load<SpriteFont>("Fonts/GameOver");
            instructionsFont = Content.Load<SpriteFont>("Fonts/Instructions");
            attributionFont = Content.Load<SpriteFont>("Fonts/Attribution");
            scoreFont = Content.Load<SpriteFont>("Fonts/Score");
            timerFont = Content.Load<SpriteFont>("Fonts/Timer");
            statusFont = Content.Load<SpriteFont>("Fonts/Status");

            fireSound = Content.Load<SoundEffect>("SoundFX/Fire_Burning-JaBa-810606813");
            fireSoundInstance = fireSound.CreateInstance();
            fireSoundInstance.IsLooped = true;

            sirenSound = Content.Load<SoundEffect>("SoundFX/Fire Trucks Sirens 2-SoundBible.com-19361847");
            sirenSoundInstance = sirenSound.CreateInstance();

            sirenSheet = new SpriteSheet(siren, 64, 64, 20, 4);

            ResetGame();
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

        private void ResetGame()
        {
            ResetScore();
            level = 1;
            BuildLevel(level);
        }
   
        private void BuildLevel(int pLevel)
        {
            ResetCounters();
            ResetTruck();
            ResetParticleSystems();

            levelGenerator = new LevelGenerator(pLevel, buildingSwatch, window, fireParticleBase);

            fireSoundInstance.Play();
        }

        private void ResetScore()
        {
            scoreTotal = 0;
        }

        private void ResetCounters()
        {            
            scoreLevel = 0;
            scorePrevious = 0;
            timer = 0;
            counter = 0;
            timeThreshold = 60;
        }

        private void ResetTruck()
        {
            truckPosition = new Vector2(350, 400);
            truckVelocity = new Vector2(100f, 0);
        }

        private void ResetParticleSystems()
        {          

            waterSprayStartPosition = new Vector2(426, 407);
            waterSprayParticleSystem = new ParticleSystem(waterSprayStartPosition, "WATER");
            waterSprayDistance = waterSprayDistanceMin = new Vector2(0.25f, 0.375f);
            waterSprayDistanceMax = new Vector2(0.75f, 1.75f);

            waterSprayParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f)
                                                , new Vector2(0, -1)
                                                , new Vector2(0.01f * MathHelper.Pi, 0.01f * -MathHelper.Pi)
                                                , waterSprayDistance
                                                , new Vector2(3, 4)
                                                , new Vector2(1, 4)
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

            var buildingClearCount = 0;
            foreach (Building building in levelGenerator.Buildings)
            {
                if (building.FireParticleSystem.EmitterList.Count >= building.Windows.Count)
                {
                    gameState = GameState.GameOver;

                    if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                    {
                        ResetGame();
                        gameState = GameState.Running;
                        sirenSoundInstance.Play();
                    }

                    break;
                }                

                if (building.FireParticleSystem.EmitterList.Count.Equals(0))
                {
                    buildingClearCount++;
                }

                if(buildingClearCount.Equals(levelGenerator.Buildings.Count))
                {
                    //calculate score once
                    if (!gameState.Equals(GameState.LevelComplete))
                    {
                        fireSoundInstance.Stop();

                        //waterSprayParticleSystem.Clear();
                        waterSprayParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);

                        scorePrevious = scoreTotal;

                        if (timeThreshold > timer)
                            scoreLevel = timeThreshold - timer; //bonus
                        scoreLevel += 10; //level complete

                        gameState = GameState.LevelComplete;
                    }

                    if (scoreTotal < scorePrevious + scoreLevel)
                        scoreTotal++;
                    else
                        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                        {
                            if (level < 7)
                            {
                                level++;
                                BuildLevel(level);
                            }
                            else
                                ResetGame();

                            gameState = GameState.Running;
                            sirenSoundInstance.Play();
                        }

                    break;
                }
            }

            if(gameState.Equals(GameState.Loading))
            {

                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                {
                    gameState = GameState.Running;
                    sirenSoundInstance.Play();
                }
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

                counter++;
                if (counter.Equals(60))
                {
                    timer++;
                    counter = 0;
                }
            }


            foreach (Building building in levelGenerator.Buildings)
            {
                foreach (Emitter emitter in building.FireParticleSystem.EmitterList)
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

                    if (emitter.StartLife.X < building.FireHeightMax.X)
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
                        foreach (Emitter e in building.FireParticleSystem.EmitterList)
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
                    building.FireParticleSystem.AddEmitter(new Vector2(0.001f, 0.0015f),
                                            new Vector2(0, -1),
                                            new Vector2(0.1f * MathHelper.Pi, 0.1f * -MathHelper.Pi),
                                            building.FireHeight,
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

                building.FireParticleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
                sirenSheet.Update();
            }

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
            spriteBatch.DrawString(scoreFont, "Score: " + scoreTotal.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(timerFont, "Time: " + timer.ToString(), new Vector2(400, 10), Color.White);
            spriteBatch.End();

            if(gameState.Equals(GameState.Loading))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(instructionsFont, "Press Enter to start. (Press Esc to quit.)", new Vector2(200, 370), Color.White);
                spriteBatch.DrawString(attributionFont, "Fire burning sound effect by Jaba http://soundbible.com/1902-Fire-Burning.html", new Vector2(50, 420), Color.White);
                spriteBatch.DrawString(attributionFont, "Siren sound effect by FiremanSam http://soundbible.com/1494-Fire-Trucks-Sirens-2.html", new Vector2(50, 435), Color.White);
                spriteBatch.DrawString(attributionFont, "All sound effects available under Creative Comons Attribution 3.0 License https://creativecommons.org/licenses/by/3.0/us/", new Vector2(50, 450), Color.White);
                spriteBatch.End();
            }

            if (gameState.Equals(GameState.Running))
            {
                foreach (Building building in levelGenerator.Buildings)
                {
                    building.Draw(spriteBatch);

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    //smokeParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
                    building.FireParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
                    spriteBatch.End();
                }

                sirenSheet.Draw(spriteBatch, truckPosition + new Vector2(12, 35));

                spriteBatch.Begin();
                spriteBatch.Draw(truck, truckPosition, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                waterSprayParticleSystem.Draw(spriteBatch, 1, Vector2.Zero);
                spriteBatch.End();
            }

            if (gameState.Equals(GameState.GameOver))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(gameOverFont, "Game Over!", new Vector2(200, 150), Color.OrangeRed);
                spriteBatch.DrawString(instructionsFont, "Press Enter to play again. (Press Esc to quit.)", new Vector2(200, 370), Color.White);
                spriteBatch.End();
            }

            if(gameState.Equals(GameState.LevelComplete))
            {
                
                spriteBatch.Begin();

                if (level < 7)
                {
                    spriteBatch.DrawString(statusFont, "Level Complete!", new Vector2(250, 150), Color.Yellow);
                    if (scoreTotal.Equals(scorePrevious + scoreLevel))
                        spriteBatch.DrawString(instructionsFont, "Press Enter to start level " + (level + 1) + ". (Press Esc to quit.)", new Vector2(200, 370), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(statusFont, "You won!", new Vector2(300, 150), Color.Yellow);
                    if (scoreTotal.Equals(scorePrevious + scoreLevel))
                        spriteBatch.DrawString(instructionsFont, "Press Enter to play again. (Press Esc to quit.)", new Vector2(200, 370), Color.White);

                }
                
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }

    
}
