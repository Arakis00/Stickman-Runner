using System;
using System.IO;
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

namespace TheRunner
{
    public class Level
    {
        #region Global Variables
        //Declare All Class Variables

        // Development Version 1.0.0 - 3.0.2
        // Declare Graphics Variables
        // Requirement 4.0.0
        ContentManager Content;
        GraphicsDeviceManager graphics;
        Texture2D countryBackground, countryBackgroundZombie, countryHorizon, countryHorizonZombie, countryTrack, countryTrackZombie, 
            stadiumBackground, stadiumBackgroundZombie, stadiumTrack, moonBackground, moonTrack, moonForeground, stars, distantStars, 
            moonBackgroundZombie, moonHorizon, black;
        int starsX, distantStarsX; //for moon background
        SpriteFont mainFont;

        // Development Version 1.0.0 - 2.0.1
        // Declare Variables for Animation Class Objects and Player
        // Requirements 11.0.0 and 11.2.0
        SpriteBatch spriteBatch;
        Rectangle srcRect, destRect;
        int frames;
        const int RUNNER_X_POSITION = 40;
        private AnimationPlayer sprite;
        Animation stickRun, stickJump, stickCrouch;
        float elapsed, delay = 70f, transitionElapsed;

        // Development Version 1.0.1
        // Declare Variables for Jump, Movement and Level
        // Requirements 1.2.0, 1.3.0, and 3.1.0
        int counter, backgroundX, foregroundX, runnerY, trackX,
            speed, horizonX, level;
        const float JUMP_DELAY = 110f;
        const int JUMP_SPEED = 5, TRACK_HEIGHT = 400;//364
        bool jumping, crouching;

        // Development Version 1.0.1
        // Declare Variables for Gameover and Going Back to Main Menu
        // Requirement 5.9.0
        bool gameOver, mainMenu;

        // Development Version 1.0.1
        // Declare Variables for Score, Strike, Streak and Multiplier
        // Requirements 5.6.0, 5.7.0, 6.0.0 and 6.1.0
        int score, strikes, multiplier, currentStreak;
        const int MULTIPLIER_THRESHOLD = 10, BASE_SCORE = 25, STRIKE_THRESHOLD = 3;
        double distance, totalDistance;
        String strikeString = "";

        // Development Version 1.0.2
        // Declare Variables for Pause Button, Pause Screen and Game Over Screen Functions
        // Requirements 4.2.0, 5.4.0, 5.5.0 and 13.0.0
        bool paused, dontJump;
        Texture2D pauseBttnImg, playBttnImg, pausedTitle, pausedQuit, pausedResume, pausedSelector,
            gameOverTitle, gameOverReplay, completedImg;
        mButton pauseBtn, pausedTitleBtn, pausedQuitBtn, pausedResumeBtn, gameOverReplayBtn, gameOverTitleBtn;

        // Development Version 1.0.4
        // Declare Variables for Obstacle Creation, Circular Array and Array Management Variables
        // Requirements 2.0.0, 10.1.0, 10.1.1, and 10.1.2
        Obstacle[] obstacles = new Obstacle[30];
        Random randHurdle = new Random();
        Texture2D hurdle, overheadBar, tombstoneHurdle, bonesOverheadBar, finishLine;
        int headIndex, tailIndex, hurdleType;
        float obstacleElapsed;

        // Development Version 1.0.4 - 3.0.2
        // Declare Variables for Level Creation
        // Requirement 5.8.0
        const int LEVEL_1_SIZE = 61, LEVEL_2_SIZE = 151, LEVEL_3_SIZE = 301;
        int levelIndex, levelSize;
        float[] delays;
        int[] types;
        float[] level1 = new float[LEVEL_1_SIZE];
        float[] level2 = new float[LEVEL_2_SIZE];
        float[] level3 = new float[LEVEL_3_SIZE];
        int[] level1Type = new int[LEVEL_1_SIZE];
        int[] level2Type = new int[LEVEL_2_SIZE];
        int[] level3Type = new int[LEVEL_3_SIZE];

        // Development Version 2.0.0
        // Declare Navigation Variables
        // Requirement 5.0.0
        KeyboardState oldState, currentKeyboard, oldKeyboard;
        MouseState oldMouseState, currentMouse, oldMouse;

        // Development Version 2.0.3
        // Declare Variables for Music
        // Requirements 7.0.0 and 7.2.0
        Song levelSong;

        // Development Version 2.0.3
        // Declare Variables for Sound Effects
        // Requirement 7.3.0
        SoundEffect jumpSound, hitSound, hitZombieSound, gameOverSound, selectionSound, selectionZombieSound;
        SoundEffectInstance hitInstance, hitZombieInstance, gameOverInstance, selectionInstance, selectionZombieInstance;

        // Development Version 2.0.4
        // Declare Variables for Jumping Physics
        // Requirement 1.3.1
        const int INITIAL_VELOCITY = 32;
        int jumpTime;
        double velocity, gravity;

        // Development Version 2.0.4
        // Declare Variables for Level Management, Character State and Game Reference
        // Requirements 1.4.0, 1.5.0, and 5.11.0
        // DEVELOPER NOTE: "game" used to hold a reference to the game during program execution.
        bool levelFinished, transition;
        enum CharState { Red, Green, Orange, Pink, Blue, Purple, Zombie }
        CharState currentCharState;
        Game game; 
        Color opacity;
        #endregion

        #region Load Next Level - Not Looped
        //load the next level
        public void loadLevel(int lvl)
        {
            // Development Version 1.0.0 - 3.0.2
            // Re-Initialize Variables for New Level
            // Requirement 9.0.0
            // DEVELOPER NOTE 1: "level = lvl" assigns the player's current level to the global "level" 
            //      variable.
            // DEVELOPER NOTE 2: Global content needing reinitialized in each level is between this note 
            //      and the first if statement for the level checks.
            counter = 0;
            frames = 9;
            destRect = new Rectangle(40, 330, 64, 64);
            runnerY = TRACK_HEIGHT - 29;
            multiplier = 1;
            currentStreak = 0;
            strikes = 0;
            speed = 1;
            strikeString = "";
            loadPauseData();
            gameOver = false;
            paused = false;
            oldState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
            currentMouse = Mouse.GetState();
            jumpTime = 0;
            velocity = INITIAL_VELOCITY;
            gravity = 4.5;
            level = lvl;
            headIndex = 0;
            tailIndex = 0;
            loadObstacles();
            
            if (level == 1) //Country Level
            {
                //Loads global content that doesn't need reinitialized in each level
                loadGlobalContent();
                //Start music (won't play if muted)
                MediaPlayer.Play(levelSong);

                // Development Version 1.0.1
                // Load Country and Zombie Country Background Content
                // Requirements 3.2.1 and 3.2.4
                countryBackground = Content.Load<Texture2D>("Backgrounds/CountryBackground(Sky)");
                countryHorizon = Content.Load<Texture2D>("Backgrounds/CountryBackground(Horizon)");
                countryTrack = Content.Load<Texture2D>("Backgrounds/CountryTrack");
                countryBackgroundZombie = Content.Load<Texture2D>("Backgrounds/CountryBackground(Sky)ZombieMode");
                countryHorizonZombie = Content.Load<Texture2D>("Backgrounds/CountryBackground(Horizon)ZombieMode");
                countryTrackZombie = Content.Load<Texture2D>("Backgrounds/CountryTrackZombieMode");
            
                // Developement Version 1.0.4
                // Load Obstacle Attributes for Current Level
                // Requirements 2.3.1, 10.2.0 and 10.3.0
                levelSize = LEVEL_1_SIZE;
                delays = level1;
                types = level1Type;
            
            }
            else if (level == 2) //Stadium Level
            {
                //reset distance to 0
                distance = 0;
                //start music (won't play if muted)
                MediaPlayer.Play(levelSong);

                // Development Version 2.0.6
                // Load Stadium and Zombie Stadium Background Content
                // Requirement 3.2.2
                stadiumBackgroundZombie = Content.Load<Texture2D>("Backgrounds/StadiumBackgroundZombieMode");
                stadiumBackground = Content.Load<Texture2D>("Backgrounds/StadiumBackground");
                stadiumTrack = Content.Load<Texture2D>("Backgrounds/StadiumTrack");

                // Developement Version 1.0.4
                // Load Obstacle Attributes for Current Level
                // Requirements 2.3.1, 10.2.0 and 10.3.0
                levelSize = LEVEL_2_SIZE;
                delays = level2;
                types = level2Type;
            }
            else if (level == 3) //Moon Level
            {
                //reset distance to 0
                distance = 0;
                //start music (won't play if muted)
                MediaPlayer.Play(levelSong);

                // Development Version 3.0.0
                // Load Moon and Zombie Moon Background Content
                // Requirement 3.2.3
                distantStars = Content.Load<Texture2D>("Backgrounds/MoonBackgroundDistantStars");
                stars = Content.Load<Texture2D>("Backgrounds/MoonBackgroundStars");
                moonBackground = Content.Load<Texture2D>("Backgrounds/MoonBackgroundEarth(Color)");
                moonHorizon = Content.Load<Texture2D>("Backgrounds/MoonBackground(Horizon)");
                moonForeground = Content.Load<Texture2D>("Backgrounds/MoonBackground(Craters)");
                moonTrack = Content.Load<Texture2D>("Backgrounds/MoonTrack");
                moonBackgroundZombie = Content.Load<Texture2D>("Backgrounds/MoonBackgroundEarthZombieMode");

                // Developement Version 1.0.4
                // Load Obstacle Attributes for Current Level
                // Requirements 2.3.1, 10.2.0 and 10.3.0
                levelSize = LEVEL_3_SIZE;
                delays = level3;
                types = level3Type;
            }
        }

        public void loadPauseData()
        {
            // Development Version 1.0.2
            // Load Textures and Data for Pause Button, Pause Screen and Gameover Screen
            // Requirement 5.4.0 and 5.5.0
            pauseBttnImg = Content.Load<Texture2D>("Buttons/PauseButton");
            playBttnImg = Content.Load<Texture2D>("Buttons/PlayButton");
            pausedTitle = Content.Load<Texture2D>("PausedScreen/PausedTitle");
            pausedQuit = Content.Load<Texture2D>("PausedScreen/PausedQuit");
            pausedResume = Content.Load<Texture2D>("PausedScreen/PausedResume");
            pausedSelector = Content.Load<Texture2D>("PausedScreen/PausedSelector");
            gameOverTitle = Content.Load<Texture2D>("GameOverScreen/GameOverTitle");
            gameOverReplay = Content.Load<Texture2D>("GameOverScreen/GameOverReplay");
            //create buttons for pausing/game over

            // Development Version 1.0.2
            // Set Pause and Quit Buttons to the Appropriate Graphic, Size and position
            // Requirement 13.1.0
                    /*set button graphics*/
            pauseBtn = new mButton(pauseBttnImg, graphics.GraphicsDevice);
            pausedTitleBtn = new mButton(pausedTitle, graphics.GraphicsDevice);
            pausedQuitBtn = new mButton(pausedQuit, graphics.GraphicsDevice);
            pausedResumeBtn = new mButton(pausedResume, graphics.GraphicsDevice);
            gameOverTitleBtn = new mButton(gameOverTitle, graphics.GraphicsDevice);
            gameOverReplayBtn = new mButton(gameOverReplay, graphics.GraphicsDevice);
                    /*set button sizes*/
            pauseBtn.setSize(new Vector2(pauseBttnImg.Width / 2, pauseBttnImg.Height / 2));
            pausedTitleBtn.setSize(new Vector2(pausedTitle.Width, pausedTitle.Height));
            pausedQuitBtn.setSize(new Vector2(pausedQuit.Width, pausedQuit.Height));
            pausedResumeBtn.setSize(new Vector2(pausedResume.Width, pausedResume.Height));
            gameOverTitleBtn.setSize(new Vector2(gameOverTitle.Width, gameOverTitle.Height));
            gameOverReplayBtn.setSize(new Vector2(gameOverReplay.Width, gameOverReplay.Height));
                    /*set button positions*/
            pauseBtn.setPosition(new Vector2(730, 10));
            pausedTitleBtn.setPosition(new Vector2(330, 150));
            pausedQuitBtn.setPosition(new Vector2(450, 220));
            pausedResumeBtn.setPosition(new Vector2(300, 220));
            gameOverTitleBtn.setPosition(new Vector2(310, 150));
            gameOverReplayBtn.setPosition(new Vector2(300, 220));
        }

        //load content used across all levels
        public void loadGlobalContent()
        {
            // Development Version 1.0.2
            // Load Obstacle Content
            // Requirement 2.0.0
            finishLine = Content.Load<Texture2D>("Obstacles/FinishLine");
            hurdle = Content.Load<Texture2D>("Obstacles/Hurdle");
            overheadBar = Content.Load<Texture2D>("Obstacles/OverheadBar");
            tombstoneHurdle = Content.Load<Texture2D>("Obstacles/TombstoneHurdle");
            bonesOverheadBar = Content.Load<Texture2D>("Obstacles/BonesOverheadBar");

            // Developement Version 2.0.0
            // Load Black Image for Pause and Quit Screen Dimming
            // Requirements 4.2.2 and 4.4.3
            black = Content.Load<Texture2D>("black");

            // Development Version 2.0.1
            // Load Level Completed Image for Level Completed Screen
            // Requirement 5.11.1
            completedImg = Content.Load<Texture2D>("LevelCompleted");


            // Development version 2.0.1
            // Initialize Animations With Appropriate Shoe Color
            // Requirements 1.4.1, 1.4.2, 1.4.3, 1.4.4, 1.4.5, 1.4.6, and 1.4.7
            // DEVELOPER NOTE: Second value is timing related, last value determines whether it loops 
            //     or not. Running/ducking loops repeatedly, jumping does not.
            if (currentCharState == CharState.Red)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlpha"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlpha"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlpha"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Blue)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlphaBlue"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlphaBlue"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlphaBlue"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Pink)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlphaPink"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlphaPink"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlphaPink"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Orange)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlphaOrange"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlphaOrange"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlphaOrange"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Green)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlphaGreen"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlphaGreen"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlphaGreen"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Zombie)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunZombie"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpZombie"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchZombie"), 0.1f / speed, true);
            }
            else if (currentCharState == CharState.Purple)
            {
                stickRun = new Animation(Content.Load<Texture2D>("Sprite/RunAlphaPurple"), 0.1f / speed, true);
                stickJump = new Animation(Content.Load<Texture2D>("Sprite/JumpAlphaPurple"), 0.1f, false);
                stickCrouch = new Animation(Content.Load<Texture2D>("Sprite/CrouchAlphaPurple"), 0.1f / speed, true);
            }

            // Development Version 2.0.3
            // Load Music
            // Requirement 7.0.0
            levelSong = Content.Load<Song>("Sound/NateTheme");

            // Development Version 2.0.3
            // Load Sound Effects
            // Requirement 7.3.0
            jumpSound = Content.Load<SoundEffect>("Sound/Jump");
            hitSound = Content.Load<SoundEffect>("Sound/Hit");
            hitZombieSound = Content.Load<SoundEffect>("Sound/ZombieSound2");
            selectionSound = Content.Load<SoundEffect>("Sound/Selection");
            selectionZombieSound = Content.Load<SoundEffect>("Sound/ZombieSound1");
            gameOverSound = Content.Load<SoundEffect>("Sound/Boo");
            hitInstance = hitSound.CreateInstance();
            hitInstance.IsLooped = false;
            hitZombieInstance = hitZombieSound.CreateInstance();
            hitZombieInstance.IsLooped = false;
            selectionInstance = selectionSound.CreateInstance();
            selectionInstance.IsLooped = false;
            selectionZombieInstance = selectionZombieSound.CreateInstance();
            selectionZombieInstance.IsLooped = false;
            gameOverInstance = gameOverSound.CreateInstance();
            gameOverInstance.IsLooped = false;
        }

        public void loadObstacles()
        {
            // Developement Version 1.0.2 - 3.0.2
            // Load Obstacles from File
            // Requirements 10.0.0
            levelIndex = 0;
            string line;
            int i;
            StreamReader sr;

            if (level == 1) //country level
            {
                // Redundancy check for game initialization
                totalDistance = 0;
                distance = 0;
                score = 0;

                // Developement Version 1.0.2
                // Load Obstacle Time Intervals (Delays) from File
                // Requirements 10.3.4
                line = string.Empty;
                i = 0;
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level1Delays.txt");
                while ((line = sr.ReadLine()) != null && i < LEVEL_1_SIZE)
                {
                    level1[i] = (float)Convert.ToInt32(line);
                    i++;
                }

                // Developement Version 1.0.2
                // Load Obstacle Types from File
                // Requirements 10.2.1
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level1Types.txt");
                i = 0;
                while ((line = sr.ReadLine()) != null && i < LEVEL_1_SIZE)
                {
                    level1Type[i] = Convert.ToInt32(line);
                    i++;
                }
            }
            else if (level == 2) //stadium level
            {
                // Developement Version 2.0.4
                // Load Obstacle Time Intervals (Delays) from File
                // Requirements 10.3.4
                line = string.Empty;
                i = 0;
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level2Delays.txt");
                while ((line = sr.ReadLine()) != null && i < LEVEL_2_SIZE)
                {
                    level2[i] = (float)Convert.ToInt32(line);
                    i++;
                }

                // Developement Version 2.0.4
                // Load Obstacle Types from File
                // Requirements 10.2.1
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level2Types.txt");
                i = 0;
                while ((line = sr.ReadLine()) != null && i < LEVEL_2_SIZE)
                {
                    level2Type[i] = Convert.ToInt32(line);
                    i++;
                }
            }
            else if (level == 3) //moon level
            {
                // Developement Version 3.0.0
                // Load Obstacle Time Intervals (Delays) from File
                // Requirements 10.3.4
                line = string.Empty;
                i = 0;
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level3Delays.txt");
                while ((line = sr.ReadLine()) != null && i < LEVEL_3_SIZE)
                {
                    level3[i] = (float)Convert.ToInt32(line);
                    i++;
                }

                // Developement Version 3.0.0
                // Load Obstacle Types from File
                // Requirements 10.2.1
                sr = new StreamReader("../../../../TheRunnerDataFiles/Level3Types.txt");
                i = 0;
                while ((line = sr.ReadLine()) != null && i < LEVEL_3_SIZE)
                {
                    level3Type[i] = Convert.ToInt32(line);
                    i++;
                }
            }
        }
        #endregion

        #region Constructor
        //constructor
        //gfx = the graphic device
        public Level(GraphicsDeviceManager gfx, IServiceProvider serviceProvider, SpriteBatch sb, Game gm)
        {
            // Development Version 1.0.2 - 2.0.4
            // Initialize Variables for Game Start
            // Requirement 9.0.0
            graphics = gfx;
            Content = new ContentManager(serviceProvider, "Content");
            Content.RootDirectory = "Content";
            spriteBatch = sb;//Link spritebatch to the passed variable
            mainFont = Content.Load<SpriteFont>("GeneralFont");  //global font
            game = gm;
            levelFinished = true;
            transition = false;
            opacity = new Color(0, 0, 0, 125); //for drawing transparent layer during pause/game over
        }
        #endregion

        #region Update Logic in Game Loop

        // Development Version 1.0.0 - 3.0.1
        // Handles Updates to Game Logic
        // Requirement 14.0.0
        public void updateLogic(GameTime gameTime)
        {
            // Development Version 2.0.0
            // Update Mouse and Keyboard States
            // Requirement 15.1.0
            currentMouse = Mouse.GetState();
            currentKeyboard = Keyboard.GetState();

            // Development Version 1.0.4
            // Update Game Buttons
            // Requirement 13.2.0
            pauseBtn.Update(currentMouse);
            pausedTitleBtn.Update(currentMouse);
            pausedQuitBtn.Update(currentMouse);
            pausedResumeBtn.Update(currentMouse);

            // Development Version 1.0.4
            // Pause/Resume Game When Player Clicks Pause Button or Presses the P Key
            // Requirements 5.4.1, 5.4.2 and 5.5.1
            if (pauseBtn.isClicked  
                || (currentKeyboard.IsKeyDown(Keys.P) && !oldKeyboard.IsKeyDown(Keys.P))) //set pause flag to true if clicked and change button image
            {
                pauseUnpauseGame();
            }
            // Development Version 1.0.4
            // Quit Button is Selected From Pause or Game Over Menu
            // Requirement 5.9.2
            if (pausedQuitBtn.isClicked && paused)
            {
                // Development Version 1.0.4
                // Check Highscores if Score is Greater Than 0
                // Requirements 6.4.1, 6.4.2 and 12.0.0
                if (score > 0)
                    updateHighscores();
                mainMenu = true;
            }
            // Development Version 1.0.4
            // Resumes Game When Player Clicks Resume Button Only When Game is Paused
            // Requirement 5.5.2
            if (paused && pausedResumeBtn.isClicked)
            {
                pauseUnpauseGame();
            }
            // Development Version 1.0.4
            // Restarts Game at Level 1 When Replay is Chosen
            // Requirement 4.4.4
            if (gameOverReplayBtn.isClicked)
            {
                // Development Version 1.0.4
                // Check Highscores if Score is Greater Than 0
                // Requirements 6.4.1, 6.4.2 and 12.0.0
                if(score > 0)
                    updateHighscores();
                loadLevel(1);
            }

            // Development Version 1.0.3
            // Update Saved (Old) State
            // Requirement 15.2.0
            oldKeyboard = currentKeyboard;
            oldMouse = currentMouse;

            if (!paused && !transition) //not paused, so continue gameplay
            {
                if (level == 1)
                {
                    // Development Version 1.0.0
                    // Move Background
                    // Requirement 3.1.0
                    backgroundSpeed();

                    // Development Version 1.0.0
                    // Update Level Logic Whenever Game is Running
                    // Requirement 14.1.0
                    updateLevelLogic(gameTime);
                }
                else if (level == 2)
                {
                    // Development Version 2.0.0
                    // Move Background
                    // Requirement 3.1.0
                    backgroundSpeed();

                    // Development Version 2.0.1
                    // Update Level Logic Whenever Game is Running
                    // Requirement 14.1.0
                    updateLevelLogic(gameTime);
                }
                else if (level == 3)
                {
                    // Development Version 3.0.0
                    // Move Background
                    // Requirement 3.1.0
                    backgroundSpeed();

                    // Development Version 3.0.1
                    // Update Level Logic Whenever Game is Running
                    // Requirement 14.1.0
                    updateLevelLogic(gameTime);
                }
                else //all levels cleared
                {
                    // Development Version 1.0.2
                    // End Game
                    // Requirement 5.9.3
                    paused = true;
                    gameOver = true;
                }

                // Development Version 1.0.2
                // Update all inputs
                // Requirement 14.2.0
                UpdateInput();
            }
            else if (transition)
            {
                // Development Version 2.0.4
                // Initiate Level Transition
                // Requirement 5.11.1
                startTransition(gameTime);
            }
            else //either paused or game over (pause has to be set along with gameover for this to execute)
            {
                // Development Version 1.0.4
                // Pause Gameplay
                // Requirement 5.4.3
                MediaPlayer.Stop();
                stickRun.setRunning(false);
                stickCrouch.setRunning(false);
                stickJump.setRunning(false);

                //prevents character from being stuck in crouch animation until a button is pressed/released upon unpausing
                crouching = false;

                // Development Version 1.0.4
                // Game Over Screen Functionality
                // Requirement 4.4.0
                if (gameOver)
                {
                    // Development Version 1.0.4
                    // Option to Quit and Replay
                    // Requirement 4.4.1 and 4.4.2
                    pausedQuitBtn.Update(currentMouse);
                    gameOverTitleBtn.Update(currentMouse);
                    gameOverReplayBtn.Update(currentMouse);
                }
            }
        }

        // Development Version 1.0.4
        // Handles Pause and Resume Functions
        // Requirements 5.4.0 and 5.5.0
        private void pauseUnpauseGame()
        {
            // Development Version 1.0.4
            // Pause Game
            // Requirements 5.4.1 and 5.4.2
            if (!paused)
            {
                paused = true;
                pauseBtn.changeImage(playBttnImg);
            }
            // Development Version 1.0.4
            // Unpause Game
            // Requirements 5.5.1, 5.5.2 and 5.5.3
            else
            {
                paused = false;
                //Resume game music
                MediaPlayer.Resume();
                //Change image back
                pauseBtn.changeImage(pauseBttnImg);
                //Set all animations to resume after unpause
                stickRun.setRunning(true);
                stickCrouch.setRunning(true);
                stickJump.setRunning(true);
                dontJump = true; //Set to not jump on unpausing with mouse
            }
        }

        // Development Version 2.0.6
        // Handles Transition Screen
        // Requirement 5.11.1
        private void startTransition(GameTime timer)
        {
            transitionElapsed += (float)timer.ElapsedGameTime.TotalMilliseconds;
            transition = true;

            // Development Version 2.0.6
            // Display Transition Screen for 4 Seconds
            // Requirement 5.11.2
            if (transitionElapsed > 4000)
            {
                transition = false;
                levelFinished = true;
                transitionElapsed = 0;
            }
        }

        // Development Version 1.0.0 - 3.0.2
        // Determines Background Speed for the Levels According to the Runner's Speed
        // Requirement 3.1.0
        private void backgroundSpeed()
        {
            if (level == 1)
            {
                // Development Version 1.0.0
                // Move Background Sections Differently to Simulate Depth
                // Requirements 3.1.1 and 3.1.2
                trackX -= 4 * speed;
                horizonX -= 2 * speed;
                backgroundX -= speed;

                // Development Version 1.0.0
                // Reset Background Sections Once Looping Point (LP) is Reached
                // Requirement 3.1.3
                if (backgroundX < -1693)
                    backgroundX = -1;
                if (horizonX < -1693)
                    horizonX = -2;
                if (trackX < -1693)
                    trackX = -4;
            }
            else if (level == 2)
            {
                // Development Version 2.0.3
                // Move Background Sections Differently to Simulate Depth
                // Requirements 3.1.1 and 3.1.2
                trackX -= 4 * speed;
                backgroundX -= 2 * speed;

                // Development Version 2.0.3
                // Reset Background Sections Once Looping Point (LP) is Reached
                // Requirement 3.1.3
                if (backgroundX < -1693)
                    backgroundX = -2;
                if (trackX < -1693)
                    trackX = -4;
            }
            else if (level == 3)
            {
                // Development Version 3.0.2
                // Move Background Sections Differently to Simulate Depth
                // Requirements 3.1.1 and 3.1.2
                trackX -= 4 * speed;
                foregroundX -= 2 * speed;
                horizonX -= speed;
                if (counter % 2 == 0)
                {
                    backgroundX -= speed;
                }

                if (counter % 4 == 0)
                {
                    starsX -= speed;
                }

                if (counter % 8 == 0)
                {
                    distantStarsX -= speed;
                    counter = 0;
                }

                // Development Version 3.0.2
                // Reset Background Sections Once Looping Point (LP) is Reached
                // Requirement 3.1.3
                if (distantStarsX < -1693)
                    distantStarsX = -1;
                if (starsX < -1693)
                    starsX = -1;
                if (backgroundX < -1693)
                    backgroundX = -1;
                if (horizonX < -1693)
                    horizonX = -1;
                if (foregroundX < -1693)
                    foregroundX = -2;
                if (trackX < -1693)
                    trackX = -4;
            }
        }

        // Development Version 1.0.1
        // Update Level Logic
        // Requirement 14.1.1
        private void updateLevelLogic(GameTime gameTime)
        {
            // Development Version 1.0.1
            // Increment Current Level Distance
            // Requirement 5.10.0
            distance += .015 * speed;

            // Development Version 2.0.2
            // Add Distance to Total Distance Traveled
            // Requirement 5.10.1
            totalDistance += .015 * speed;


            // Add the time passed since last frame to the time elapsed.
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Development Version 1.0.2
            // Animations for Running, Jumping and Crouching
            // Requirement 11.3.0
            if (jumping)
            {
                // Development Version 1.0.2
                // Cycle Through Jump Animation Frames on Frame Time
                // Requirement 11.1.3
                if (elapsed >= JUMP_DELAY)
                {
                    // Development Version 1.0.2
                    // Reset Frame Number for Jump Animation
                    // Requirement 11.1.4
                    if (frames <= 0)
                    {
                        frames = 9;
                    }
                    else
                    {
                        frames--;
                    }
                    elapsed = 0;//Reset elapsed for next frame time
                }
            }
            // Development Version 1.0.2
            // Cycle Through Run Animation Frames on Frame Time
            // Requirement 11.1.3
            else if (speed > 0)
            {
                if (elapsed >= delay)
                {
                    // Development Version 1.0.2
                    // Reset Frame Number for Run Animation
                    // Requirement 11.1.4
                    if (frames <= 0)
                    {
                        frames = 9;
                    }
                    else
                    {
                        frames--;
                    }
                    elapsed = 0;
                }
            }

            if (crouching && !jumping)
            {
                // Development Version 1.0.2
                // Set Cource and Destination Rectangles for Crouching Animation
                // Requirements 11.1.2, 11.1.5 and 11.3.1
                // DEVELOPER NOTE: "runnerY + 21" is used so crouching can successfully avoid overhead bars.
                srcRect = new Rectangle(64 * frames, 0, 64, stickCrouch.FrameHeight);
                destRect = new Rectangle(20, runnerY + 21, 64, stickCrouch.FrameHeight);
            }
            else
            {
                // Development Version 1.0.2
                // Set Source and Destination Rectangles for Running or Jumping Animation
                // Requirement 11.1.2, 11.1.5 and 11.3.2
                srcRect = new Rectangle(64 * frames, 0, 64, 64);
                destRect = new Rectangle(20, runnerY, 64, stickRun.FrameHeight);
            }

            // Development Version 1.0.4
            // Generate Obstacles
            // Requirement 2.0.0
            if (speed > 0)
            {
                obstacleElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Development Version 2.0.4
                // Add Obstacles Based on Level Time Intervals (Delays)
                // Requirement 10.3.0
                if (obstacleElapsed >= delays[levelIndex])
                {
                    // Development Version 1.0.4
                    // Increase Speed Only When Player Has Enough Time to React While Speed < Level
                    // Requirements 1.2.4 and 1.2.5
                    if (delays[levelIndex] > 3000 && speed < level)
                    {
                        speed++;
                    }

                    // Development Version 1.0.4
                    // Assign New Obstacle Type
                    // Requirement 10.2.0
                    if (levelIndex < levelSize)
                        hurdleType = types[levelIndex];

                    // Development Version 1.0.4
                    // Add New Obstacle at Head Index
                    // Requirement 10.3.1
                    obstacles[headIndex] = new Obstacle();
                    obstacles[headIndex].createObstacle(hurdleType);

                    // Development Version 1.0.4
                    // Set Corect Draw Height Based on Obstacle Type
                    // Requirement 10.2.5
                    if (hurdleType == 1)
                        obstacles[headIndex].setYPosition(TRACK_HEIGHT - 22);
                    else
                        obstacles[headIndex].setYPosition(TRACK_HEIGHT - 54);

                    // Development Version 1.0.4
                    // Increment Head Index
                    // Requirement 10.3.2
                    headIndex++;
                    if (headIndex >= obstacles.Length)
                    {
                        // Development Version 1.0.4
                        // Reset Head Index When End of Array is Reached
                        // Requirement 10.3.3
                        headIndex = 0;
                    }

                    // Development Version 1.0.4
                    // Increment Level Index
                    // Requirement 10.3.5
                    levelIndex++;

                    obstacleElapsed = 0;//Reset obstacle elapsed time.
                }
            }
        }

        // Development Version 1.0.3
        // Handle Input Updates
        // Requirement 15.0.0
        private void UpdateInput()
        {
            // Development Version 2.0.0
            // Update Mouse and Keyboard States
            // Requirement 15.1.0
            KeyboardState newState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            Rectangle currentMousePosition = new Rectangle(newMouseState.X, newMouseState.Y, 1, 1);

            // Development Version 2.0.0
            // Don't Jump if Player Clicks Pause/Resume Button
            // Requirement 5.2.3
            if (!dontJump)
            {
                // Development Version 2.0.0
                // Jump Runner on Left-Click of Mouse
                // Requirement 5.2.2
                // DEVELOPER NOTE: Old state check ensures the button is newly pressed.
                if (currentMouse.LeftButton == ButtonState.Pressed) 
                {
                    if (!(oldMouseState.LeftButton == ButtonState.Pressed))
                    {
                        // Development Version 2.0.3
                        // Don't Play Jump Sound Effect When Muted
                        // Requirement 7.4.0
                        if (!jumping && !MediaPlayer.IsMuted)
                        {
                            // Development Version 2.0.3
                            // Play Zombie Jump Sound Effect
                            // Requirement 7.3.1
                            jumpSound.Play();
                        }
                        jumping = true;
                    }
                    frames = 0;
                }
                // Development Version 2.0.0
                // Crouch Runner on Right-Click of Mouse
                // Requirement 5.3.2
                // DEVELOPER NOTE: Old state check ensures the button is newly pressed.
                if (newMouseState.RightButton == ButtonState.Pressed)
                {
                    if (!(oldMouseState.RightButton == ButtonState.Pressed))
                    {
                        crouching = true;
                    }
                }
                // Development Version 2.0.0
                // Exit Crouch When Right-Click of Mouse is Lifted
                // Requirement 5.3.0
                else if (oldMouseState.RightButton == ButtonState.Pressed)
                {
                    crouching = false;
                }
                // Development Version 2.0.0
                // Jump Runner on Up Arrow Press
                // Requirement 5.2.1
                // DEVELOPER NOTE: Old state check ensures the key is newly pressed.
                if (newState.IsKeyDown(Keys.Up))
                {
                    if (!oldState.IsKeyDown(Keys.Up))
                    {
                        // Development Version 2.0.3
                        // Don't Play Jump Sound Effect When Muted
                        // Requirement 7.4.0
                        if (!jumping && !MediaPlayer.IsMuted)
                        {
                            // Development Version 2.0.3
                            // Play Zombie Jump Sound Effect
                            // Requirement 7.3.1
                            jumpSound.Play();
                        }
                        jumping = true;
                    }
                    frames = 0;
                }
                // Development Version 2.0.0
                // Crouch Runner on Down Arrow Press
                // Requirement 5.3.1
                // DEVELOPER NOTE: Old state check ensures the key is newly pressed.
                if (newState.IsKeyDown(Keys.Down))
                {
                    if (!oldState.IsKeyDown(Keys.Down))
                    {
                        crouching = true;
                    }
                }
                // Development Version 2.0.0
                // Exit Crouch When Down Arrow Key is Lifted
                // Requirement 5.3.0
                else if (oldState.IsKeyDown(Keys.Down))
                {
                    crouching = false;
                }
                counter++;// Used to control background section movement and timing.

                // Development Version 2.0.0
                // Make Runner Jump When Commanded
                // Requirement 5.2.0
                if (jumping)
                    Jump();

                // Development Version 1.0.2
                // Update Obstacles
                // Requirement 2.5.0
                updateObstacles();

                // Development Version 1.0.3
                // Update Saved (Old) State
                // Requirement 15.2.0
                oldState = newState;
                oldMouseState = currentMouse;
            }
            else// Just unpaused with mouse, don't initiate jump on current click.
            {
                oldMouseState = currentMouse;
                dontJump = false;
            }
        }

        // Development Version 1.0.2
        // Make Runner Jump
        // Requirement 5.2.0
        private void Jump()
        {
            jumpTime++;

            // Development Version 2.0.4
            // Jump Based on Physics
            // Requirement 1.3.1
            // DEVELOPER NOTE: Modulus 3 is used to slow down the jumping action
            //      to help the runner stay aloft longer
            if (jumpTime % 3 == 0)
            {
                velocity -= gravity;
                runnerY -= (int)velocity;
                if (runnerY >= TRACK_HEIGHT - 29)
                {
                    runnerY = TRACK_HEIGHT - 29;
                    velocity = INITIAL_VELOCITY;
                    jumping = false;
                    jumpTime = 0;
                }
            }
        }

        // Development Version 1.0.4
        // Update Obstacle Position Movement
        // Requirement 2.5.0
        private void updateObstacles()
        {
            // Development Version 1.0.4
            // Manage Array With Head and Tail Index Variables
            // Requirement 10.1.2
            if (tailIndex < headIndex)
            {
                // Development Version 1.0.4
                // Iterate Through the Array From Tail Index to Head Index
                // Requirement 10.5.0
                for (int i = tailIndex; i < headIndex; i++)
                {
                    if (obstacles[i] != null)// Skip any null obstacles (precautionary: shouldn't occur)
                    {
                        // Development Version 1.0.4
                        // Move Obstacles According to the Runner's Pace Automatically
                        // Requirements 1.2.0 and 1.2.1
                        obstacles[i].setXPosition(obstacles[i].getXPosition() - 4 * speed);

                        // Development Version 1.0.4
                        // Obstacle Collision With Runner
                        // Requirement 2.6.0
                        if (destRect.Intersects(obstacles[i].getFrame()))
                        {
                            // Development Version 1.0.4
                            // Mark Obstacle as Hit
                            // Requirement 2.6.1
                            obstacles[i].setHitStatus(true);

                            // Development Version 2.0.3
                            // Don't Play Sound Effects if Muted
                            // Requirement 7.4.0
                            if (!MediaPlayer.IsMuted)
                            {
                                if (currentCharState == CharState.Zombie)
                                {
                                    // Development Version 2.0.3
                                    // Play Zombie Hit Sound Effect
                                    // Requirement 7.3.3
                                    hitZombieInstance.Play();
                                }
                                else if (obstacles[i].getType() != 2)
                                {
                                    // Development Version 2.0.3
                                    // Play Standard Hit Sound Effect
                                    // Requirement 7.3.2
                                    hitInstance.Play();
                                }
                            }

                            // Development Version 1.0.4
                            // Reduce Runner's Speed to Initial Level When Obstacle is Hit
                            // Requirement 1.2.3
                            speed = 1;
                            
                        }

                        // Development Version 1.0.4
                        // Obstacle Reached Far Left Side of Screen and Out of View
                        // Requirement 10.4.0
                        if (obstacles[i].getXPosition() < -obstacles[i].getWidth())
                        {
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Avoided
                            // Requirements 6.0.0 and 10.4.4
                            if (!obstacles[i].getHitStatus())
                            {
                                // Development Version 1.0.1
                                // Calculate Progressive Awarded Score (PAS) and Add to Total Score
                                // Requirements 6.2.0, 6.2.1 and 6.3.0
                                calculatePAS();

                                // Development Version 1.0.4
                                // Increment Current Streak
                                // Requirement 5.6.0
                                currentStreak++;

                                // Development Version 1.0.4
                                // Doubles Multiplier Every 10 Successful Clears Until x16 is Reached
                                // Requirements 5.7.0, 5.7.1, 5.7.2, 5.7.3 and 5.7.4
                                if (currentStreak % MULTIPLIER_THRESHOLD == 0 && currentStreak < 50)
                                {
                                    multiplier = multiplier * 2;
                                }
                            }
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Hit
                            // Requirement 10.4.4
                            else if (obstacles[i].getType() != 2)
                            {
                                // Development Version 1.0.4
                                // Award a Strike for Overhead Bar or Hurdle Collision
                                // Requirements 2.1.2 and 2.2.2
                                strikes++;
                                if (strikes <= STRIKE_THRESHOLD)
                                {
                                    strikeString = strikeString + "X ";
                                }

                                // Development Version 1.0.4
                                // Reset Multiplier and Streak When Obstacle Hit
                                // Requirements 5.6.1 and 5.7.5
                                multiplier = 1;
                                currentStreak = 0;
                            }
                            else
                            {
                                // Development Version 1.0.4
                                // Level Completed on Finish Line Collision
                                // Requirement 2.4.1
                                transition = true;//levelFinished = true;
                            }

                            // Development Version 1.0.4
                            // Remove Obstacle at Tail Index (TI) and Increment the TI
                            // Requirement 10.4.1 and 10.4.2
                            obstacles[i] = null;
                            tailIndex++;

                            // Development Version 1.0.4
                            // Reset the TI to 0 When the End of the Array is Reached
                            // Requirement 10.4.3
                            if (tailIndex >= obstacles.Length)
                                tailIndex = 0;
                        }
                    }
                }
            }
            // Development Version 1.0.4
            // Iterate Through the Array From Tail to Array End and From 0 to Head
            // Requirement 10.5.1
            else if (headIndex < tailIndex)
            {
                for (int i = tailIndex; i < obstacles.Length; i++)
                {
                    if (obstacles[i] != null) // Skip any null obstacles (precautionary: shouldn't occur)
                    {
                        // Development Version 1.0.4
                        // Move Obstacles According to the Runner's Pace Automatically
                        // Requirements 1.2.0 and 1.2.1
                        obstacles[i].setXPosition(obstacles[i].getXPosition() - 4 * speed);

                        // Development Version 1.0.4
                        // Obstacle Collision With Runner
                        // Requirement 2.6.0
                        if (destRect.Intersects(obstacles[i].getFrame()))
                        {
                            // Development Version 1.0.4
                            // Mark Obstacle as Hit
                            // Requirement 2.6.1
                            obstacles[i].setHitStatus(true);

                            // Development Version 2.0.3
                            // Don't Play Sound Effects if Muted
                            // Requirement 7.4.0
                            if (!MediaPlayer.IsMuted)
                            {
                                if (currentCharState == CharState.Zombie)
                                {
                                    // Development Version 2.0.3
                                    // Play Zombie Hit Sound Effect
                                    // Requirement 7.3.3
                                    hitZombieInstance.Play();
                                }
                                else if (obstacles[i].getType() != 2)
                                {
                                    // Development Version 2.0.3
                                    // Play Standard Hit Sound Effect
                                    // Requirement 7.3.2
                                    hitInstance.Play();
                                }
                            }

                            // Development Version 1.0.4
                            // Reduce Runner's Speed to Initial Level When Obstacle is Hit
                            // Requirement 1.2.3
                            speed = 1;

                        }

                        // Development Version 1.0.4
                        // Obstacle Reached Far Left Side of Screen and Out of View
                        // Requirement 10.4.0
                        if (obstacles[i].getXPosition() < -obstacles[i].getWidth())
                        {
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Avoided
                            // Requirements 6.0.0 and 10.4.4
                            if (!obstacles[i].getHitStatus())
                            {
                                // Development Version 1.0.1
                                // Calculate Progressive Awarded Score (PAS) and Add to Total Score
                                // Requirements 6.2.0, 6.2.1 and 6.3.0
                                calculatePAS();

                                // Development Version 1.0.4
                                // Increment Current Streak
                                // Requirement 5.6.0
                                currentStreak++;

                                // Development Version 1.0.4
                                // Doubles Multiplier Every 10 Successful Clears Until x16 is Reached
                                // Requirements 5.7.0, 5.7.1, 5.7.2, 5.7.3 and 5.7.4
                                if (currentStreak % MULTIPLIER_THRESHOLD == 0 && currentStreak < 50)
                                {
                                    multiplier = multiplier * 2;
                                }
                            }
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Hit
                            // Requirement 10.4.4
                            else if (obstacles[i].getType() != 2)
                            {
                                // Development Version 1.0.4
                                // Award a Strike for Overhead Bar or Hurdle Collision
                                // Requirements 2.1.2 and 2.2.2
                                strikes++;
                                if (strikes <= STRIKE_THRESHOLD)
                                {
                                    strikeString = strikeString + "X ";
                                }

                                // Development Version 1.0.4
                                // Reset Multiplier and Streak When Obstacle Hit
                                // Requirements 5.6.1 and 5.7.5
                                multiplier = 1;
                                currentStreak = 0;
                            }
                            else
                            {
                                // Development Version 1.0.4
                                // Level Completed on Finish Line Collision
                                // Requirement 2.4.1
                                transition = true;//levelFinished = true;
                            }

                            // Development Version 1.0.4
                            // Remove Obstacle at Tail Index (TI) and Increment the TI
                            // Requirement 10.4.1 and 10.4.2
                            obstacles[i] = null;
                            tailIndex++;

                            // Development Version 1.0.4
                            // Reset the TI to 0 When the End of the Array is Reached
                            // Requirement 10.4.3
                            if (tailIndex >= obstacles.Length)
                                tailIndex = 0;
                        }
                    }
                }
                for (int i = 0; i < headIndex; i++)
                {
                    if (obstacles[i] != null)// Skip any null obstacles (precautionary: shouldn't occur)
                    {
                        // Development Version 1.0.4
                        // Move Obstacles According to the Runner's Pace Automatically
                        // Requirements 1.2.0 and 1.2.1
                        obstacles[i].setXPosition(obstacles[i].getXPosition() - 4 * speed);

                        // Development Version 1.0.4
                        // Obstacle Collision With Runner
                        // Requirement 2.6.0
                        if (destRect.Intersects(obstacles[i].getFrame()))
                        {
                            // Development Version 1.0.4
                            // Mark Obstacle as Hit
                            // Requirement 2.6.1
                            obstacles[i].setHitStatus(true);

                            // Development Version 2.0.3
                            // Don't Play Sound Effects if Muted
                            // Requirement 7.4.0
                            if (!MediaPlayer.IsMuted)
                            {
                                if (currentCharState == CharState.Zombie)
                                {
                                    // Development Version 2.0.3
                                    // Play Zombie Hit Sound Effect
                                    // Requirement 7.3.3
                                    hitZombieInstance.Play();
                                }
                                else if (obstacles[i].getType() != 2)
                                {
                                    // Development Version 2.0.3
                                    // Play Standard Hit Sound Effect
                                    // Requirement 7.3.2
                                    hitInstance.Play();
                                }
                            }

                            // Development Version 1.0.4
                            // Reduce Runner's Speed to Initial Level When Obstacle is Hit
                            // Requirement 1.2.3
                            speed = 1;

                        }

                        // Development Version 1.0.4
                        // Obstacle Reached Far Left Side of Screen and Out of View
                        // Requirement 10.4.0
                        if (obstacles[i].getXPosition() < -obstacles[i].getWidth())
                        {
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Avoided
                            // Requirements 6.0.0 and 10.4.4
                            if (!obstacles[i].getHitStatus())
                            {
                                // Development Version 1.0.1
                                // Calculate Progressive Awarded Score (PAS) and Add to Total Score
                                // Requirements 6.2.0, 6.2.1 and 6.3.0
                                calculatePAS();

                                // Development Version 1.0.4
                                // Increment Current Streak
                                // Requirement 5.6.0
                                currentStreak++;

                                // Development Version 1.0.4
                                // Doubles Multiplier Every 10 Successful Clears Until x16 is Reached
                                // Requirements 5.7.0, 5.7.1, 5.7.2, 5.7.3 and 5.7.4
                                if (currentStreak % MULTIPLIER_THRESHOLD == 0 && currentStreak < 50)
                                {
                                    multiplier = multiplier * 2;
                                }
                            }
                            // Development Version 1.0.4
                            // Handle Scoring and Other Statistical Management For Obstacle Hit
                            // Requirement 10.4.4
                            else if (obstacles[i].getType() != 2)
                            {
                                // Development Version 1.0.4
                                // Award a Strike for Overhead Bar or Hurdle Collision
                                // Requirements 2.1.2 and 2.2.2
                                strikes++;
                                if (strikes <= STRIKE_THRESHOLD)
                                {
                                    strikeString = strikeString + "X ";
                                }

                                // Development Version 1.0.4
                                // Reset Multiplier and Streak When Obstacle Hit
                                // Requirements 5.6.1 and 5.7.5
                                multiplier = 1;
                                currentStreak = 0;
                            }
                            else
                            {
                                // Development Version 1.0.4
                                // Level Completed on Finish Line Collision
                                // Requirements 2.4.1 and 5.11.0
                                transition = true;//levelFinished = true;
                            }

                            // Development Version 1.0.4
                            // Remove Obstacle at Tail Index (TI) and Increment the TI
                            // Requirement 10.4.1 and 10.4.2
                            obstacles[i] = null;
                            tailIndex++;

                            // Development Version 1.0.4
                            // Reset the TI to 0 When the End of the Array is Reached
                            // Requirement 10.4.3
                            if (tailIndex >= obstacles.Length)
                                tailIndex = 0;
                        }
                    }
                }
            }

            // Development Version 1.0.4
            // End Game if Player has 3 Strikes
            // Requirement 5.9.1
            if (strikes > 2)
            {
                paused = true;
                gameOver = true;
            }
        }

        // Development Version 1.0.1
        // Calculates the Current Progressive Awarded Score (PAS)
        // Requirement 6.2.0
        private void calculatePAS()
        {
            // Development Version 1.0.1
            // Calculates the Current PAS According to the Formula: 
            //     PAS = [BS * CL1.2 - (BS * CL1.2  % BS)] * CM.
            // Requirement 6.2.1
            int pas = (int)(BASE_SCORE * Math.Pow(level, 1.2) - (BASE_SCORE * Math.Pow(level, 1.2) % BASE_SCORE)) * multiplier;

            // Development Version 1.0.1
            // Adds the PAS to the Player's Score
            // Requirement 6.3.0
            score += pas;
        }
        
        #endregion

        #region Drawing Logic in Game Loop

        // Development Version 1.0.0
        // Draw Gameplay to Window
        // Requirement 4.0.0
        public void Draw(GameTime gameTime)
        {
            if (!levelFinished) //level in progress
            {
                // Development Version 1.0.0
                // Draw Background According to Level and Player Mode
                // Requirements 3.0.0 and 3.2.0
                if (level == 1)
                {
                    if (currentCharState == CharState.Zombie) // Zombie mode is active
                    {
                        // Development Version 2.0.4
                        // Draw Zombie Country Background
                        // Requirement 3.2.4
                        spriteBatch.Draw(countryBackgroundZombie, new Rectangle(backgroundX, 0, countryBackground.Width, countryBackground.Height), Color.White);
                        spriteBatch.Draw(countryHorizonZombie, new Rectangle(horizonX, 0, countryHorizon.Width, countryHorizon.Height), Color.White);
                        spriteBatch.Draw(countryTrackZombie, new Rectangle(trackX, TRACK_HEIGHT, countryTrack.Width, countryTrack.Height), Color.White);

                        // Development Version 2.0.4
                        // Draw HUD for Zombie Mode
                        // Requirements 4.1.0 and 4.1.9
                        drawRedHUD();
                    }
                    else // Normal mode is active
                    {
                        // Development Version 1.0.0
                        // Draw Country Background
                        // Requirement 3.2.1
                        spriteBatch.Draw(countryBackground, new Rectangle(backgroundX, 0, countryBackground.Width, countryBackground.Height), Color.White);
                        spriteBatch.Draw(countryHorizon, new Rectangle(horizonX, 0, countryHorizon.Width, countryHorizon.Height), Color.White);
                        spriteBatch.Draw(countryTrack, new Rectangle(trackX, TRACK_HEIGHT, countryTrack.Width, countryTrack.Height), Color.White);

                        // Development Version 1.0.1
                        // Draw Standard HUD
                        // Requirements 4.1.0 and 4.1.8
                        drawBlackHUD();
                    }

                }
                else if (level == 2)
                {
                    if (currentCharState == CharState.Zombie) // Zombie mode is active
                    {
                        // Development Version 2.0.0
                        // Draw Zombie Stadium Background
                        // Requirement 3.2.5
                        spriteBatch.Draw(stadiumBackgroundZombie, new Rectangle(backgroundX, 0, stadiumBackgroundZombie.Width, stadiumBackgroundZombie.Height), Color.White);
                        spriteBatch.Draw(countryTrackZombie, new Rectangle(trackX, TRACK_HEIGHT, countryTrackZombie.Width, countryTrackZombie.Height), Color.White);

                        // Development Version 2.0.0
                        // Draw Zombie HUD
                        // Requirements 4.1.0 and 4.1.9
                        drawRedHUD();
                    }
                    else // Normal mode is active
                    {
                        // Development Version 2.0.0
                        // Draw Stadium Background
                        // Requirement 3.2.1
                        spriteBatch.Draw(stadiumBackground, new Rectangle(backgroundX, 0, stadiumBackground.Width, stadiumBackground.Height), Color.White);
                        spriteBatch.Draw(stadiumTrack, new Rectangle(trackX, TRACK_HEIGHT, stadiumTrack.Width, stadiumTrack.Height), Color.White);

                        // Development Version 2.0.0
                        // Draw Standard HUD
                        // Requirements 4.1.0 and 4.1.8
                        drawBlackHUD();
                    }
                }
                else if (level == 3)
                {
                    if (currentCharState == CharState.Zombie) // Zombie mode is active
                    {
                        // Development Version 3.0.1
                        // Draw Zombie Lunar Background
                        // Requirement 3.2.6
                        spriteBatch.Draw(distantStars, new Rectangle(distantStarsX, 0, distantStars.Width, distantStars.Height), Color.White);
                        spriteBatch.Draw(stars, new Rectangle(starsX, 0, stars.Width, stars.Height), Color.White);
                        spriteBatch.Draw(moonBackgroundZombie, new Rectangle(backgroundX, 0, moonBackground.Width, moonBackground.Height), Color.White);
                        spriteBatch.Draw(moonHorizon, new Rectangle(horizonX, 75, moonHorizon.Width, moonHorizon.Height), Color.White);
                        spriteBatch.Draw(moonForeground, new Rectangle(foregroundX, 0, moonForeground.Width, moonForeground.Height), Color.White);
                        spriteBatch.Draw(moonTrack, new Rectangle(trackX, TRACK_HEIGHT, moonTrack.Width, moonTrack.Height), Color.White);

                        // Development Version 3.0.1
                        // Draw Zombie HUD
                        // Requirements 4.1.0 and 4.1.9
                        drawRedHUD();
                    }
                    else // Normal mode is active
                    {
                        // Development Version 3.0.1
                        // Draw Lunar Background
                        // Requirement 3.2.3
                        spriteBatch.Draw(distantStars, new Rectangle(distantStarsX, 0, distantStars.Width, distantStars.Height), Color.White);
                        spriteBatch.Draw(stars, new Rectangle(starsX, 0, stars.Width, stars.Height), Color.White);
                        spriteBatch.Draw(moonBackground, new Rectangle(backgroundX, 0, moonBackground.Width, moonBackground.Height), Color.White);
                        spriteBatch.Draw(moonHorizon, new Rectangle(horizonX, 75, moonHorizon.Width, moonHorizon.Height), Color.White);
                        spriteBatch.Draw(moonForeground, new Rectangle(foregroundX, 0, moonForeground.Width, moonForeground.Height), Color.White);
                        spriteBatch.Draw(moonTrack, new Rectangle(trackX, TRACK_HEIGHT, moonTrack.Width, moonTrack.Height), Color.White);

                        // Development Version 3.0.1
                        // Draw Zombie HUD For Background Contrast Purposes
                        // Requirements 4.1.0 and 4.1.9
                        drawRedHUD();
                    }
                }
                
                // Development Version 1.0.4
                // Draw Obstacles
                // Requirement 2.0.0
                drawObstacles();

                // Development Version 1.0.4
                // Don't Draw Character and Pause Button if the Game is Over
                // Requirement 4.4.5
                if (!gameOver)
                {
                    drawCharacterAnimation(gameTime);
                    pauseBtn.Draw(spriteBatch);
                }
                // Development Version 1.0.2
                // Draw Pause or Game Over Menu
                // Requirements 4.2.0 and 4.4.0
                if (paused)
                {
                    drawPauseGameOverMenu();
                }
                // Development Version 1.0.2
                // Draw Level Transition
                // Requirement 5.11.1
                if (transition)
                {
                    spriteBatch.Draw(black, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.Black);//opacity);
                    spriteBatch.Draw(completedImg, new Rectangle(150, 200, completedImg.Width, completedImg.Height), Color.White);
                }
            }
        }

        // Development Version 1.0.2
        // Draw Pause or Game Over Menu
        // Requirements 4.2.0 and 4.4.0
        public void drawPauseGameOverMenu()
        {
            // Development Version 1.0.2
            // Dim Background and Draw Quit Button
            // Requirements 4.2.1, 4.2.2, 4.4.2 and 4.4.3
            // DEVELOPER NOTE: Quit button draws for both pause and game over screens.
            spriteBatch.Draw(black, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), opacity);
            pausedQuitBtn.Draw(spriteBatch);

            if (!gameOver)
            {
                // Development Version 1.0.2
                // Display Title and Option to Resume
                // Requirements 4.2.0 and 4.2.1
                pausedTitleBtn.Draw(spriteBatch);
                pausedResumeBtn.Draw(spriteBatch);
            }
            else // Game is over
            {
                // Development Version 1.0.2
                // Display Title and Option to Replay
                // Requirements 4.4.0 and 4.2.1
                gameOverTitleBtn.Draw(spriteBatch);
                gameOverReplayBtn.Draw(spriteBatch);
            }
        }

        // Development Version 1.0.0 - 2.0.1
        // Draw Character Jumping, Crouching and Running Animations
        // Requirement 11.3.0
        public void drawCharacterAnimation(GameTime gameTime)
        {
            if (jumping)
            {
                sprite.PlayAnimation(stickJump);
            }
            else if (crouching)
            {
                sprite.PlayAnimation(stickCrouch);
            }
            else
            {
                sprite.PlayAnimation(stickRun);
            }

            // Development Version 1.0.1
            // Declare Animation Position
            // Requirement 11.1.6
            // DEVELOPER NOTE: Sprite sheet is flipped horizontally to look in the right direction.
            Vector2 position;
            position.X = RUNNER_X_POSITION;
            position.Y = runnerY + stickRun.FrameHeight;
            sprite.Draw(gameTime, spriteBatch, position, SpriteEffects.FlipHorizontally);
        }

        // Development Version 2.0.4
        // Draw Zombie HUD
        // Requirements 4.1.0 and 4.1.9
        public void drawRedHUD()
        {
            // Development Version 2.0.4
            // Display Current Score
            // Requirement 4.1.1
            spriteBatch.DrawString(mainFont, "Score: " + score, new Vector2(10, 0), Color.Red);

            // Development Version 2.0.4
            // Display Current Level
            // Requirement 4.1.2
            spriteBatch.DrawString(mainFont, "Level " + level, new Vector2(365, 0), Color.Red);

            // Development Version 2.0.4
            // Display Current Streak
            // Requirement 4.1.3
            spriteBatch.DrawString(mainFont, "Streak: " + currentStreak, new Vector2(350, 30), Color.Red);
           
            // Development Version 2.0.4
            // Display Current Strikes
            // Requirement 4.1.6
            spriteBatch.DrawString(mainFont, "Strikes: ", new Vector2(10, 90), Color.Red);
            spriteBatch.DrawString(mainFont, strikeString, new Vector2(80, 90), Color.White);

            // Development Version 2.0.4
            // Display Current Distance
            // Requirement 4.1.7
            spriteBatch.DrawString(mainFont, "Distance: " + distance.ToString("N2") + "yd", new Vector2(10, 30), Color.Red);

            // Development Version 2.0.4
            // Display Current Multiplier
            // Requirement 4.1.4
            displayCurrentMultiplier(); 
        }

        // Development Version 1.0.1
        // Draw Standard HUD
        // Requirements 4.1.0 and 4.1.8
        public void drawBlackHUD()
        {
            // Development Version 1.0.1
            // Display Current Score
            // Requirement 4.1.1
            spriteBatch.DrawString(mainFont, "Score: " + score, new Vector2(10, 0), Color.Black);

            // Development Version 1.0.1
            // Display Current Level
            // Requirement 4.1.2
            spriteBatch.DrawString(mainFont, "Level " + level, new Vector2(365, 0), Color.Black);

            // Development Version 1.0.1
            // Display Current Streak
            // Requirement 4.1.3
            spriteBatch.DrawString(mainFont, "Streak: " + currentStreak, new Vector2(350, 30), Color.Black);

            // Development Version 1.0.1
            // Display Current Strikes
            // Requirement 4.1.6
            spriteBatch.DrawString(mainFont, "Strikes: ", new Vector2(10, 90), Color.Black);
            spriteBatch.DrawString(mainFont, strikeString, new Vector2(80, 90), Color.Red);

            // Development Version 1.0.1
            // Display Current Distance
            // Requirement 4.1.7
            spriteBatch.DrawString(mainFont, "Distance: " + distance.ToString("N2") + "yd", new Vector2(10, 30), Color.Black);

            // Development Version 1.0.1
            // Display Current Multiplier
            // Requirement 4.1.4
            displayCurrentMultiplier();
        }

        // Development Version 1.0.1
        // Display Current Multiplier in Different Colors Depending on the Current Multiplier.
        // Requirement 4.1.5
        private void displayCurrentMultiplier()
        {
            if (multiplier > 0)
            {
                if (multiplier == 1)
                {
                    spriteBatch.DrawString(mainFont, "Multiplier: x" + multiplier, new Vector2(10, 60), Color.Red);
                }
                else if (multiplier == 2)
                {
                    spriteBatch.DrawString(mainFont, "Multiplier: x" + multiplier, new Vector2(10, 60), Color.Orange);
                }
                else if (multiplier == 4)
                {
                    spriteBatch.DrawString(mainFont, "Multiplier: x" + multiplier, new Vector2(10, 60), Color.Yellow);
                }
                else if (multiplier == 8)
                {
                    spriteBatch.DrawString(mainFont, "Multiplier: x" + multiplier, new Vector2(10, 60), Color.GreenYellow);
                }
                else
                {
                    spriteBatch.DrawString(mainFont, "Multiplier: x" + multiplier, new Vector2(10, 60), Color.Cyan);
                }
            }
        }

        // Development Version 1.0.4
        // Draws Obstacles for the Track
        // Requirement 2.0.0
        private void drawObstacles()
        {
            // Development Version 1.0.4
            // Manage Array With Head and Tail Index Variables
            // Requirement 10.1.2
            if (tailIndex < headIndex)
            {
                // Development Version 1.0.4
                // Iterate Through the Array From Tail Index to Head Index
                // Requirement 10.5.0
                for (int i = tailIndex; i < headIndex; i++)
                {
                    if (currentCharState == CharState.Zombie)
                    {
                        // Development Version 2.0.4
                        // Draw Tombstone Hurdle
                        // Requirements 2.1.0, 2.1.3 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(tombstoneHurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                        
                        // Development Version 2.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                        
                        // Development Version 2.0.4
                        // Draw Bone Overhead Bar
                        // Requirements 2.2.0, 2.2.3 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(bonesOverheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }
                    else
                    {
                        
                        // Development Version 1.0.4
                        // Draw Red Hurdle
                        // Requirements 2.1.0, 2.1.1 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(hurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                        
                        // Development Version 1.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                        
                        // Development Version 1.0.4
                        // Draw Blue Overhead Bar
                        // Requirements 2.2.0, 2.2.1 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(overheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }
                }
            }
            else if (headIndex < tailIndex)
            {
                // Development Version 1.0.4
                // Iterate Through the Array From Tail to Array End and From 0 to Head
                // Requirement 10.5.1
                for (int i = tailIndex; i < obstacles.Length; i++)
                {
                    if (currentCharState == CharState.Zombie)
                    {
                        // Development Version 2.0.4
                        // Draw Tombstone Hurdle
                        // Requirements 2.1.0, 2.1.3 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(tombstoneHurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 2.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 2.0.4
                        // Draw Bone Overhead Bar
                        // Requirements 2.2.0, 2.2.3 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(bonesOverheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }
                    else
                    {

                        // Development Version 1.0.4
                        // Draw Red Hurdle
                        // Requirements 2.1.0, 2.1.1 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(hurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 1.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 1.0.4
                        // Draw Blue Overhead Bar
                        // Requirements 2.2.0, 2.2.1 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(overheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }

                }
                for (int i = 0; i < headIndex; i++)
                {
                    if (currentCharState == CharState.Zombie)
                    {
                        // Development Version 2.0.4
                        // Draw Tombstone Hurdle
                        // Requirements 2.1.0, 2.1.3 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(tombstoneHurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 2.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 2.0.4
                        // Draw Bone Overhead Bar
                        // Requirements 2.2.0, 2.2.3 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(bonesOverheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }
                    else
                    {

                        // Development Version 1.0.4
                        // Draw Red Hurdle
                        // Requirements 2.1.0, 2.1.1 and 10.2.3
                        if (obstacles[i] != null && obstacles[i].getType() == 1)
                            spriteBatch.Draw(hurdle, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 1.0.4
                        // Draw Finish Line
                        // Requirements 2.4.0 and 10.2.4
                        else if (obstacles[i] != null && obstacles[i].getType() == 2)
                            spriteBatch.Draw(finishLine, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);

                        // Development Version 1.0.4
                        // Draw Blue Overhead Bar
                        // Requirements 2.2.0, 2.2.1 and 10.2.2
                        else if (obstacles[i] != null)
                            spriteBatch.Draw(overheadBar, new Rectangle(obstacles[i].getXPosition(), obstacles[i].getYPosition(), obstacles[i].getWidth(), obstacles[i].getHeight()), Color.White);
                    }
                }
            }
        }

        #endregion

        #region Setters & Getters

        // Development Version 2.0.4
        // Set Shoe Color/Zombie Mode
        // Requirements 1.4.0 and 1.5.0
        public void setCharState(int state)
        {
            currentCharState = (CharState)state;
        }

        // Set current level
        public void setLevel(int lvl)
        {
            level = lvl;
        }

        // Get current level
        public int getLevel()
        {
            return level;
        }

        // Set whether level is finished or not
        public void setLevelFinished(bool fin)
        {
            levelFinished = fin;
        }

        // Return whether level is finished or not
        public bool getLevelFinished()
        {
            return levelFinished;
        }

        // Get whether game is finished and going back to main menu
        public bool getGoToMainMenu()
        {
            return mainMenu;
        }
        #endregion

        #region Check Highscores

        // Development Version 3.0.1
        // Checks Player's Score Against Highscores
        // Requirement 12.0.0
        public void updateHighscores()
        {
            // Development Version 3.0.1
            // Read Highscores from Text File
            // Requirement 12.1.0
            StreamReader hsFileReader = new StreamReader("../../../../TheRunnerDataFiles/Highscores.txt");

            // Development Version 3.0.1
            // Declare Variables for the Top 5 Scores
            // Requirement 6.4.0
            string firstScore, secondScore, thirdScore, fourthScore, fifthScore;
            string firstDistance, secondDistance, thirdDistance, fourthDistance, fifthDistance;

            // Development Version 3.0.1
            // Top 5 Highscores are Stored on Separate Lines in Score-Distance Pairs
            // Requirements 12.1.1 and 12.1.2
            string line = hsFileReader.ReadLine();

            firstScore = line;
            line = hsFileReader.ReadLine();
            firstDistance = line;
            line = hsFileReader.ReadLine();
            secondScore = line;
            line = hsFileReader.ReadLine();
            secondDistance = line;
            line = hsFileReader.ReadLine();
            thirdScore = line;
            line = hsFileReader.ReadLine();
            thirdDistance = line;
            line = hsFileReader.ReadLine();
            fourthScore = line;
            line = hsFileReader.ReadLine();
            fourthDistance = line;
            line = hsFileReader.ReadLine();
            fifthScore = line;
            line = hsFileReader.ReadLine();
            fifthDistance = line;

            hsFileReader.Close(); // Close file

            StreamWriter hsFileWriter = new StreamWriter("../../../../TheRunnerDataFiles/Highscores.txt");

            // Development Version 3.0.1
            // Check Player's Score Against the Highscores Starting With the First
            // Requirement 12.2.0
            if (score > Convert.ToInt32(firstScore))
            {
                // Development Version 3.0.1
                // Add New Score and Shift Other Highscores Accordingly
                // Requirement 12.3.0
                fifthScore = fourthScore;
                fifthDistance = fourthDistance;
                fourthScore = thirdScore;
                fourthDistance = thirdDistance;
                thirdScore = secondScore;
                thirdDistance = secondDistance;
                secondScore = firstScore;
                secondDistance = firstDistance;
                firstScore = score.ToString();
                firstDistance = totalDistance.ToString();
            }
            else if (score > Convert.ToInt32(secondScore))
            {
                // Development Version 3.0.1
                // Add New Score and Shift Other Highscores Accordingly
                // Requirement 12.3.0
                fifthScore = fourthScore;
                fifthDistance = fourthDistance;
                fourthScore = thirdScore;
                fourthDistance = thirdDistance;
                thirdScore = secondScore;
                thirdDistance = secondDistance;
                secondScore = score.ToString();
                secondDistance = totalDistance.ToString();
            }
            else if (score > Convert.ToInt32(thirdScore))
            {
                // Development Version 3.0.1
                // Add New Score and Shift Other Highscores Accordingly
                // Requirement 12.3.0
                fifthScore = fourthScore;
                fifthDistance = fourthDistance;
                fourthScore = thirdScore;
                fourthDistance = thirdDistance;
                thirdScore = score.ToString();
                thirdDistance = totalDistance.ToString();
            }
            else if (score > Convert.ToInt32(fourthScore))
            {
                // Development Version 3.0.1
                // Add New Score and Shift Other Highscores Accordingly
                // Requirement 12.3.0
                fifthScore = fourthScore;
                fifthDistance = fourthDistance;
                fourthScore = score.ToString();
                fourthDistance = totalDistance.ToString();
            }
            else if (score > Convert.ToInt32(fifthScore))
            {
                // Development Version 3.0.1
                // Add New Score and Shift Other Highscores Accordingly
                // Requirement 12.3.0
                fifthScore = score.ToString();
                fifthDistance = totalDistance.ToString();
            }

            // Development Version 3.0.1
            // Write Highscores Back to the Text File
            // Requirement 12.4.0
            hsFileWriter.WriteLine(firstScore);
            hsFileWriter.WriteLine(firstDistance);
            hsFileWriter.WriteLine(secondScore);
            hsFileWriter.WriteLine(secondDistance);
            hsFileWriter.WriteLine(thirdScore);
            hsFileWriter.WriteLine(thirdDistance);
            hsFileWriter.WriteLine(fourthScore);
            hsFileWriter.WriteLine(fourthDistance);
            hsFileWriter.WriteLine(fifthScore);
            hsFileWriter.WriteLine(fifthDistance);

            hsFileWriter.Close(); // Close file

        }

        #endregion
    }
}