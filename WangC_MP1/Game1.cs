//Author: Colin Wang
//File Name: Game1.cs
//Project Name: MP1 a Flappy Bird Clone With Classes
//Created Date: March 4, 2024
//Modified Date: March 10, 2024
//Description: Clone of Flappy Bird a mobile game with file i/o stats saving, implementations of simple data structures, and OOP classes


using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace WangC_MP1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // screen dimensions
        const int SCREEN_WIDTH = 576;
        const int SCREEN_HEIGHT = 1024;

        // game states
        const int STATS_STATE = 0;
        const int MENU_STATE = 1;
        const int PRE_GAME_PLAY_STATE = 2;
        const int GAME_PLAY_STATE = 3;
        const int PRE_GAME_OVER_STATE = 4;
        const int GAME_OVER_STATE = 5;

        // button indexes and amount
        const int BTN_COUNT = 3;
        const int START_BTN = 0;
        const int STATS_BTN = 1;
        const int MENU_BTN = 2;

        // y value of the title height
        const int TITLE_Y = 200;

        // value of the spacer between title and bird
        const int TITLE_SPACER = 20;

        // title wave constants
        const int TITLE_WAVE_TIME = 500; // time per one half a period of the title/bird on the title screen
        const int TITLE_WAVE_AMP = 10; // amplitude (horizontal displace) from TITLE_Y

        // button y value on screen
        const int BTN_Y = 670;

        // amount of ground images used
        const int GRD_AMOUNT = 2;

        // ground y position
        const int GRD_Y = 800;

        // bird should be at 130 during pre game play and game play
        const int BIRD_X = 130;

        public const float SCRL_SPD = 3.5f;

        // fade time and opacity constants
        const int FADE_STATE_TIME = 800; // 1500 ms to fade out and 1500 to fade out
        const int FADE_DEATH_TIME = 200;
        const float FULL_OPACITY = 1f;
        const float EMPTY_OPACITY = 0f;

        // score variables
        const int SCOR_RST = 0; // score reset position

        // x and y positions of stats and average stats
        const int PLAYS_Y = 325;
        const int FLAPS_Y = 375;
        const int AVG_SCOR_Y = 445;
        const int AVG_FLAPS_Y = 495;
        const int STATS_SCOR_X = 350;

        // y position of the stats box
        const int STAT_BOX_Y = 240;

        // num of scores counted in stats
        const int TOP_SCORS = 5;

        // x position of top 5 scores
        const int TOP_SCORS_X = 480;
        const int TOP_SCORS_Y = 325;
        const int TOP_SCORS_Y_SPC = 50;

        // y location of the score counter in pre/game play
        const int CUR_SCR_Y = 50;

        // spacer between large numbers
        const int NUM_SPC_FACTOR = 4;

        // length of death timer
        const int DEATH_TIMER = 500;

        // spacer between game over title and result box
        const int GAME_OVR_RSLT_SPC_Y = 10;

        // timer for how long it takes for game over title to fade in (ms)
        const int GAME_OVR_FADE_TIME = 1000;
        const int GAME_OVR_WAVE_AMP = 100;
        const int GAME_OVR_WAVE_DISPL = 216;

        // speed of the result box
        const int RSLT_BOX_SPD = -130;

        // positions of current and best score on result box
        const int RSLT_SCOR_X = 470;
        const int RSLT_CUR_SCOR_Y = 460;
        const int RSLT_HIGH_SCOR_Y = 543; // guess


        // reset and final location of the menu button
        const int MENU_BTN_Y_RST = SCREEN_HEIGHT;
        const int MENU_BTN_Y_FIN = BTN_Y;

        // time it takes for menu to move up (ms)
        const int MENU_POP_UP_TIME = 300;

        // new best tag location
        const int NEW_BEST_X = 326;
        const int NEW_BEST_Y = 505;

        // timer for counting up score in game over screen (ms)
        const int CNT_DELAY = 50;

        // coin count and index
        const int COINS_COUNT = 4;
        const int BRONZE_COIN = 0;
        const int SILVER_COIN = 1;
        const int GOLD_COIN = 2;
        const int PLAT_COIN = 3;

        // scores requirements for their respective scores
        const int BRONZE_SCR = 10;
        const int SILVER_SCR = 20;
        const int GOLD_SCR = 30;
        const int PLAT_SCR = 40;

        // location of the medals relative to the top of the results box
        const int COIN_OFFSET_Y = 86;
        const int COIN_OFFSET_X = 53;

        // stats file path, and blank template of stats
        const string STATS_FILE_PATH = "stats.txt";

        // length of spark length
        const int SPARK_DUR = 500;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // create a instance of the random function
        Random rand = new Random();

        // initialize game state variable, and set to TITLE SCREEN
        int gameState = MENU_STATE;
        int nxtGameState;

        // initialize mouse variables
        MouseState mouse;
        MouseState prevMouse;

        // initialize keyboard variables
        KeyboardState prevKb;
        KeyboardState kb;

        // initialize background variables
        Vector2 bgLoc;

        // initialize title variables
        Vector2 titleLoc;
        Vector2 titleLocRst;

        // initialize bird variables
        Bird bird;

        // bird death variables
        Timer birdDeathTimer = new Timer(DEATH_TIMER, false);

        // initialize button variables
        Rectangle[] btnRecs = new Rectangle[BTN_COUNT];

        // initialize ground variables
        Vector2[] grdLocs = new Vector2[GRD_AMOUNT];
        Rectangle[] grdRecs = new Rectangle[GRD_AMOUNT];

        // initialize fader variables
        Rectangle scrnRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT); // rectangle of fade is size of screen
        Timer fadeCurTime = new Timer(FADE_STATE_TIME, false); // set to fade state time for now
        float fadeOpacity = FULL_OPACITY; // fade opacity, set the opacity to 100

        // tells weather flashed reached half way
        bool fadedHalf = false;

        // initialize score variables
        int curScore = SCOR_RST;
        int bestScore = SCOR_RST;
        bool isNewBest = false;
        Vector2 curScoreLoc = new Vector2(SCREEN_WIDTH / 2, CUR_SCR_Y);


        // initialize positions of each stat on the stats box
        Vector2 playsLoc = new Vector2(STATS_SCOR_X, PLAYS_Y);
        Vector2 flapsLoc = new Vector2(STATS_SCOR_X, FLAPS_Y);
        Vector2 avgScorsLoc = new Vector2(STATS_SCOR_X, AVG_SCOR_Y);
        Vector2 avgFlapsLoc = new Vector2(STATS_SCOR_X, AVG_FLAPS_Y);
        Vector2 topScorsLoc = new Vector2(TOP_SCORS_X, TOP_SCORS_Y);

        // initialize stats page variables
        Vector2 statsBoxLoc;

        // initialize click instructions variables
        Vector2 intrcuLoc;

        // initialize get ready title variables
        Vector2 readyTitleLoc;

        // initialize game over title variables
        Vector2 gameOvrLoc;
        Vector2 gameOvrLocFin;
        Timer gameOvrFade = new Timer(GAME_OVR_FADE_TIME, false);

        // initialize game over title variables
        Vector2 rsltBoxLoc;
        Vector2 rsltBoxLocRst;
        Vector2 rsltBoxLocFin;
        Vector2 rsltcurScoreLoc = new Vector2(RSLT_SCOR_X, RSLT_CUR_SCOR_Y);
        Vector2 rsltHighScorLoc = new Vector2(RSLT_SCOR_X, RSLT_HIGH_SCOR_Y);

        // initialize result score variables
        int rsltScore = SCOR_RST;
        Timer cntDelay = new Timer(CNT_DELAY, false);

        // initialize medals
        Vector2 coinLoc = new Vector2();
        Vector2 coinSize;
        int coinRadius;
        Vector2 coinOrigin;

        // initialize the time it takes to move the menu button up
        Timer menuUpTime = new Timer(MENU_POP_UP_TIME, false);

        // new best tag
        Vector2 newBestLoc = new Vector2(NEW_BEST_X, NEW_BEST_Y);

        // sparkle variables
        Animation sparkAnim;
        Vector2 sparkLoc;

        // pipe manger class
        PipeManager pipesManager;

        ScoreData scoreData;

        public Game1()
        {
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
            // set screen dimensions to the defined width and height
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            // apply the graphics change
            graphics.ApplyChanges();

            // set mouse to visible
            IsMouseVisible = true;

            scoreData = new ScoreData(STATS_FILE_PATH);

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

            Assets.Content = Content;
            Assets.Initialize();

            // load background image and initialize position
            bgLoc = Vector2.Zero;


            // store button rectangles
            // the start and stats button should be such that the remaining space is evenly spaced in thirds
            btnRecs[START_BTN] = new Rectangle((SCREEN_WIDTH - 2 * Assets.startBtnImg.Width) / 3, BTN_Y, Assets.startBtnImg.Width, Assets.startBtnImg.Height);
            btnRecs[STATS_BTN] = new Rectangle(SCREEN_WIDTH - ((SCREEN_WIDTH - 2 * Assets.statsBtnImg.Width) / 3) - Assets.statsBtnImg.Width, BTN_Y, Assets.statsBtnImg.Width, Assets.statsBtnImg.Height);
            btnRecs[MENU_BTN] = new Rectangle((SCREEN_WIDTH / 2) - Assets.menuBtnImg.Width / 2, BTN_Y, Assets.menuBtnImg.Width, Assets.menuBtnImg.Height);

            // rectangle variables data
            // note, able to initialize each index independently as it is known only 2 ground images are
            grdLocs[0] = new Vector2(0, GRD_Y);
            grdRecs[0] = new Rectangle((int)grdLocs[0].X, (int)grdLocs[0].Y, Assets.grdImg.Width, Assets.grdImg.Height);
            grdLocs[1] = new Vector2(Assets.grdImg.Width, GRD_Y);
            grdRecs[1] = new Rectangle((int)grdLocs[1].X, (int)grdLocs[1].Y, Assets.grdImg.Width, Assets.grdImg.Height);


            // load bird animation sheet and class
            bird = new Bird(spriteBatch,
                new Animation(Assets.birdImg, 4, 1, 4, 0, 1, Animation.ANIMATE_FOREVER, Bird.ANIM_TITLE_DUR, Vector2.Zero, true));


            // store the reset title position & bird position and rectangle
            titleLocRst = new Vector2((SCREEN_WIDTH - Assets.titleImg.Width - bird.GetRec().Width - TITLE_SPACER) / 2, TITLE_Y);
            titleLoc = titleLocRst;

            bird.SetRstLoc(new Vector2(titleLocRst.X + Assets.titleImg.Width + TITLE_SPACER, TITLE_Y + Assets.titleImg.Height / 2 - bird.GetRec().Height / 2),
                            new Vector2(BIRD_X, GRD_Y / 2));

            pipesManager = new PipeManager(spriteBatch);


            // load the stats box image and store the position of it
            statsBoxLoc = new Vector2(SCREEN_WIDTH / 2 - Assets.statsBoxImg.Width / 2, STAT_BOX_Y);

            // load click instruction image and store it's position
            intrcuLoc = new Vector2(SCREEN_WIDTH / 2 - Assets.instrucImg.Width / 2, GRD_Y / 2);

            // load click instruction image and store it's position
            readyTitleLoc = new Vector2(SCREEN_WIDTH / 2 - Assets.getReadyImg.Width / 2, SCREEN_HEIGHT / 3 - Assets.getReadyImg.Height / 2); // horizontally centered and 1/3 of the way down from top to ground

            // load game over title image
            gameOvrLocFin = new Vector2(SCREEN_WIDTH / 2 - Assets.gameOvrImg.Width / 2, SCREEN_HEIGHT / 3 - Assets.gameOvrImg.Height / 2);
            gameOvrLoc = gameOvrLocFin;

            // load result box images
            rsltBoxLocRst = new Vector2(SCREEN_WIDTH / 2 - Assets.rsltsBoxImg.Width / 2, SCREEN_HEIGHT);
            rsltBoxLocFin = new Vector2(rsltBoxLocRst.X, gameOvrLoc.Y + Assets.gameOvrImg.Height + GAME_OVR_RSLT_SPC_Y);
            rsltBoxLoc = rsltBoxLocFin;

            // load the coin images and initialize all other coin variables
            coinLoc = new Vector2(rsltBoxLocFin.X + COIN_OFFSET_X, rsltBoxLocFin.Y + COIN_OFFSET_Y);
            coinSize = new Vector2(Assets.coinsImg.Width / COINS_COUNT, Assets.coinsImg.Height);
            coinRadius = (int)coinSize.X / 2;
            coinOrigin = new Vector2(coinLoc.X + coinSize.X / 2, coinLoc.Y + coinSize.Y / 2);

            // load sparkle image
            sparkAnim = new Animation(Assets.sparkleImg, 5, 1, 5, 1, Animation.NO_IDLE, Animation.ANIMATE_ONCE, SPARK_DUR, sparkLoc, false);

            //Set sound options
            MediaPlayer.IsRepeating = true; //Sets it to play forever
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // update mouse variables
            prevMouse = mouse;
            mouse = Mouse.GetState();

            // update keyboard variables
            prevKb = kb;
            kb = Keyboard.GetState();

            // check if the music is playing if not play it
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(Assets.bgMusic);
            }

            // update the current game state
            switch (gameState)
            {
                case STATS_STATE:
                    UpdateStats(gameTime);
                    break;

                case MENU_STATE:
                    // update title screen
                    UpdateMenu(gameTime);
                    break;

                case PRE_GAME_PLAY_STATE:
                    // update pre game play screen
                    UpdatePreGame(gameTime);
                    break;

                case GAME_PLAY_STATE:
                    // Update game play
                    UpdateGamePlay(gameTime);
                    break;

                case PRE_GAME_OVER_STATE:
                    // update pre game over
                    UpdatePreGameOvr(gameTime);
                    break;

                case GAME_OVER_STATE:
                    // update game over screen
                    UpdateGameOvr(gameTime);
                    break;
            }


            base.Update(gameTime);
        }

        // method to update stats state
        private void UpdateStats(GameTime gameTime)
        {
            // update ground scrolling
            UpdateGround();

            // check if there is no fade
            if (fadeCurTime.IsFinished() || fadeCurTime.IsInactive())
            {
                // check if player clicks the menu button or preses space
                if (IsClickedOn(btnRecs[MENU_BTN]) || IsKeyPressed(Keys.Space))
                {
                    // reset the fade time and activate it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(Assets.fadeSnd);

                    // set next game state to MENU
                    nxtGameState = MENU_STATE;
                }
            }

            // if fade timer is on update fade
            if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME, nxtGameState);
            }
        }

        private void UpdateMenu(GameTime gameTime)
        {
            bird.Update(gameTime, IsClick() || IsKeyPressed(Keys.Space));

            // move title and bird up and down
            titleLoc.Y = TITLE_WAVE_AMP * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME * Math.PI)) + titleLocRst.Y;
            bird.SetLoc(bird.GetLoc().X, TITLE_WAVE_AMP * (float)(Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / TITLE_WAVE_TIME * Math.PI)) + bird.GetRstLoc(Bird.MENU).Y);

            // update ground
            UpdateGround();

            // check if there is no fade
            if (fadeCurTime.IsFinished() || fadeCurTime.IsInactive())
            {
                // check if player clicks the start button or presses space, set nextGameState to pre game play
                // if so, reset the fade respectively
                if (IsClickedOn(btnRecs[START_BTN]) || IsKeyPressed(Keys.Space))
                {
                    // reset fade and active it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(Assets.fadeSnd);

                    // set next game state to pre game
                    nxtGameState = PRE_GAME_PLAY_STATE;
                }
                else if (IsClickedOn(btnRecs[STATS_BTN]))
                {
                    // reset fade and active it
                    ResetFade(FADE_STATE_TIME, true);

                    // start fade sound
                    PlaySound(Assets.fadeSnd);

                    // set next game state to pre game
                    nxtGameState = STATS_STATE;
                }
            }

            // if fade timer is on update it
            if (fadeCurTime.IsActive())
            {
                // update fade time
                UpdateFade(gameTime, FADE_STATE_TIME, nxtGameState);
            }
        }

        private void UpdatePreGame(GameTime gameTime)
        {
            // update ground scrolling
            UpdateGround();
            bird.Update(gameTime);

            // only update the fade if the timer is active
            if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME);
            }

            //  other wise check if the player has pressed space or clicked to start
            else if (fadeCurTime.IsFinished() && (IsClick() || IsKeyPressed(Keys.Space)))
            {
                // change the game state to game play
                gameState = GAME_PLAY_STATE;

                bird.state = Bird.ALIVE;

                bird.SetAnimDur(Bird.ANIM_GAME_DUR);

                bird.Update(gameTime, true);
            }
        }

        private void UpdateGamePlay(GameTime gameTime)
        {
            // update the bird
            bird.Update(gameTime, IsClick() || IsKeyPressed(Keys.Space));

            // update the pipes
            pipesManager.UpdatePipes();

            // check if bird passed
            if (pipesManager.CheckPassed(bird.GetRec())) UpdateScore();

            UpdateGround();

            UpdateCollision();
        }

        private void UpdatePreGameOvr(GameTime gameTime)
        {
            // update the death fade (flash)
            UpdateFadeDeath(gameTime);
            birdDeathTimer.Update(gameTime);

            // check how the bird died
            switch (bird.deathType)
            {
                case Bird.GRD_DEATH:
                    // update the death timer
                    UpdateCollision();
                    break;
                case Bird.PIPE_DEATH:
                    // still need to wait for bird to hit ground
                    // thus update the bird position
                    bird.Update(gameTime);

                    UpdateCollision();
                    break;
            }


            // change to game over state after death timer finish
            if (birdDeathTimer.IsFinished())
            {
                // update game sate
                gameState = GAME_OVER_STATE;

                scoreData.UpdateStats();

                // set newBest flag to true, if new high score is reached
                if (scoreData.GetBestScore() < curScore)
                {
                    isNewBest = true;
                }

                // reset game over state
                ResetGameOvr();
            }
        }

        private void UpdateGameOvr(GameTime gameTime)
        {
            if (birdDeathTimer.IsActive())
            {
                birdDeathTimer.Update(gameTime);
            }
            else if (birdDeathTimer.IsFinished())
            {
                birdDeathTimer.ResetTimer(false);
                bird.PlayDeathSnd();
            }


            // update the game over title
            UpdateGameOvrTitle(gameTime);

            // update the result box and it's elements
            UpdateRsltBox(gameTime);

            // update score elements in the result box
            UpdatersltScore(gameTime);

            // update the menu button pop up timer
            menuUpTime.Update(gameTime);

            // activate the menu timer is the score counter if finished
            if (rsltBoxLoc.Y == rsltBoxLocFin.Y && rsltScore == curScore && menuUpTime.IsInactive())
            {
                menuUpTime.Activate();
            }

            // check if menu timer is active
            if (menuUpTime.IsActive())
            {
                // update the y position of the button passed on the completion of the timer
                btnRecs[MENU_BTN].Y = (int)MathHelper.Lerp(MENU_BTN_Y_RST, MENU_BTN_Y_FIN, (float)menuUpTime.GetTimePassed() / MENU_POP_UP_TIME);
            }
            // else check if fade between states is active, and update fade is so
            else if (fadeCurTime.IsActive())
            {
                UpdateFade(gameTime, FADE_STATE_TIME, MENU_STATE);
            }
            // check if player has press menu button or pressed space, if the score counting is finished and result box is in place
            else if ((IsClickedOn(btnRecs[MENU_BTN]) || IsKeyPressed(Keys.Space)) && menuUpTime.IsFinished() && rsltBoxLoc.Y == rsltBoxLocFin.Y)
            {
                scoreData.SetFlapsCounts(bird.GetFlapCount() + scoreData.GetFlapCount());
                scoreData.AddScore(curScore);

                // reset the fade, and activate it
                ResetFade(FADE_STATE_TIME, true);

                // start fade sound
                PlaySound(Assets.fadeSnd);
            }
        }


        // method to update game over title
        private void UpdateGameOvrTitle(GameTime gameTime)
        {
            // update the game over title fade
            gameOvrFade.Update(gameTime);

            // update game over title position
            gameOvrLoc.Y = GAME_OVR_WAVE_AMP * (float)Math.Sin((gameOvrFade.GetTimePassed() + GAME_OVR_FADE_TIME) * (4.0 / (3.0 * GAME_OVR_FADE_TIME)) * Math.PI) + GAME_OVR_WAVE_DISPL;
            // more info of the sine wave
            // https://www.desmos.com/calculator/lk5euhols6
        }

        // method to update result box and it's elements
        private void UpdateRsltBox(GameTime gameTime)
        {
            // check if game over title fade is finished and result box hasn't reached
            if (gameOvrFade.IsFinished() && rsltBoxLoc != rsltBoxLocFin)
            {
                // update the position of the box with the speed
                rsltBoxLoc.Y += RSLT_BOX_SPD;

                // if the box crossed, set it equal to the final position
                if (rsltBoxLoc.Y < rsltBoxLocFin.Y)
                {
                    rsltBoxLoc.Y = rsltBoxLocFin.Y;
                }
            }
            // update sparkle if the result score counted up to current score, and a medal is earned
            else if (rsltScore == curScore && curScore >= BRONZE_SCR)
            {
                UpdateSparkle(gameTime);
            }
        }

        // method to update the result scores
        private void UpdatersltScore(GameTime gameTime)
        {
            // check if result box is in place
            if (rsltBoxLoc.Y == rsltBoxLocFin.Y)
            {
                // check if the result score is lower, and count delay is finished
                // then the results score counts up to current score
                if (rsltScore < curScore && cntDelay.IsFinished())
                {
                    // increase the count of the result score
                    rsltScore++;

                    // check if the result score surpassed the high score, if so update the high score
                    if (isNewBest && rsltScore > scoreData.GetBestScore())
                    {
                        scoreData.SetBestScore(scoreData.GetBestScore() + 1);
                    }

                    // reset the timer
                    cntDelay.ResetTimer(true);

                }
                // update the count delay between increments
                else
                {
                    cntDelay.Update(gameTime);
                }
            }
        }

        // method to update the sparkles
        private void UpdateSparkle(GameTime gameTime)
        {
            // check if animation is currently not animating
            if (!sparkAnim.IsAnimating())
            {
                // reset the position of the animation
                sparkLoc.Y = rand.Next((int)coinLoc.Y, (int)(coinLoc.Y + coinSize.Y));
                sparkLoc.X = rand.Next((int)coinLoc.X, (int)(coinLoc.X + coinSize.X));

                // check if the distance between the coin's origin
                while (PointDistance(sparkLoc, coinOrigin) > coinRadius)
                {
                    // re generate a coin position
                    sparkLoc.Y = rand.Next((int)coinLoc.Y, (int)(coinLoc.Y + coinSize.Y));
                    sparkLoc.X = rand.Next((int)coinLoc.X, (int)(coinLoc.X + coinSize.X));
                }

                // translate the animation's center to spark Loc
                sparkAnim.TranslateTo(sparkLoc.X - sparkAnim.GetDestRec().Width / 2, sparkLoc.Y - sparkAnim.GetDestRec().Height / 2);

                // active the spark animation and reset it
                sparkAnim.Activate(true);
            }
            // if animating (not not animating), update the animation
            else
            {
                sparkAnim.Update(gameTime);
            }
        }

        // method that updates the fade death class
        private void UpdateFadeDeath(GameTime gameTime)
        {
            UpdateFade(gameTime, FADE_DEATH_TIME);
        }


        // updates the fade time, and returns true if fade timer is completed
        // if isIn is true, then it's fading in, otherwise it's fading out
        private void UpdateFade(GameTime gameTime, int fadeTime, int nextGameState = -1)
        {
            // update the fade timer
            fadeCurTime.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            // fade out
            if (fadeCurTime.GetTimePassed() < fadeTime / 2)
            {
                // fade out
                // store the opacity of the fade, as it should be
                fadeOpacity = Math.Min(1f, (float)fadeCurTime.GetTimePassed() / (fadeTime / 2) * FULL_OPACITY);
            }
            else
            {
                // fade in
                if (nextGameState != -1 && !fadedHalf)
                {
                    // set game state to the next sate in queue
                    gameState = nextGameState;

                    // reset set the next game state
                    switch (gameState)
                    {
                        case PRE_GAME_PLAY_STATE:
                            // reset pre game place
                            ResetPreGamePlay();
                            break;
                        case MENU_STATE:
                            // reset the menu state
                            ResetMenu();
                            break;
                    }
                }

                // store the fade opacity as the timer is a factor which opacity changes
                fadeOpacity = Math.Min(1f, (fadeTime - (float)fadeCurTime.GetTimePassed()) / (fadeTime / 2)) * FULL_OPACITY;
                fadedHalf = true;
            }
        }

        private void UpdateGround()
        {
            // update ground scrolling
            for (int i = 0; i < grdRecs.Length; i++)
            {
                // check if the right side of the ground rectangle has passed/reach the screen's right (0)
                if (grdRecs[i].Right <= 0)
                {
                    // reposition image back to the other end of the other image
                    grdLocs[i].X = Assets.grdImg.Width;
                }
                // move each ground image left by the scroll speed
                grdLocs[i].X -= SCRL_SPD;

                // update ground rectangles to match ground position
                grdRecs[i].X = (int)grdLocs[i].X;
            }
        }

        private void UpdateCollision()
        {
            // check if bird collided with pipe (or top of screen, which is considered as a *pipe*)
            if ((pipesManager.CheckCollision(bird.GetRec()) || bird.GetRec().Top < 0) && bird.state != Bird.DEAD)
            {
                bird.deathType = Bird.PIPE_DEATH;

                // method that reset all death delay variables
                DeathDelayReset();

                // play hit sound and activate hit timer
                bird.PlayHitSnd();

            }

            if (bird.GetRec().Bottom > GRD_Y)
            {
                bird.SetLoc(bird.GetLoc().X, GRD_Y - bird.GetRec().Height);

                if (bird.state != Bird.DEAD)
                {
                    bird.deathType = Bird.GRD_DEATH;

                    bird.PlayHitSnd();

                    // method that reset all death delay variables
                    DeathDelayReset();
                }
                else if (birdDeathTimer.IsFinished())
                {
                    gameState = PRE_GAME_OVER_STATE;
                }
                else
                {
                    birdDeathTimer.Activate();
                }
            }
        }

        // method to update score, (increment used for debug)
        private void UpdateScore(int increment = 1)
        {
            // add increment to the current score value
            curScore += increment;

            PlaySound(Assets.pointSnd);
        }

        private void DeathDelayReset()
        {
            bird.SetDeathAnim();

            // reset bird timer
            birdDeathTimer.ResetTimer(false);

            // reset the fade timer
            ResetFade(FADE_DEATH_TIME, true);

            // set game state to pre game, and isBirdDeath to true
            gameState = PRE_GAME_OVER_STATE;
            bird.state = Bird.DEAD;

            // check if ground death
            if (bird.deathType == Bird.GRD_DEATH)
            {
                // if ground death, bird death timer starts
                birdDeathTimer.Activate();
            }
        }

        // method to reset any menu elements
        private void ResetMenu()
        {
            // reset the bird position to title position
            bird.Rst(Bird.MENU);
        }

        private void ResetPreGamePlay()
        {
            // set bird to pre game state
            bird.Rst(Bird.PRE_GAME);

            // reset current score variables
            ResetScore();

            // reset the positions of pipes
            pipesManager.RstPipes();
        }

        // method to reset any game over elements
        private void ResetGameOvr()
        {
            // reset the count delay timer. However only activate it if the current score is non-zero
            if (curScore != 0)
            {
                cntDelay.ResetTimer(true);
            }
            else
            {
                cntDelay.ResetTimer(false);
            }

            // reset the game over fade title timer
            gameOvrFade.ResetTimer(true);

            // reset the result box position
            rsltBoxLoc = rsltBoxLocRst;

            // reset the menu button timer and position
            menuUpTime.ResetTimer(false);
            btnRecs[MENU_BTN].Y = MENU_BTN_Y_RST;
        }

        // reset all fade variables
        private void ResetFade(int fadeTime, bool isActive)
        {
            // reset the fade opacity and fade timer
            fadeOpacity = EMPTY_OPACITY;
            fadeCurTime.SetTargetTime(fadeTime);
            fadeCurTime.ResetTimer(isActive);

            // reset the fade half flag
            fadedHalf = false;
        }

        // method reset the score variables
        private void ResetScore()
        {
            // reset the position of the score
            curScoreLoc.X = SCREEN_WIDTH / 2; // able to use 20 since (x/10/2) is (x/20)

            // reset the current and result scores
            curScore = SCOR_RST;
            rsltScore = SCOR_RST;

            // reset best score flag to false
            isNewBest = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // draw the background
            spriteBatch.Draw(Assets.bgImg, bgLoc, Color.White);

            // draw the current game state
            switch (gameState)
            {
                case STATS_STATE:
                    //draw stats screen
                    DrawStats();
                    break;

                case MENU_STATE:
                    // draw title screen
                    DrawMenuScreen();
                    break;

                case PRE_GAME_PLAY_STATE:
                    // draw pre game play state
                    DrawPreGamePlay();
                    break;

                case GAME_PLAY_STATE:
                    // draw game play state
                    DrawGamePlay();
                    break;

                case PRE_GAME_OVER_STATE:
                    // draw the pre game over screen
                    DrawPreGameOvr();
                    break;

                case GAME_OVER_STATE:
                    // draw the game over screen
                    DrawGameOvr();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        // method draw the stats screen
        private void DrawStats()
        {
            // draw the ground
            DrawGround();

            // draw the menu button
            spriteBatch.Draw(Assets.menuBtnImg, btnRecs[MENU_BTN], Color.White);

            // draw the stats box
            DrawStatsBox();

            // draw fade
            DrawFadeScr(Color.Black);
        }

        // method to draw the menu screen
        private void DrawMenuScreen()
        {
            // draw the ground
            DrawGround();

            // draw the title
            spriteBatch.Draw(Assets.titleImg, titleLoc, Color.White);

            // draw animated bird
            bird.Draw();

            // draw the starting and stats button in the title screen
            spriteBatch.Draw(Assets.startBtnImg, btnRecs[START_BTN], Color.White);
            spriteBatch.Draw(Assets.statsBtnImg, btnRecs[STATS_BTN], Color.White);

            // only draw the fade is the timer is active
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }
        }


        // method to draw the pre game play screen
        private void DrawPreGamePlay()
        {
            bird.Draw();

            // draw the ground
            DrawGround();

            // draw click instructions
            spriteBatch.Draw(Assets.instrucImg, intrcuLoc, Color.White);

            // draw get ready title
            spriteBatch.Draw(Assets.getReadyImg, readyTitleLoc, Color.White);

            // check if the timer is still running, if so draw the fade
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }

            // draw the current score, such that the unit digits is centered
            DrawNumberSequence(curScore.ToString(), Assets.lrgNumsImg, curScoreLoc, NUM_SPC_FACTOR, 0.5f);
        }

        // method to draw the game play
        private void DrawGamePlay()
        {
            pipesManager.DrawPipes();

            DrawGround();

            bird.Draw();

            // draw the current score
            DrawNumberSequence(curScore.ToString(), Assets.lrgNumsImg, curScoreLoc, NUM_SPC_FACTOR, 0.5f);
        }

        // method to draw the pre game over screen
        private void DrawPreGameOvr()
        {
            // draw the pipes
            pipesManager.DrawPipes();

            // draw the ground
            DrawGround();

            // draw the bird rotated
            bird.Draw();

            // draw the death fade (flash)
            DrawFadeScr(Color.White);
        }

        // method to draw the game over screen
        private void DrawGameOvr()
        {

            // draw the pipes
            pipesManager.DrawPipes();

            // draw the ground
            DrawGround();

            // draw the bird rotated
            bird.Draw();

            // draw the game over title
            DrawGameOvrTitle();

            // draw the result box and it's elements
            DrawRsltBox();

            // result box is in place draw the menu
            if (rsltBoxLoc == rsltBoxLocFin)
            {
                spriteBatch.Draw(Assets.menuBtnImg, btnRecs[MENU_BTN], Color.White);
            }

            // check if the timer is still running, if so draw the fade
            if (fadeCurTime.IsActive())
            {
                DrawFadeScr(Color.Black);
            }
        }

        // method to draw the ground
        private void DrawGround()
        {
            // draw the ground
            spriteBatch.Draw(Assets.grdImg, grdRecs[0], Color.White);
            spriteBatch.Draw(Assets.grdImg, grdRecs[1], Color.White);
        }

        // draw the fade screen
        private void DrawFadeScr(Color color)
        {
            spriteBatch.Draw(Assets.fadeImg, scrnRec, color * fadeOpacity);
        }


        // method to draw numbers with the num texture when given a number in the form a string
        private void DrawNumberSequence(string num, Texture2D texture, Vector2 Loc, int spacingFactor, float rightOffset = 0)
        {
            // store the width of a digit
            int digitWidth = texture.Width / 10; // 10 digits in sprite

            // space between each digit, as a factor of the digit width
            int numSpacer = digitWidth / spacingFactor;

            // change the position of the number by the numbers length, and offset
            Loc.X -= num.Length * (texture.Width / 10 + numSpacer) - numSpacer;
            Loc.X += digitWidth * rightOffset;

            // iterate through the number
            for (int i = 0; i < num.Length; i++)
            {
                // store the digits value, by changing char to int
                int digit = num[i] - '0';

                // store the digit's draw position and score rectangle based of which number place it is
                Vector2 drawLoc = new Vector2(Loc.X + i * (digitWidth + numSpacer), Loc.Y);
                Rectangle sourceRec = new Rectangle(digit * digitWidth, 0, digitWidth, texture.Height);

                // draw the digit
                spriteBatch.Draw(texture, drawLoc, sourceRec, Color.White);
            }
        }

        // method to draw the game title
        private void DrawGameOvrTitle()
        {
            spriteBatch.Draw(Assets.gameOvrImg, gameOvrLoc, Color.White * (float)(gameOvrFade.GetTimePassed() / GAME_OVR_FADE_TIME));
        }

        // method draw the result box and it's elements
        private void DrawRsltBox()
        {
            spriteBatch.Draw(Assets.rsltsBoxImg, rsltBoxLoc, Color.White);

            // check if the result box is at it's final position, meaning that it finished moving
            if (rsltBoxLoc == rsltBoxLocFin)
            {
                // draw the result and best score with large numbers
                // spacing between the digits of a 1/4 of the digit size
                // offset is zero as digits are right of the points
                DrawNumberSequence(rsltScore.ToString(), Assets.lrgNumsImg, rsltcurScoreLoc, NUM_SPC_FACTOR, 0);
                DrawNumberSequence(scoreData.GetBestScore().ToString(), Assets.lrgNumsImg, rsltHighScorLoc, NUM_SPC_FACTOR, 0);

                // only draw the coins if the score counter if finished
                if (rsltScore == curScore)
                {
                    // check if score is greater or equal to each coin score
                    // if so draw that coin
                    if (curScore >= PLAT_SCR)
                    {
                        DrawCoin(PLAT_COIN);
                    }
                    else if (curScore >= GOLD_SCR)
                    {
                        DrawCoin(GOLD_COIN);
                    }
                    else if (curScore >= SILVER_SCR)
                    {
                        DrawCoin(SILVER_COIN);
                    }
                    else if (curScore >= BRONZE_SCR)
                    {
                        DrawCoin(BRONZE_COIN);
                    }
                }

                // draw the nest best if a nest best is reached and result score has reached the best score
                if (isNewBest && scoreData.GetBestScore() == rsltScore)
                {
                    DrawNewPb();
                }
            }
        }

        // method to draw the spark animation
        private void DrawSparkle()
        {
            sparkAnim.Draw(spriteBatch, Color.White);
        }

        // method to draw the coin's, when given coin type
        private void DrawCoin(int coinType)
        {
            // use a source rectangle to find crop the coins images
            // as the coin type is by index within the coin's image, by multiplying it by the coin size the x value of the source rectangle is given
            Rectangle sourceRec = new Rectangle(coinType * (int)coinSize.X, 0, (int)coinSize.X, (int)coinSize.Y);

            // draw the actual coin
            spriteBatch.Draw(Assets.coinsImg, coinLoc, sourceRec, Color.White);

            // call the draw spark's method to draw sparks
            DrawSparkle();
        }

        // method to draw the new best tag
        private void DrawNewPb()
        {
            // draw the new Best tag
            spriteBatch.Draw(Assets.newImg, newBestLoc, Color.White);
        }

        // method to draw stats rectangle box
        private void DrawStatsBox()
        {
            // draw the stats image
            spriteBatch.Draw(Assets.statsBoxImg, statsBoxLoc, Color.White);

            // draw the total play count and total flap count with small numbers
            DrawNumberSequence(scoreData.GetPlaysCount().ToString(), Assets.smallNumsImg, playsLoc, NUM_SPC_FACTOR, 0);
            DrawNumberSequence(scoreData.GetFlapCount().ToString(), Assets.smallNumsImg, flapsLoc, NUM_SPC_FACTOR, 0);

            // draw the average score, and average flaps with small numbers
            DrawNumberSequence(scoreData.GetAverageScore().ToString(), Assets.smallNumsImg, avgScorsLoc, NUM_SPC_FACTOR, 0);
            DrawNumberSequence(scoreData.GetAverageFlaps().ToString(), Assets.smallNumsImg, avgFlapsLoc, NUM_SPC_FACTOR, 0);

            // iterate until the end of the score list or until the top 5 scores (TOP_SCORS)
            for (int i = 0; i < scoreData.GetScoreListSize() && i < TOP_SCORS; i++)
            {
                // draw the ith score
                DrawNumberSequence(scoreData.GetScore(i).ToString(), Assets.smallNumsImg, new Vector2(topScorsLoc.X, topScorsLoc.Y + i * TOP_SCORS_Y_SPC), NUM_SPC_FACTOR, 0);
            }
        }


        // method which returns the distance between given two points
        private double PointDistance(Vector2 point1, Vector2 point2)
        {
            // use Pythagorean theorem to find distance between two points
            return Math.Sqrt(Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.X - point2.X, 2));
        }


        // method play sound effect with a specific volume
        private void PlaySound(SoundEffect snd, float volume = 1f)
        {
            SoundEffectInstance sndInstance = snd.CreateInstance();
            sndInstance.Volume = volume;
            sndInstance.Play();
        }

        // method that returns if a rectangle is click on
        private bool IsClickedOn(Rectangle rec)
        {
            // if mouse in rectangle and is clicked return true
            return rec.Contains(mouse.Position) && IsClick();
        }

        // method that returns is mouse is clicked
        private bool IsClick()
        {
            return mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed;
        }

        // method that returns if a given key is pressed
        private bool IsKeyPressed(Keys key)
        {
            return kb.IsKeyDown(key) && !prevKb.IsKeyDown(key);
        }
    }
}
