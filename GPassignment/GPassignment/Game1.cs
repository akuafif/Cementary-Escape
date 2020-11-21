using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //state of the game
        enum GameState
        {
            StartingSplashScreen,
            StartMenu,
            CreditScreen,
            ControlsScreen,
            Playing,
            PlayerWin,
            LevelSplashScreen,
            Pause,
            PlayerDied,
            OutOfTime
        };

        GameState CurrentState = GameState.StartingSplashScreen;

        #region Variables
        protected GraphicsDeviceManager graphics;
        protected GraphicsDevice graphicsDevice;
        protected SpriteBatch spriteBatch;

        //fog of war
        Texture2D gradient;
        RenderTarget2D fogTarget;
        bool fogIncreaseSize = false;
        float fogTime = 0f, fogInterval = 0f, fogScale;
        float fogOfWarPowerUpDuration = 0f;

        //screens
        SplashScreen storyScreen;
        HelpScreen helpScreen;
        StartMenuScreen startScreen;                        /// Start Screen Declaration
        PauseScreen pauseScreen;                            /// Pause Screen
                                                            ///         
        World map;                                          /// World Map object
        int numberOfMaps, currentMap;                                                   ///
       
        bool lastMap= false, runOnce = true;
        bool godMode = false;
        bool fog_cheat = false; 
        bool fogOn = true;
        bool splashScreenForLevelExist = true;
        ///

        //font
        private SpriteFont XYFont, scoreFont;

        //for getting the total score
        int currentTotalScore, oldTotalScore;

        //sound effects
        bool dontLoadMusicAgain =false;

        SoundEffect backgroundMusic;
        SoundEffectInstance bgMusicInstance;

        SoundEffect deathMusic;
        SoundEffectInstance deathMusicInstance;

        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;

        //zombie wsound
        public SoundEffect zombieSound;
        public SoundEffectInstance zombieSoundInstance;

        KeyboardState keystate;
        KeyboardState oldkeys;

        float interval;
        string onScreenText;
        bool enableXYPosition;
        
        float playerRootedInterval = 0f;
        #endregion

        #region Main Methods

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            /// This is ours
            ///

            //Change Window Title
            Window.Title = "Cemetery Escape";

            //Resolution Setting
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 650;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;
            startScreen = new StartMenuScreen();   //declaration
            pauseScreen = new PauseScreen();
            storyScreen = new SplashScreen();
            helpScreen = new HelpScreen();
            Global.highscore = new HighScore();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            CurrentState = GameState.StartingSplashScreen;
            this.IsMouseVisible = true;
            onScreenText = ""; 
            enableXYPosition = false;
         
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            // TODO: use this.Content to load your game content here
            startScreen.LoadContent(Content);
            pauseScreen.LoadContent(Content);
            storyScreen.LoadContent(Content);
            helpScreen.LoadContent(Content);

            Global.player = new Player();
            Global.player.LoadContent(Content);

            // get number of map exist in the directory
            numberOfMaps = getNumberOfMapFiles();
            currentMap = 1;
            
            //initialise score variables
            Global.highscore.LoadContent(numberOfMaps);
            currentTotalScore = 0;
            oldTotalScore = Global.highscore.getTotalScore();

            //loading of sound effects
            if (!dontLoadMusicAgain)
            {
                graphicsDevice = this.GraphicsDevice;
                backgroundMusic = Content.Load<SoundEffect>("Sounds/BackgroundMusic");
                bgMusicInstance = backgroundMusic.CreateInstance();
                bgMusicInstance.IsLooped = true;

                deathMusic = Content.Load<SoundEffect>("Sounds/DeathScreams");
                deathMusicInstance = deathMusic.CreateInstance();
                deathMusicInstance.IsLooped = false;

                jumpSound = Content.Load<SoundEffect>("Sounds/Jump");
                jumpSoundInstance = jumpSound.CreateInstance();
                jumpSoundInstance.IsLooped = false;

                zombieSound = Content.Load<SoundEffect>("Sounds/Zombie");
                zombieSoundInstance = zombieSound.CreateInstance();

                dontLoadMusicAgain = true;                
            }
        }

        #region function to load the levels.
        //load level  files
        protected void LoadLevel()
        {
            //load fog
            fogTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            gradient = Content.Load<Texture2D>("Images/WorldMap/gradient");
            fogScale = 1.5f;

            Global.player = new Player();
            Global.player.LoadContent(Content);

            map = new World(currentMap);
            map.LoadContent(Content);

            //update totalscore. So you can change the score at any point of time before any map loads
            oldTotalScore = Global.highscore.getTotalScore();

            XYFont = Content.Load<SpriteFont>("FontForXY");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
        }

        public void loadCreditScreen()
        {
            Global.player = new Player();
            Global.player.LoadContent(Content);

            map = new World(999);
            map.LoadContent(Content);

            //update totalscore. So you can change the score at any point of time before any map loads
            oldTotalScore = Global.highscore.getTotalScore();

            godMode = true;

            XYFont = Content.Load<SpriteFont>("FontForXY");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
        }
        #endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                // TODO: Add your update logic here
                playerRootedInterval += (float)gameTime.ElapsedGameTime.TotalSeconds;
                fogTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                interval += (float)gameTime.ElapsedGameTime.TotalSeconds;
                fogOfWarPowerUpDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //update keystates
                oldkeys = keystate;
                keystate = Keyboard.GetState();

                //save the hassle, just exit once this is detected
                if (startScreen.Exit.clicked || pauseScreen.quitGame.clicked)
                {
                    this.Exit();
                }

                #region play background music forever
               
                playSounds();

                //zombie sound
                if (zombieSoundInstance.State != SoundState.Playing)
                    zombieSoundInstance.Volume = 0f;

                #endregion 

                #region On screen text for debuggin. activate this by pressing ctrl-X

                if (interval > 2f)
                    onScreenText = "";
                if ((keystate.IsKeyDown(Keys.LeftControl) && keystate.IsKeyDown(Keys.X)) && !(oldkeys.IsKeyDown(Keys.LeftControl) && oldkeys.IsKeyDown(Keys.X)))
                {
                    if (enableXYPosition)
                        enableXYPosition = false;
                    else
                        enableXYPosition = true;
                }

                #endregion 

                // check if the player is at the last map.
                if (currentMap == numberOfMaps)
                    lastMap = true;

                #region switchcase
                switch (CurrentState)
                {
                    #region GameState.StartingSplashScreen -> Starting SplashScreen
                    case GameState.StartingSplashScreen:
                        if (storyScreen.Update(gameTime))
                            CurrentState = GameState.StartMenu;
                        interval = 0f;
                        break;
                    #endregion
                    #region GameState.StartMenu -> Start Menu Updates
                    case GameState.StartMenu:
                        startScreen.Update();

                        //if control button is click
                        if (startScreen.Controls.clicked)
                        {
                            startScreen.Controls.clicked = false;
                            CurrentState = GameState.ControlsScreen;
                        }

                        //if play button is clicked or spacebar button
                        if (((startScreen.Play.clicked) || (keystate.IsKeyDown(Keys.Space) && interval > 0.4f)))
                        {
                            startScreen.Play.clicked = true;
                            this.IsMouseVisible = false;
                            LoadLevel();
                            interval = 0f;
                            CurrentState = GameState.Playing;
                        }

                        //credit screen
                        if (startScreen.Credits.clicked)
                        {
                            startScreen.Credits.clicked = false;
                            this.IsMouseVisible = false;
                            interval = 0f;
                            loadCreditScreen();
                            currentMap = 999;
                            CurrentState = GameState.CreditScreen;
                        }
                        break;
                    #endregion                    
                    #region GameState.ControlsScreen -> Control Screen Methods
                    case GameState.ControlsScreen:
                        //check if user exit controls screen
                        if (helpScreen.buto)
                        {
                            helpScreen = new HelpScreen();
                            helpScreen.LoadContent(Content);
                            CurrentState = GameState.StartMenu;
                        }
                        else
                            helpScreen.Update();
                        break;
                    #endregion
                    #region GameState.CreditScreen -> run the credit screen
                    case GameState.CreditScreen:
                    //same as playing
                    #endregion
                    #region GameState.Playing -> This contain all the updates once it starts playing
                    case GameState.Playing:
                        //if game has started, press esc to pause
                        if (keystate.IsKeyDown(Keys.Escape) && !(oldkeys.IsKeyDown(Keys.Escape)))
                        {
                            CurrentState = GameState.Pause;
                        }

                        //check for player health reaches zero, he die
                        if (Global.player.isHeDead() && CurrentState != GameState.CreditScreen)
                        {
                            interval = 0;
                            CurrentState = GameState.PlayerDied;
                        }

                        /// if score reaches 0
                        if (Global.highscore.getCurrentScore() < 0 && CurrentState != GameState.CreditScreen)
                        {
                            CurrentState = GameState.OutOfTime;
                        }

                        if (Global.player.rooted = true && playerRootedInterval > 3f)
                        {
                            Global.player.rooted = false;
                        }

                        this.IsMouseVisible = false;
                        map.Update(gameTime, Content);
                        Camera(gameTime);
                        Collision();
                        Global.player.Update(gameTime);
                        ZombieCollision();
                        PlayerCollisionWithZombie();
                        chaserCollidePlayer();

                        #region Power Ups
                        if (fogOfWarPowerUpDuration > 5f && !fog_cheat)
                        {
                            fogOn = true;
                        }

                        #endregion

                        #region GodMode, FogOfWar and SkipLevel Cheats
                        //turn off Fog Of war
                        if ((keystate.IsKeyDown(Keys.F) && keystate.IsKeyDown(Keys.LeftControl)) && !(oldkeys.IsKeyDown(Keys.F) && oldkeys.IsKeyDown(Keys.LeftControl)))
                        {
                            if (fogOn)
                            {
                                fogOn = false;
                                fog_cheat = true;
                            }
                            else
                            {
                                fogOn = true;
                                fog_cheat = false;
                            }
                        }

                        //godmode, unlimited health
                        if ((keystate.IsKeyDown(Keys.G) && keystate.IsKeyDown(Keys.LeftControl)) && !(oldkeys.IsKeyDown(Keys.G) && oldkeys.IsKeyDown(Keys.LeftControl)))
                        {
                            if (godMode)
                                godMode = false;
                            else
                                godMode = true;
                        }

                        //if godMode, player health will never move
                        if (godMode)
                            Global.player.playerHealth = 0;

                        //bypass the current level by pressing ctrl-C
                        if ((keystate.IsKeyDown(Keys.LeftControl) && keystate.IsKeyDown(Keys.C)) && !(oldkeys.IsKeyDown(Keys.LeftControl) && oldkeys.IsKeyDown(Keys.C)))
                        {
                            if (currentMap < numberOfMaps)
                            {
                                ++currentMap;
                                enableXYPosition = false;
                                LoadLevel();
                            }
                            else
                            {
                                //onScreenText = "NO MORE MAPS TO LOAD!";
                                interval = 0;
                            }
                        }

                        //fog update scale
                        if (fogTime > fogInterval)
                        {
                            if (fogIncreaseSize)
                            {
                                if (fogScale > 3.5f)
                                    fogIncreaseSize = false;
                                fogScale += 0.01f;
                            }

                            if (!fogIncreaseSize)
                            {
                                if (fogScale < 1.5f)
                                    fogIncreaseSize = true;
                                fogScale -= 0.01f;
                            }

                            fogTime = 0f;
                        }
                        #endregion 
                        break;
                    #endregion
                    #region GameState.Pause -> pause update
                    case GameState.Pause:
                        this.IsMouseVisible = true;
                        pauseScreen.Update();

                        //resume validation
                        if (pauseScreen.resume.clicked ||
                            (((keystate.IsKeyDown(Keys.Escape) ||
                            keystate.IsKeyDown(Keys.Enter)) && interval > 0.3f) &&
                            ( !(oldkeys.IsKeyDown(Keys.Escape) ||
                            oldkeys.IsKeyDown(Keys.Enter)) ) ) )
                        {
                            pauseScreen = new PauseScreen();
                            pauseScreen.LoadContent(Content);
                            CurrentState = GameState.Playing;
                        }

                        //exit level
                        if (pauseScreen.exitLevel.clicked)
                        {
                            pauseScreen = new PauseScreen();
                            pauseScreen.LoadContent(Content);
                            storyScreen.buto = false;

                            if(currentMap == 999)
                            {
                                map.player.Stop();
                            }

                            this.LoadContent();
                        }
                        break;
                    #endregion
                    #region GameState.PlayerWin -> Player win, reach the goal
                    case GameState.PlayerWin:
                        //this will add up the totalcurrentscore to show on the screen when finish
                        if (runOnce)
                        {
                            Global.highscore.CalculateScore();
                            currentTotalScore += Global.highscore.currentScore;
                            runOnce = false;
                        }

                        // press escape, enter, space to exit screen
                        if (((keystate.IsKeyDown(Keys.Escape) || keystate.IsKeyDown(Keys.Space) ||
                            keystate.IsKeyDown(Keys.Enter)) && interval > 0.3f) &&
                            ( !(oldkeys.IsKeyDown(Keys.Escape) || oldkeys.IsKeyDown(Keys.Space) ||
                            oldkeys.IsKeyDown(Keys.Enter)) ) )
                        {
                            if (lastMap)
                            {
                                currentMap = 999;
                                loadCreditScreen();
                                CurrentState = GameState.CreditScreen;
                            }
                            else
                                CurrentState = GameState.LevelSplashScreen;
                        }
                        break;
                    #endregion
                    #region GameState.LevelSplashScreen -> show story screen
                    case GameState.LevelSplashScreen:
                    // press escape, enter, space to exit screen
                    if ( (((keystate.IsKeyDown(Keys.Escape) || keystate.IsKeyDown(Keys.Space) ||
                        keystate.IsKeyDown(Keys.Enter)) && interval > 0.3f) &&
                        ( !(oldkeys.IsKeyDown(Keys.Escape) || oldkeys.IsKeyDown(Keys.Space) ||
                        oldkeys.IsKeyDown(Keys.Enter)) ) ) || !splashScreenForLevelExist)
                    {
                        runOnce = true;

                        interval = 0f;

                        if (!lastMap) //if not lastmap
                        {
                            ++currentMap;
                            enableXYPosition = false;
                            LoadLevel();
                            CurrentState = GameState.Playing;
                        }
                        else
                        {
                            //comparing old totalScore to current regradless of level
                            if (currentTotalScore > oldTotalScore)
                                Global.highscore.addTotalScore(currentTotalScore);

                            currentMap = 1;
                            pauseScreen = new PauseScreen();
                            pauseScreen.LoadContent(Content);
                            storyScreen.buto = false;
                            lastMap = false;
                            this.LoadLevel();
                        }
                    }
                        break;
                    #endregion
                    #region GameState.PlayerDied -> player Died from zombie, chaser or falling into blackhole
                case GameState.PlayerDied:
                        ///Combined with OutOfTime case
                    #endregion
                    #region GameState.OutOfTime -> out of time!!
                    case GameState.OutOfTime:
                         //Zombie eat you, you die!
                        //Exit to main menu
                       // press escape, enter, space to exit screen
                        if (((keystate.IsKeyDown(Keys.Escape) || keystate.IsKeyDown(Keys.Enter)) && interval > 0.3f) &&
                            ( !(oldkeys.IsKeyDown(Keys.Escape) || oldkeys.IsKeyDown(Keys.Enter)) ) )
                        {
                            pauseScreen = new PauseScreen();
                            pauseScreen.LoadContent(Content);
                            storyScreen.buto = false;
                            this.LoadContent();
                        }

                        //restart level, press R
                        if (keystate.IsKeyDown(Keys.R) && !oldkeys.IsKeyDown(Keys.R))
                        {
                            startScreen.Play.clicked = true;
                            this.IsMouseVisible = false;
                            LoadLevel();
                            interval = 0f;
                            CurrentState = GameState.Playing;
                        }
                        break;
                    #endregion
                }
                #endregion
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            /// start screen
            ///
            switch (CurrentState)
            {
                #region GameState.StartingSplashScreen
                case GameState.StartingSplashScreen:
                    storyScreen.Draw(spriteBatch);
                    break;
                #endregion
                #region GameState.StartMenu
                case GameState.StartMenu:
                    startScreen.Draw(spriteBatch);
                    break;
                #endregion
                #region GameState.ControlsScreen
                case GameState.ControlsScreen:
                    helpScreen.Draw(spriteBatch);
                    break;
                #endregion
                #region GameState.CreditScreen
                case GameState.CreditScreen:
                //draw the same as playing state
                #endregion
                #region GameState.Playing
                case GameState.Playing:
                     this.IsMouseVisible = false;
            DrawTheMap:
                    //set up fog of war first la 
                    if(fogOn && currentMap != 999)
                        fogOfWarBefore(spriteBatch);

                    spriteBatch.Begin();
                    map.Draw(spriteBatch);
                    Global.player.Draw(spriteBatch);

                    // some text for me to see
                    spriteBatch.DrawString(XYFont, onScreenText, new Vector2(450, 400), Color.Yellow);

                    spriteBatch.End();

                    //time to blend them
                    if (fogOn && currentMap != 999)
                        fogOfWarAfter(spriteBatch);

                    spriteBatch.Begin();
                    if(currentMap != 999)
                        Global.player.playerHUD.Draw(spriteBatch);

                    if (enableXYPosition)
                    {
                        spriteBatch.DrawString(XYFont,
                                                "rooting Y : " + map.rootLocationList[0].rootRectangle.Y
                                                + "\nrootHeight: " + map.rootLocationList[0].rootTexture.Height
                                                + "\nRootInterval: " + map.rootLocationList[0].popUpInterval
                                                + "\nzombietotal in List: " + map.zombieList.Count()
                                                + "\n\nPlayer\n" + map.chaser.ZombieLocation+ "\n" +
                                                Global.player.XYlocation, new Vector2(650, 200), Color.Yellow);
                    }
                    spriteBatch.End();

                    if (CurrentState == GameState.PlayerWin)
                        goto PlayerWin;
                    if (CurrentState == GameState.PlayerDied)
                        goto PlayerDied;
                    if (CurrentState == GameState.OutOfTime)
                        goto OutOfTime;
                    if (CurrentState == GameState.LevelSplashScreen)
                        goto Hell;
                    break;
                #endregion
                #region GameState.Pause
                case GameState.Pause:
                    this.IsMouseVisible = true;
                    pauseScreen.Draw(spriteBatch);
                    break;
                #endregion
                #region GameState.PlayerWin
                case GameState.PlayerWin:
                    goto DrawTheMap;
                PlayerWin:
                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>("Images/WorldMap/YouWin"), new Rectangle(0, 0, 1000, 650), Color.White);
                    spriteBatch.DrawString(scoreFont, "TOTAL SCORE", new Vector2(300, 430), Color.White);
                    spriteBatch.DrawString(scoreFont, oldTotalScore.ToString(), new Vector2(510, 430), Color.White);

                    spriteBatch.DrawString(scoreFont, "YOUR TOTAL \nSCORE", new Vector2(300, 470), Color.White);
                    spriteBatch.DrawString(scoreFont, currentTotalScore.ToString(), new Vector2(510, 470), Color.White);

                    if (lastMap && currentTotalScore > oldTotalScore)
                        spriteBatch.DrawString(scoreFont, "NEW HIGH SCORE!!", new Vector2(380, 120), Color.White);

                    spriteBatch.End();
                    break;
                #endregion
                #region GameState.LevelSplashScreen
                case GameState.LevelSplashScreen:
                    goto DrawTheMap;
                Hell:
                    try
                    {
                        if (!lastMap)
                        {
                            spriteBatch.Begin();
                            spriteBatch.Draw(Content.Load<Texture2D>("Images/SplashScreens/" + currentMap), new Rectangle(0, 0, 1000, 650), Color.White);
                            spriteBatch.End();
                            splashScreenForLevelExist = true;
                        }
                    }
                    catch (ContentLoadException) { splashScreenForLevelExist = false; } //if no splash screen, exit
                    break;
                #endregion
                #region GameState.PlayerDied
                case GameState.PlayerDied:
                    goto DrawTheMap;
            PlayerDied:
                    spriteBatch.Begin();
                    Global.player.Draw(spriteBatch);
                    spriteBatch.Draw(Content.Load<Texture2D>("Images/WorldMap/YoureDead"), new Rectangle(0, 0, 1000, 650), Color.White);
                    spriteBatch.End();
                    break;
                #endregion
                #region GameState.OutOfTime
                case GameState.OutOfTime:
                    goto DrawTheMap;
            OutOfTime:
                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>("Images/WorldMap/outoftime"), new Rectangle(0, 0, 1000, 650), Color.White);
                    spriteBatch.End();
                    break;
                #endregion
            }

            base.Draw(gameTime);
        }

        #region Player Collision and Camera Methods

        public void Camera(GameTime gameTime)
        {
            Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 0f);
            float cameraSpeed = 4f;
            if (map.mapX + Global.player.footBounds.X > Global.player.worldPosition.Y)
            {
                if (Global.player.cameraPosition.X > graphics.GraphicsDevice.Viewport.Width / 2)
                {
                    map.mapX += (int)cameraSpeed;
                    Global.player.cameraPosition.X -= cameraSpeed;
                    map.ZombieCamera(-cameraSpeed);
                    map.chaser.cameraPosition.X -= cameraSpeed;

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -2f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -2f);

                    map.textCamera(-cameraSpeed);

                    //if (map.fallingObject.isAlive)
                    //    map.fallingObject.cameraPosition.X -= 5f;
                }
            }

                if (Global.player.worldPosition.X <= 0)
                {
                    Global.player.worldPosition.X = 0;
                    Global.player.cameraPosition.X = 0;
                }
                if (map.mapX < 0)
                {
                    map.mapX = 0;
                    Global.player.cameraPosition.X -= cameraSpeed;
                    map.ZombieCamera(-cameraSpeed);
                    map.chaser.cameraPosition.X -= cameraSpeed;

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -2f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -2f);

                    map.textCamera(-cameraSpeed);

                    //if (map.fallingObject.isAlive)
                    //    map.fallingObject.cameraPosition.X -= 5f;
                }

                if (Global.player.cameraPosition.X < graphics.GraphicsDevice.Viewport.Width / 2 && Global.player.facing == "left")
                {
                    Global.player.cameraPosition.X += cameraSpeed;
                    map.mapX -= (int)cameraSpeed;
                    map.ZombieCamera(cameraSpeed);
                    map.chaser.cameraPosition.X += cameraSpeed;

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 2f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 2f);

                        map.textCamera(cameraSpeed);

                    //if (map.fallingObject.isAlive)
                    //    map.fallingObject.cameraPosition.X += 5f;
                }

                if (map.mapX > map.mapW - graphics.GraphicsDevice.Viewport.Width)
                {
                    map.mapX = map.mapW - graphics.GraphicsDevice.Viewport.Width;
                    Global.player.cameraPosition.X += cameraSpeed;
                    map.ZombieCamera(cameraSpeed);
                    map.chaser.cameraPosition.X += cameraSpeed;

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 2f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 2f);

                    map.textCamera(2f);
                    //if (map.fallingObject.isAlive)
                    //    map.fallingObject.cameraPosition.X += 5f;
                }

                if (Global.player.cameraPosition.X > graphics.GraphicsDevice.Viewport.Width)
                {
                    Global.player.cameraPosition.X -= (cameraSpeed * 2);
                    Global.player.worldPosition.X -= (cameraSpeed * 2);
                    map.ZombieCamera(-(cameraSpeed * 2));
                    map.chaser.cameraPosition.X -= (cameraSpeed * 2);

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -5f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, -5f);

                    map.textCamera(-cameraSpeed * 2);

                    //if (map.fallingObject.isAlive)
                    //    map.fallingObject.cameraPosition.X -= 10f;
                }

                if (Global.player.worldPosition.Y >= graphics.GraphicsDevice.Viewport.Height)
                {
                    Global.player.worldPosition.Y = 0;
                    Global.player.cameraPosition.Y = 0;

                    map.chaser.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 0f);
                    Global.player.particleSystem.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f, 0f);
                    Global.player.playerHealth += 100f;
                }
        }

        public void Collision()
        {

            //Check for finish location
            if (map.finish.Intersects(Global.player.rectangle))
            {
                interval = 0f;
                CurrentState = GameState.PlayerWin;
            }

            #region BLOCKS Collision.
            //check if player feet touch the block top
            foreach (Rectangle top in map.blocksTop)
            {
                if (top.Intersects(Global.player.footBounds)) //standing
                {
                    Global.player.gravity = 0f;
                    Global.player.state = "stand";
                    Global.player.canJump = true;
                    Global.player.startY = Global.player.worldPosition.Y;
                }
               // else
                   // Global.player.canJump = false;
            }

            //headbutt
            foreach (Rectangle bottom in map.blocksBottom)
            {
                if (bottom.Intersects(Global.player.headBounds)) //jump
                {
                    Global.player.jumpSpeed = -1f;
                }
            }

            foreach (Rectangle left in map.blocksLeft) 
            {
                if (left.Intersects(Global.player.rightRec))
                {
                    if (Global.player.footBounds.Y >= left.Y)
                    {

                        Global.player.worldPosition.X -= 4f;
                        Global.player.cameraPosition.X -= 4f;
                      // map.ZombieCamera(-5f);
                    }
                }
            }

            foreach (Rectangle right in map.blocksRight)
            {
                if (right.Intersects(Global.player.leftRec))
                {
                    if (Global.player.footBounds.Y >= right.Y)
                    {
                        Global.player.worldPosition.X += 4f;
                        Global.player.cameraPosition.X += 4f;
                      //map.ZombieCamera(+5f);
                    }
                }
            }
            #endregion

            #region powerUPs
            foreach (Rectangle fogPU in map.fogPowerUp)
            {
                if (fogPU.Intersects(Global.player.rectangle)) //standing
                {
                    fogOn = false;
                    fogOfWarPowerUpDuration = 0f;
                    map.tileMap[fogPU.Y / 40, fogPU.X / 40] = 0;
                    map.fogPowerUp.Remove(fogPU);
                    break;
                }
            }

            foreach (Rectangle healthPU in map.healthPowerUp)
            {
                if (healthPU.Intersects(Global.player.rectangle)) //standing
                {
                    fogOn = false;
                    Global.player.playerHealth = 0f;
                    map.tileMap[healthPU.Y / 40, healthPU.X / 40] = 0;
                    map.fogPowerUp.Remove(healthPU);
                    break;
                }
            }
            #endregion

            #region rooting collisong
            foreach (Root r in map.rootLocationList)
            {
                if (!r.rooting)
                {
                    if (Global.player.rectangle.Intersects(r.worldPositionRectangle))
                    {
                        Global.player.rooted = true;
                        playerRootedInterval = 0f;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region zombie and chaser collision

        //to check chaser collide with player
        public void chaserCollidePlayer()
        {
            Rectangle checkCollide = map.chaser.rectangle;
            if (checkCollide.Intersects(Global.player.rectangle))
            {
                Global.player.playerHealth += 0.3f;
                if (interval > 4f)
                    Global.player.playerHealth = 150f;
            }
            else
                interval = 0;
        }

        //this method will check if player collide with zombie
        public void PlayerCollisionWithZombie()
        {
            foreach (Zombie z in map.zombieList)
            {
                if (z.rectangle.Intersects(Global.player.rectangle))
                {
                    Global.player.playerHealth += 0.7f;

                    if (zombieSoundInstance.State == SoundState.Playing)
                    {
                        if (zombieSoundInstance.Volume < 0.9f)
                            zombieSoundInstance.Volume += 0.01f;
                    }
                    zombieSoundInstance.Play();

                    if (Global.player.isHeDead())
                    {
                        interval = 0f;
                        CurrentState = GameState.PlayerDied;
                    }
                }
            }
        }

        //check if zombie hit any blocks
        public void ZombieCollision()
        {
            foreach (Zombie z in map.zombieList)
            {
                foreach (Rectangle top in map.blocksTop)
                {
                    if (top.Intersects(z.footBounds)) //standing
                    {
                        z.gravity = 0f;
                        z.state = "stand";
                        z.startY = z.worldPosition.Y;
                    }
                }

                foreach (Rectangle left in map.blocksLeft)
                {
                    if (left.Intersects(z.rightRec))
                    {
                        z.facing = "left";
                       z.cameraPosition.X -= 5f;
                       z.worldPosition.X -= 5f;

                    }
                }

                foreach (Rectangle right in map.blocksRight)
                {
                    if (right.Intersects(z.leftRec))
                    {
                        z.facing = "right";
                       z.cameraPosition.X += 5f;
                       z.worldPosition.X += 5f;
                    }
                }
            }
        }

        #endregion

        public void playSounds()
        {
            if (CurrentState != GameState.StartingSplashScreen && CurrentState != GameState.CreditScreen && currentMap != 999)
            {
                if (bgMusicInstance.State != SoundState.Playing)
                {
                    bgMusicInstance.Volume = .67f;
                    bgMusicInstance.Play();
                }
            }
            else
                bgMusicInstance.Stop();

            if (CurrentState == GameState.Playing)
            {
                if (Global.player.state == "jump")
                {
                    if (jumpSoundInstance.State != SoundState.Playing)
                    {
                        jumpSoundInstance.Volume = .2f;
                        jumpSoundInstance.Play();
                    }
                }
            }

            if (CurrentState == GameState.PlayerDied && interval < 1f)
            {
                if (deathMusicInstance.State != SoundState.Playing)
                {
                    deathMusicInstance.Volume = 0.8f;
                    deathMusicInstance.Play();
                    interval = 10f;
                }

                bgMusicInstance.Stop();
            }
        }

        private int getNumberOfMapFiles()
        {
            int counter = 0;
            while (true)
            {
                if (counter == 0)
                {
                    if (File.Exists("Content/Maps/Maps.txt"))
                    {
                        ++counter;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (File.Exists("Content/Maps/Maps" + (counter + 1) + ".txt"))
                    {
                        ++counter;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return counter;
        }

        #region Safe region to print font

        Rectangle TitleSafeRegion(string outputString, SpriteFont font)
        {
            Vector2 stringDimensions = font.MeasureString(outputString);
            int stringWidth = (int)stringDimensions.X; // string pixel width
            int stringHeight = (int)stringDimensions.Y; // font pixel height

            // some televisions only show 80% of the window
            const float UNSAFEAREA = 0.2f;
            const float MARGIN = UNSAFEAREA / 2.0f;

            // calculate title safe bounds for string
            int top, left, safeWidth, safeHeight;
            top = (int)(Window.ClientBounds.Height * MARGIN);
            left = (int)(Window.ClientBounds.Width * MARGIN);
            safeWidth = (int)((1.0f - UNSAFEAREA) * Window.ClientBounds.Width)
                                    - stringWidth;
            safeHeight = (int)((1.0f - UNSAFEAREA) * Window.ClientBounds.Height)
                                    - stringHeight;
            return new Rectangle(left, top, safeWidth, safeHeight);
        }
        #endregion

        #region Fog Of War

        private void fogOfWarBefore(SpriteBatch spriteBatch)
        {
            //Start by drawing a texture with black background and white where you want "light"
            GraphicsDevice.SetRenderTarget(fogTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            spriteBatch.Draw(gradient, Global.player.cameraPosition, new Rectangle(0,0,256,256), Color.White, 0f, new Vector2(gradient.Width / 2, gradient.Height / 2), fogScale, SpriteEffects.None, 0);
            
            spriteBatch.End();

            //Reset renderTarget
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Red);

            //Draw everything as usual
        }

        private void fogOfWarAfter(SpriteBatch spriteBatch)
        {
            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;

            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null, null, null);
            spriteBatch.Draw(fogTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        #endregion
    }
}
        #endregion