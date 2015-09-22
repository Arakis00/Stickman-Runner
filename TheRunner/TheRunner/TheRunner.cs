using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TheRunner
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheRunner : Microsoft.Xna.Framework.Game
    {
        #region Global Variables

        // Development Version 1.0.0
        // Declare Graphics and State Variables
        // Requirements 4.0.0 and 4.3.0
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D mainMenuBG, helpMenuBG, highscoreMenuBG;
        Color highscoreTxtColorFirst, highscoreTxtColorMain;
        int screenWidth = 800, screenHeight = 480; // Screen size adjustments
        enum GameState { MainMenu, Playing, HighScore, Help, Customize } // Possible game state values
        enum CharState { Red, Green, Orange, Pink, Blue, Purple, Zombie } // Set in customize menu, possible character states
        GameState currentGameState = GameState.MainMenu; // Used to hold the current game state and initialized as the main menu

        // Development Version 2.0.0
        // Declare Char State Initializing With a Shoe Color of Red
        // Requirement 1.4.1
        CharState currentCharState = CharState.Red;

        // Development Version 2.0.0
        // Declare Level Variables
        // Requirement 5.8.0
        Level levelSystem;
        int level;
        bool levelFinished;

        // Development Version 2.0.1 
        // Declare Menu Button Variables and Customize Underline Variable
        // Requirements 4.3.10 and 13.0.0
        mButton playButton, highScoreButton, helpButton, customizeButton, soundButton; // Main menu buttons
        mButton redButton, greenButton, orangeButton, pinkButton, blueButton, purpleButton, zombieButton; // Customize screen buttons
        mButton backButton; // Back button used in highscore, help, and customize menus
        Rectangle underline; // Customize menu underline location to depict current selection
        
        // Development Version 2.0.3
        // Declare Sound Effect and Music Variables
        // Requirement 7.0.0
        Song titleSong, custSong, helpSong, highScoreSong;
        SoundEffect menuSelection, zombieSelection;

        #endregion

        #region Initializing and Loading Content Pre-GameLoop
        public TheRunner()
        {
            level = 0; //Initialized to 0 (incremented in update function due to levelFinish being returned as true from level class)
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            // Development Version 1.0.3
            // Create a new SpriteBatch for Drawing All Textures and a Font for Highscores
            // Requirements 4.3.7, 10.2.6, 11.1.0 and 13.3.0
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("HighscoresFont");

            // Development Version 1.0.3
            // Set Preferred Screen Ratio/Size
            // Requirement 4.0.0
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges(); //commit above screen changes
            IsMouseVisible = true; //set mouse visible

            // Development Version 2.0.0
            // Load Menu Content
            // Requirements 4.3.0, 4.3.1, 4.3.2, 4.3.3, 4.3.4 and 4.3.6
            loadMainMenu();
            loadHighScoreMenu();
            loadHelpMenu();
            loadCustomizeMenu();
            backButton = new mButton(Content.Load<Texture2D>("Buttons/BackButton"), graphics.GraphicsDevice);
            backButton.setSize(new Vector2(54, 24)); //image size is 54x24 pixels, size is halved
            backButton.setPosition(new Vector2(25, 25));

            // Development Version 2.0.4
            // Add Menu Selection and Zombie Sound Effects to Menus
            // Requirements 7.6.0, 7.6.1 and 7.6.2
            menuSelection = Content.Load<SoundEffect>("Sound/Selection");
            zombieSelection = Content.Load<SoundEffect>("Sound/ZombieSound1");

            // Development Version 3.0.2
            // Create Level System
            // Requirement 5.8.0
            levelSystem = new Level(graphics, Services, spriteBatch, this);
        }
        
        //load the main menu objects
        public void loadMainMenu()
        {
            // Development Version 2.0.1
            // Set Main Menu Background
            // Requirement 4.3.8
            mainMenuBG = Content.Load<Texture2D>("TitleScreen/TitleBackground");

            // Development 2.0.1
            // Create Main Menu Buttons and Set Their Size and Position
            // Requirements 4.3.1, 4.3.2, 4.3.3, 4.3.4 and 4.3.5
            playButton = new mButton(Content.Load<Texture2D>("TitleScreen/TitlePlay"), graphics.GraphicsDevice);
            highScoreButton = new mButton(Content.Load<Texture2D>("TitleScreen/TitleHighscores"), graphics.GraphicsDevice);
            helpButton = new mButton(Content.Load<Texture2D>("TitleScreen/TitleHelp"), graphics.GraphicsDevice);
            customizeButton = new mButton(Content.Load<Texture2D>("TitleScreen/TitleCustomize"), graphics.GraphicsDevice);
            soundButton = new mButton(Content.Load<Texture2D>("Buttons/SoundButton"), graphics.GraphicsDevice);
            // Set size of main menu buttons
            playButton.setSize(new Vector2(71, 39)); // Image size is 71x39 pixels
            highScoreButton.setSize(new Vector2(177, 42)); // image size is 177x42 pixels
            helpButton.setSize(new Vector2(64, 39)); // Image size is 64x39 pixels
            customizeButton.setSize(new Vector2(165, 32)); // Image size is 165x32 pixels
            soundButton.setSize(new Vector2(115 / 2, 119 / 2)); // Image size is 115x119 pixels, size is halved
            // Set position of main menu buttons
            playButton.setPosition(new Vector2(screenWidth / 2 - playButton.size.X / 2, (screenHeight / 5) + 5));
            highScoreButton.setPosition(new Vector2(screenWidth / 2 - highScoreButton.size.X / 2, (screenHeight / 5) + 65));
            helpButton.setPosition(new Vector2(screenWidth / 2 - helpButton.size.X / 2, (screenHeight / 5) + 125));
            customizeButton.setPosition(new Vector2(screenWidth / 2 - customizeButton.size.X / 2, (screenHeight / 5) + 185));
            soundButton.setPosition(new Vector2(screenWidth - soundButton.size.X - 25, 25));

            // Development Version 2.0.4
            // Main Menu Music
            // Requirement 7.1.0
            titleSong = Content.Load<Song>("Sound/TitleMusic");
        }

        //load the high score menu objects
        public void loadHighScoreMenu()
        {
            // Development Version 3.0.0
            // Set Highscores Menu Background
            // Requirement 4.3.8
            highscoreMenuBG = Content.Load<Texture2D>("HighscoreScreen/HighscoresBackground");
            highscoreTxtColorFirst = Color.White;
            highscoreTxtColorMain = Color.Black;

            // Development Version 2.0.4
            // Highscore Menu Music
            // Requirement 7.1.0
            highScoreSong = Content.Load<Song>("Sound/HighScoreMusic");
        }

        //load the help menu objects
        public void loadHelpMenu()
        {
            // Development Version 2.0.1
            // Set Help Menu Background
            // Requirement 4.3.8
            helpMenuBG = Content.Load<Texture2D>("HelpScreen/HelpBackground");

            // Development Version 2.0.4
            // Help Menu Music
            // Requirement 7.1.0
            helpSong = Content.Load<Song>("Sound/HelpMusic");
        }

        //load the customize menu objects
        public void loadCustomizeMenu()
        {
            // Development Version 2.0.2
            // Set Customize Menu Background
            // Requirement 4.3.8
            custSong = Content.Load<Song>("Sound/CustomizeMusic");

            // Development 2.0.2
            // Create Customized Menu Buttons and Set Their Size and Position
            // Requirement 4.3.11
            redButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunRedThumb"), graphics.GraphicsDevice);
            greenButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunGreenThumb"), graphics.GraphicsDevice);
            orangeButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunOrangeThumb"), graphics.GraphicsDevice);
            pinkButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunPinkThumb"), graphics.GraphicsDevice);
            blueButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunBlueThumb"), graphics.GraphicsDevice);
            purpleButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunPurpleThumb"), graphics.GraphicsDevice);
            zombieButton = new mButton(Content.Load<Texture2D>("CustomizeScreen/RunZombieThumb"), graphics.GraphicsDevice);
            // Set size of customize menu buttons (approximate background destionation box sizes)
            redButton.setSize(new Vector2(200, 200));
            greenButton.setSize(new Vector2(100, 100));
            orangeButton.setSize(new Vector2(100, 100));
            pinkButton.setSize(new Vector2(100, 100));
            blueButton.setSize(new Vector2(100, 100));
            purpleButton.setSize(new Vector2(100, 100));
            zombieButton.setSize(new Vector2(100, 100));
            // Set position of customize menu buttons (background destination box locations)
            redButton.setPosition(new Vector2(300, 158));
            greenButton.setPosition(new Vector2(95, 125));
            orangeButton.setPosition(new Vector2(20, 280));
            pinkButton.setPosition(new Vector2(170, 280));
            blueButton.setPosition(new Vector2(605, 125));
            purpleButton.setPosition(new Vector2(530, 280));
            zombieButton.setPosition(new Vector2(678, 280));

            // Development 2.0.2
            // Create Selection Underline for Customize Screen and Default Choice to Red
            // Requirements 1.4.1 and 4.3.10
            underline = new Rectangle(327, 405, 150, 3);
        }
        
        #endregion

        #region Unload Content in GameLoop (Not Used)
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Update Logic in GameLoop
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// Development Version 1.0.1
        /// Update Game Logic
        /// Requirement 14.0.0
        protected override void Update(GameTime gameTime)
        {
            // Development Version 2.0.0
            // Update Mouse and Keyboard States
            // Requirement 15.1.0
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            // Development Version 2.0.7
            // Allows the Application to Close if the ESC Key is Pressed
            // Requirement 5.12.0
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            // Development Version 2.0.5
            // Determine What State the Game is in
            // Requirements 4.3.0, 4.3.1, 4.3.2, 4.3.3 and 4.3.4
            switch (currentGameState)
            {
                case (GameState.MainMenu):
                    updateMainMenu(mouse, keyboard);
                    break;
                case (GameState.HighScore):
                    updateHighScoreMenu(mouse, keyboard);
                    break;
                case (GameState.Help):
                    updateHelpMenu(mouse, keyboard);
                    break;
                case (GameState.Customize):
                    updateCustomizeMenu(mouse, keyboard);
                    break;
                case (GameState.Playing):
                    updateGameplay(gameTime, mouse, keyboard);
                    break;
            }

            base.Update(gameTime);
        }

        // Development Version 2.0.0
        // Check for Logic Updates Within the Main Menu
        // Requirement 14.3.0
        public void updateMainMenu(MouseState mouse, KeyboardState keyboard)
        {
            // Development Version 2.0.4
            // Play Main Menu Music if Not Already Playing
            // Requirement 7.2.0
            if (MediaPlayer.State.Equals(MediaState.Stopped))
                MediaPlayer.Play(titleSong);

            // Development Version 2.0.0
            // Start Game if Play Button is Clicked
            // Requirement 4.3.1
            if (playButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                // Development Version 2.0.0
                // Set Shoe Color or Zombie Mode
                // Requirements 1.4.0 and 1.5.0
                levelSystem.setCharState((int)currentCharState);

                currentGameState = GameState.Playing; // Initiate new game state
                MediaPlayer.Stop(); // Stop menu music
            }
            // Development Version 3.0.0
            // Launch Highscores Screen When Highscores Button is Clicked
            // Requirement 4.3.2
            else if (highScoreButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.HighScore;
                MediaPlayer.Stop(); // Stop music
            }
            // Development Version 3.0.0
            // Launch Help Screen When Help Button is Clicked
            // Requirement 4.3.3
            else if (helpButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.Help;
                MediaPlayer.Stop(); // Stop music
            }
            // Development Version 2.0.2
            // Launch Customize Screen When Customize Button is Clicked
            // Requirement 4.3.4
            else if (customizeButton.isClicked == true)  //go to customize menu if customize button is clicked
            {
                //assign normal back button image upon entering the customization menu
                backButton.changeImage(Content.Load<Texture2D>("Buttons/BackButton"));

                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.Customize;
                MediaPlayer.Stop(); // Stop music
            }
            // Development Version 2.0.4
            // Mute/Unmute Game on Sound Button Click
            // Requirement 4.3.5
            else if (soundButton.isClicked == true)
            {
                if (MediaPlayer.IsMuted == false) // Mute
                {
                    MediaPlayer.IsMuted = true;
                    soundButton.changeImage(Content.Load<Texture2D>("Buttons/SoundButtonMute"));
                }
                else // Unmute
                {
                    MediaPlayer.IsMuted = false;
                    soundButton.changeImage(Content.Load<Texture2D>("Buttons/SoundButton"));
                }
            }

            // Development Version 2.0.1
            // Update Mouse Status on Buttons
            // Requirement 14.3.1
            playButton.Update(mouse);
            highScoreButton.Update(mouse);
            helpButton.Update(mouse);
            customizeButton.Update(mouse);
            soundButton.Update(mouse);
            backButton.Update(mouse);
        }

        // Development Version 3.0.1
        // Check for Logic Updates Within the Highscore Menu
        // Requirement 14.3.0
        public void updateHighScoreMenu(MouseState mouse, KeyboardState keyboard)
        {
            // Development Version 3.0.1
            // Play Highscore Music if Not Already Playing
            // Requirement 7.2.0
            if (MediaPlayer.State.Equals(MediaState.Stopped))
                MediaPlayer.Play(highScoreSong);

            // Development Version 3.0.1
            // Go Back to Main Menu When Back Button is Clicked
            // Requirement 4.3.6
            if (backButton.isClicked == true) // Go back to main menu if clicked
            {
                // Development Version 3.0.1
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.MainMenu;
                MediaPlayer.Stop(); // Stop music
            }

            // Development Version 3.0.1
            // Update Mouse Status on Buttons
            // Requirement 14.3.1
            backButton.Update(mouse);
        }

        // Development Version 3.0.2
        // Check for Logic Updates Within the Help Menu
        // Requirement 14.3.0
        public void updateHelpMenu(MouseState mouse, KeyboardState keyboard)
        {
            // Development Version 3.0.2
            // Play Help Music if Not Already Playing
            // Requirement 7.2.0
            if (MediaPlayer.State.Equals(MediaState.Stopped))
                MediaPlayer.Play(helpSong);

            // Development Version 3.0.2
            // Go Back to Main Menu When Back Button is Clicked
            // Requirement 4.3.6
            if (backButton.isClicked == true)  //go back to main menu if clicked
            {
                // Development Version 3.0.2
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.MainMenu;
                MediaPlayer.Stop(); // Stop music
            }

            // Development Version 3.0.2
            // Update Mouse Status on Buttons
            // Requirement 14.3.1
            backButton.Update(mouse);
        }

        // Development Version 2.0.2
        // Check for Logic Updates Within the Customize Menu
        // Requirement 14.3.0
        public void updateCustomizeMenu(MouseState mouse, KeyboardState keyboard)
        {
            // Development Version 2.0.4
            // Play Customize Music if Not Already Playing
            // Requirement 7.2.0
            if (MediaPlayer.State.Equals(MediaState.Stopped))
                MediaPlayer.Play(custSong);

            // Development Version 2.0.2
            // Go Back to Main Menu When Back Button is Clicked
            // Requirement 4.3.6
            if (backButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) menuSelection.Play();

                currentGameState = GameState.MainMenu;
                MediaPlayer.Stop(); // Stop music

                // Development Version 2.0.5
                // Check for Zombie Mode and Set the Appropriate Backgrounds/Buttons
                // Requirement 4.3.9
                // DEVELOPER NOTE 1: If you add additional zombie menus, be sure to create a texture2D 
                //        variable for the background and set it in the initialize section for that menu.
                // DEVELOPER NOTE 2: Also to be sure to change the image paths used in the draw method 
                //        to the newly created variable.
                if (currentCharState == CharState.Zombie)
                {
                    // Change main menu content
                    helpMenuBG = Content.Load<Texture2D>("HelpScreen/HelpBackgroundZombieMode");
                    backButton.changeImage(Content.Load<Texture2D>("Buttons/BackButtonZombie"));
                    highscoreMenuBG = Content.Load<Texture2D>("HighscoreScreen/HighscoresBackgroundZombieMode");
                    highscoreTxtColorFirst = Color.Red;
                    highscoreTxtColorMain = Color.White;
                    mainMenuBG = Content.Load<Texture2D>("TitleScreen/TitleBackgroundZombieMode");
                    playButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitlePlayZombieMode"));
                    highScoreButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleHighscoresZombieMode"));
                    helpButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleHelpZombieMode"));
                    customizeButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleCustomizeZombieMode"));
                }
                // Development Version 2.0.5
                // Change Content Back to Normal Backgrounds/Buttons if no Zombie Mode
                // Requirement 4.3.8
                else
                {
                    // Change back main menu content
                    helpMenuBG = Content.Load<Texture2D>("HelpScreen/HelpBackground");
                    backButton.changeImage(Content.Load<Texture2D>("Buttons/BackButton"));
                    highscoreMenuBG = Content.Load<Texture2D>("HighscoreScreen/HighscoresBackground");
                    highscoreTxtColorFirst = Color.White;
                    highscoreTxtColorMain = Color.Black;
                    mainMenuBG = Content.Load<Texture2D>("TitleScreen/TitleBackground");
                    playButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitlePlay"));
                    highScoreButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleHighscores"));
                    helpButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleHelp"));
                    customizeButton.changeImage(Content.Load<Texture2D>("TitleScreen/TitleCustomize"));
                }
                // Development Version 2.0.4
                // Load Main Menu Music
                // Requirement 7.1.0
                titleSong = Content.Load<Song>("Sound/TitleMusic");
            }
            // Development Version 2.0.2
            // Red Shoe Button is Selected
            // Requirement 4.3.11
            else if (redButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Red
                // Requirement 1.4.1
                currentCharState = CharState.Red;

                // Development Version 2.0.2
                // Set Underline Under Red Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(327, 405, 150, 3);
            }
            // Development Version 2.0.2
            // Green Shoe Button is Selected
            // Requirement 4.3.11
            else if (greenButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Green
                // Requirement 1.4.5
                currentCharState = CharState.Green;

                // Development Version 2.0.2
                // Set Underline Under Green Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(120, 255, 50, 3); 
            }
            // Development Version 2.0.2
            // Orange Shoe Button is Selected
            // Requirement 4.3.11
            else if (orangeButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Orange
                // Requirement 1.4.4
                currentCharState = CharState.Orange;

                // Development Version 2.0.2
                // Set Underline Under Orange Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(45, 410, 50, 3); 
            }
            // Development Version 2.0.2
            // Pink Shoe Button is Selected
            // Requirement 4.3.11
            else if (pinkButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Pink
                // Requirement 1.4.3
                currentCharState = CharState.Pink;

                // Development Version 2.0.2
                // Set Underline Under Pink Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(195, 410, 50, 3); 
            }
            // Development Version 2.0.2
            // Blue Shoe Button is Selected
            // Requirement 4.3.11
            else if (blueButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Blue
                // Requirement 1.4.5
                currentCharState = CharState.Blue;

                // Development Version 2.0.2
                // Set Underline Under Blue Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(630, 255, 50, 3); 
            }
            // Development Version 2.0.2
            // Purple Shoe Button is Selected
            // Requirement 4.3.11
            else if (purpleButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Menu Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.1
                if (!MediaPlayer.IsMuted) 
                    menuSelection.Play();

                // Development Version 2.0.2
                // Set Shoe Color to Purple
                // Requirement 1.4.5
                currentCharState = CharState.Purple;

                // Development Version 2.0.2
                // Set Underline Under Purple Shoe Choice
                // Requirement 4.3.10
                underline = new Rectangle(555, 410, 50, 3); 
            }
            // Development Version 2.0.3
            // Zombie Mode Button is Selected
            // Requirement 4.3.11
            else if (zombieButton.isClicked == true)
            {
                // Development Version 2.0.4
                // Play Zombie Selection Sound Effect on Click if Not Muted
                // Requirement 7.6.2
                if (!MediaPlayer.IsMuted) 
                    zombieSelection.Play();

                // Development Version 2.0.3
                // Set to Zombie Mode
                // Requirement 1.5.0
                currentCharState = CharState.Zombie;

                // Development Version 2.0.3
                // Set Underline Under Zombie Choice
                // Requirement 4.3.10
                underline = new Rectangle(705, 410, 50, 3);
            }

            // Development Version 2.0.3
            // Update Mouse Status on Buttons
            // Requirement 14.3.1
            backButton.Update(mouse);
            redButton.Update(mouse);
            greenButton.Update(mouse);
            orangeButton.Update(mouse);
            pinkButton.Update(mouse);
            blueButton.Update(mouse);
            purpleButton.Update(mouse);
            zombieButton.Update(mouse);
        }

        // Development Version 2.0.6
        // Update Gameplay Logic
        // Requirement 14.0.0
        public void updateGameplay(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
        {
            // Development Version 2.0.6
            // See if Level is Finished and Take Appropriate Actions
            // Requirement 5.11.0
            levelFinished = levelSystem.getLevelFinished();

            // Development Version 2.0.6
            // Increment Level and Load Next Level if the Level is Finished
            // Requirement 5.11.3
            if (levelFinished)
            {
                MediaPlayer.Stop(); // Stop music
                level++;

                // Development Version 2.0.6
                // Load Next Level
                // Requirement 9.1.0
                levelSystem.loadLevel(level);
                levelSystem.loadObstacles();
                levelSystem.setLevelFinished(false); // Set level to not finished since new level was just loaded
            }

            // Development Version 2.0.6
            // Handles Updates to Level Game Logic
            // Requirement 14.0.0
            levelSystem.updateLogic(gameTime);

            // Development Version 2.0.6
            // Reset Game if the Level System is Saying it's Finished
            // Requirement 16.0.1
            if (levelSystem.getGoToMainMenu())
                resetGame();
        }

        // Development Version 1.0.2
        // Reset Game
        // Requirement 16.0.0
        public void resetGame()
        {
            currentGameState = GameState.MainMenu;
            level = 0; // Set level back to 0
            levelSystem = new Level(graphics, Services, spriteBatch, this);// Create level system
        }
        
        #endregion

        #region Drawing Logic in GameLoop
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        // Development Version 1.0.0
        // Main Draw Controller for Game Window
        // Requirement 4.0.0
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White); // Clear screen
            spriteBatch.Begin();

            switch (currentGameState) // Determine which game state to draw
            {
                // Development Version 2.0.1
                // Draw Main Menu
                // Requirement 4.3.0
                case (GameState.MainMenu):
                    spriteBatch.Draw(mainMenuBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    playButton.Draw(spriteBatch);
                    highScoreButton.Draw(spriteBatch);
                    helpButton.Draw(spriteBatch);
                    customizeButton.Draw(spriteBatch);
                    soundButton.Draw(spriteBatch);
                    break;
                // Development Version 3.0.1
                // Draw Highscore Screen
                // Requirement 4.3.2
                case (GameState.HighScore):

                    // Development Version 3.0.1
                    // Read Highscores From Text File
                    // Requirement 12.1.0
                    StreamReader sr = new StreamReader("../../../../TheRunnerDataFiles/Highscores.txt");
                    
                    spriteBatch.Draw(highscoreMenuBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                    // Development Version 3.0.1
                    // Format Distance to 2 Decimal Places and Then Draw Each Score and Distance to Screen
                    // Requirement 12.5.0
                    spriteBatch.DrawString(font, sr.ReadLine(), new Vector2(170, 100), highscoreTxtColorFirst);
                    double tempDistance = Convert.ToDouble(sr.ReadLine());
                    spriteBatch.DrawString(font, tempDistance.ToString("N2") + " yd.", new Vector2(550, 100), highscoreTxtColorFirst);

                    spriteBatch.DrawString(font, sr.ReadLine(), new Vector2(170, 140), highscoreTxtColorMain);
                    tempDistance = Convert.ToDouble(sr.ReadLine());
                    spriteBatch.DrawString(font, tempDistance.ToString("N2") + " yd.", new Vector2(550, 140), highscoreTxtColorMain);

                    spriteBatch.DrawString(font, sr.ReadLine(), new Vector2(170, 180), highscoreTxtColorMain);
                    tempDistance = Convert.ToDouble(sr.ReadLine());
                    spriteBatch.DrawString(font, tempDistance.ToString("N2") + " yd.", new Vector2(550, 180), highscoreTxtColorMain);

                    spriteBatch.DrawString(font, sr.ReadLine(), new Vector2(170, 220), highscoreTxtColorMain);
                    tempDistance = Convert.ToDouble(sr.ReadLine());
                    spriteBatch.DrawString(font, tempDistance.ToString("N2") + " yd.", new Vector2(550, 220), highscoreTxtColorMain);

                    spriteBatch.DrawString(font, sr.ReadLine(), new Vector2(170, 260), highscoreTxtColorMain);
                    tempDistance = Convert.ToDouble(sr.ReadLine());
                    spriteBatch.DrawString(font, tempDistance.ToString("N2") + " yd.", new Vector2(550, 260), highscoreTxtColorMain);

                    backButton.Draw(spriteBatch);
                    break;
                // Development Version 3.0.1
                // Draw Help Screen
                // Requirement 4.3.3
                case (GameState.Help):
                    spriteBatch.Draw(helpMenuBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    backButton.Draw(spriteBatch);
                    break;
                // Development Version 2.0.2
                // Draw Customize Screen
                // Requirement 4.3.4
                case (GameState.Customize):
                    spriteBatch.Draw(Content.Load<Texture2D>("CustomizeScreen/CustomizeMenu"), new Rectangle(0, 0, 800, 431), Color.White); //431x800 is image size
                    backButton.Draw(spriteBatch);
                    redButton.Draw(spriteBatch);
                    greenButton.Draw(spriteBatch);
                    orangeButton.Draw(spriteBatch);
                    pinkButton.Draw(spriteBatch);
                    blueButton.Draw(spriteBatch);
                    purpleButton.Draw(spriteBatch);
                    zombieButton.Draw(spriteBatch);
                    spriteBatch.Draw(Content.Load<Texture2D>("CustomizeScreen/underline"), underline, Color.Black);
                    break;
                // Development Version 2.0.1
                // Draw Gameplay
                // Requirement 4.3.1
                case (GameState.Playing): //draw gameplay content
                    levelSystem.Draw(gameTime);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        #endregion
    }
}
