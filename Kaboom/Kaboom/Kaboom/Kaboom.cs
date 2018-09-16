// Author: Jason Wong
// File Name: Kaboom
// Project Name: Kaboom
// Creation Date: 4/28/2016
// Modified Date: 5/15/2016
// Description: This program will allow the user to play Kaboom

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Kaboom
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Kaboom : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SoundEffect DropBomb;
        SoundEffectInstance DropBombInstance;

        Random rng = new Random();

        //Collect Keyboard and Mouse Inputs from the User
        KeyboardState kb;
        MouseState mb;

        //Sprite Sheets---------------------------------

        //Sprite Sheet Names
        Texture2D Background;
        Texture2D Bomber;
        Texture2D Bucket;
        Texture2D Bomb;
        Texture2D Number;
        Texture2D GameDone;
        Texture2D Explosions;

        //Sprite Dimensions-----------------------------

        //Number Sprite
        int NumberW;
        int NumberH;
        int NumberFrameWide = 10;
        Rectangle[] NumberRec = new Rectangle[3];
        Rectangle[] NumberSrcRec = new Rectangle[3];
        int NumberDrawW;
        int NumberDrawH;

        //Background Sprite
        int BackgroundW;
        int BackgroundH;
        int BackgroundFramesWide = 2;
        Rectangle BackgroundRec;
        Rectangle BackgroundSrcRec;
        int BackgroundDrawW;
        int BackgroundDrawH;

        //Bomber Sprite
        int BomberW;
        int BomberH;
        int BomberFramesWide = 3;
        Rectangle BomberRec;
        Rectangle BomberSrcRec;
        int BomberDrawW;
        int BomberDrawH;

        //Bomb Sprite
        int BombW;
        int BombH;
        int BombFramesWide = 4;
        Rectangle[] BombRec = new Rectangle[400];
        Rectangle[] BombSrcRec = new Rectangle[400];
        int BombDrawW;
        int BombDrawH;

        //Bucket Sprite
        int BucketW;
        int BucketH;
        int BucketFramesWide = 4;
        Rectangle[] BucketRec = new Rectangle[3];
        Rectangle[] BucketSrcRec = new Rectangle[3];
        int BucketDrawW;
        int BucketDrawH;

        //Explosion Sprite
        int ExploW;
        int ExploH;
        int ExploFrameWide = 3;
        Rectangle[] ExploRec = new Rectangle[8];
        Rectangle[] ExploSrcRec = new Rectangle[8];
        int ExploDrawW;
        int ExploDrawH;

        //Screen Dimensions
        int ScreenW;
        int ScreenH;

        //Animation-------------------------------------

        //Bombs
        int BombScrX;
        int[] BombFrame = new int[40];
        int BombSmoothness;

        //Explosion
        int SwapBomb;
        int ExploScrX;
        int ExploFrame;
        int ExploSmoothness;
        bool[] ExploDone = new bool[8];
        int ExploNumber;

        //Bucket
        int[] BucketScrX = new int[3];
        int[] BucketFrame = new int[3];
        bool BucketAnimate;
        int BucketSmoothness;
        int BucketNumber;

        //Background
        int BackgroundScrX;
        int BackgroundFrame;
        int BackgroundSmoothness;
        int FlickerNumber;

        //In-Game Counter-------------------------------

        //Loop Counter
        int i;
        int j;

        //Scores
        int ScoreOnes;
        int ScoreTens;
        int ScoreHuns;
        int CaughtCounter;

        int Lives = 3;
        int Level = 1;

        bool HitFloor;

        //Controls--------------------------------------

        //Bomber Controls
        int MovementTimer;
        int Movement;
        int Direction;
        int MoveSpeed;

        //Bomb Controls
        int BombRate;
        int BombSpeed;
        int BombCounter;
        int BombFall;

        //Collision-------------------------------------

        //Collision Scenarios
        const int NO_Collision = 0;
        const int Bucket_Collision = 1;
        const int Floor_Collision = 2;

        //Pixel Perfect Bucket Collision Rectangles
        Rectangle[] BucketBounds = new Rectangle[3];
        Rectangle FloorBounds = new Rectangle();
        Rectangle[] BombBounds = new Rectangle[40];

        //Game States-----------------------------------
        int GameStage;
        const int StageOff = 0;
        const int StageUpdate = 1;
        const int StageRun = 2;

        int RoundType;
        const int FullRound = 10;
        const int SemiRound = 5;

        const int BomberOneMS = 5;
        const int BomberTwoMS = 7;
        const int BomberThreeMS = 9;

        const int RoundOneSpeed = 40;
        const int RoundTwoSpeed = 30;
        const int RoundThreeSpeed = 20;

        const int RoundOneFall = 5;
        const int RoundTwoFall = 7;
        const int RoundThreeFall = 9;

        public Kaboom()
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
            //3:4 Ratio
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.PreferredBackBufferHeight = 768;

            this.graphics.ApplyChanges();

            //Store dimensions as integers
            ScreenH = graphics.GraphicsDevice.Viewport.Height;
            ScreenW = graphics.GraphicsDevice.Viewport.Width;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load all sprites sheets-----------------------

            Background = Content.Load<Texture2D>(@"Images\Background");
            Bomber = Content.Load<Texture2D>(@"Images\Bomber");
            Bucket = Content.Load<Texture2D>(@"Images\Bucket");
            Bomb = Content.Load<Texture2D>(@"Images\Bomb");
            Number = Content.Load<Texture2D>(@"Images\Number");
            GameDone = Content.Load<Texture2D>(@"Images\GameOver");
            Explosions = Content.Load<Texture2D>(@"Images\Explosion");

            //Create boundaries for the sprite sheets-------

            BackgroundW = Background.Width / BackgroundFramesWide;
            BackgroundH = Background.Height;
            BackgroundSrcRec = new Rectangle(0, 0, BackgroundW, BackgroundH);
            BackgroundDrawH = ScreenH;
            BackgroundDrawW = ScreenW;
            BackgroundRec = new Rectangle(0, 0, BackgroundDrawW, BackgroundDrawH);

            BomberW = Bomber.Width / BomberFramesWide;
            BomberH = Bomber.Height;
            BomberSrcRec = new Rectangle(0, 0, BomberW, BomberH);
            BomberDrawH = Bomber.Height;
            BomberDrawW = BomberW;
            BomberRec = new Rectangle(ScreenW / 2, 65, BomberDrawW, BomberDrawH);

            ExploW = Explosions.Width / ExploFrameWide;
            ExploH = Explosions.Height;
            ExploDrawH = ExploW * 3;
            ExploDrawW = ExploH * 3;

            NumberW = Number.Width / NumberFrameWide;
            NumberH = Number.Height;
            NumberDrawH = (NumberW * 2) + 5;
            NumberDrawW = (NumberH * 2) + 5;

            BombW = Bomb.Width / BombFramesWide;
            BombH = Bomb.Height;
            BombDrawH = Bomb.Height * 2;
            BombDrawW = BombW * 2;

            BucketW = Bucket.Width / BucketFramesWide;
            BucketH = Bucket.Height;
            BucketDrawH = BucketH;
            BucketDrawW = 100;

            FloorBounds = new Rectangle(0, ScreenH - 75, ScreenW, 50);

            //Loading Array Contents -----------------------

            //Sets the starting points for the explosions  
            for (i = 0; i < 8; i++)
            {
                ExploRec[i] = new Rectangle(-50, -50, ExploDrawW, ExploDrawH);
                ExploSrcRec[i] = new Rectangle(-50, 0, ExploW, ExploH);
                ExploDone[i] = true;
            }

            //Spawns in 40 bombs off screen ready to be used
            for (i = 0; i < 40; i++)
            {
                BombRec[i] = new Rectangle(-50, -50, BombDrawW, BombDrawH);
                BombSrcRec[i] = new Rectangle(0, 0, BombW, BombH);
            }

            //Sets the starting point of all three buckets and numbers
            for (i = 0; i < 3; i++)
            {
                BucketSrcRec[i] = new Rectangle(0, 0, BucketW, BucketH);
                NumberSrcRec[i] = new Rectangle(0, 0, NumberW, NumberH);
            }

            //Setting the location on screen for the numbers
            NumberRec[0] = new Rectangle((Background.Width / 2 + 500), 35, NumberDrawW, NumberDrawH);
            NumberRec[1] = new Rectangle((NumberRec[0].X - 25), 35, NumberDrawW, NumberDrawH);
            NumberRec[2] = new Rectangle((NumberRec[1].X - 25), 35, NumberDrawW, NumberDrawH);

            //Game rules------------------------------------

            //Starting round type
            RoundType = FullRound;

            //Sounds----------------------------------------
            DropBomb = Content.Load<SoundEffect>(@"Sounds\18530");
            DropBombInstance = DropBomb.CreateInstance();
        }

        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //User Inputs
            mb = Mouse.GetState();
            kb = Keyboard.GetState();

            //Allows the game to exit
            if (kb.IsKeyDown(Keys.Escape))
                this.Exit();

            //Checks which subprograms can run via the state of the game
            switch (GameStage)
            {
                //Nothing is running once on this state
                case (StageOff):

                    //Plays explosion animation only if the round ends because a bomb was missed
                    if (HitFloor)
                    {
                        BackgroundFlicker();
                        ExplosionAnimate();
                    }

                    BucketMovement();

                    CatchAnimation(BucketNumber, BucketAnimate);

                    //Waits for the user to reset and move on to the next round
                    if (mb.LeftButton == ButtonState.Pressed)
                    {
                        BomberMood(0);
                        StageCheck(Level);
                        BombCounter = 0;
                        GameStage = StageRun;
                    }
                    break;

                //Updates are done during this state to check for collision, ect
                case (StageUpdate):

                    BucketMovement();
                    BombDropping();
                    CollisionTest();
                    BombAnimation();
                    StageClear();
                    CatchAnimation(BucketNumber, BucketAnimate);
                    break;

                //Updates along with AI is ran during this state
                case (StageRun):

                    BucketMovement();
                    BombDropping();
                    CollisionTest();
                    BombAnimation();
                    BomberAI();
                    BombSpawn();
                    CatchAnimation(BucketNumber, BucketAnimate);
                    break;
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(Background, BackgroundRec, BackgroundSrcRec, Color.White);
            spriteBatch.Draw(Bomber, BomberRec, BomberSrcRec, Color.White);

            DrawBomb();

            //Draw explosion rectangles waiting to be used
            for (i = 0; i < 3; i++)
            {
                spriteBatch.Draw(Explosions, ExploRec[i], ExploSrcRec[i], Color.White);
            }

            LifeCount();
            GameOver();
            DisplayScore(ScoreOnes, ScoreTens, ScoreHuns);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Allows bomber to move in a random order
        private void BomberAI()
        {
            MovementTimer = MovementTimer + 1;

            //Checks if the bomber is at the edges
            if (BomberRec.X <= (50))
            {
                Direction = (MoveSpeed);
            }
            else if (BomberRec.X >= (ScreenW - 125))
            {
                Direction = -(MoveSpeed);
            }
            //Picks a new direction for the bomber to move in
            else
            {
                if (MovementTimer % 7 == 0)
                {
                    if (BomberRec.X <= (ScreenW / 2))
                    {
                        Movement = rng.Next(2, 5);
                    }
                    else
                    {
                        Movement = rng.Next(1, 4);
                    }

                    switch (Movement)
                    {
                        case (1):
                        case (2):
                            Direction = -(MoveSpeed);
                            break;
                        default:
                            Direction = MoveSpeed;
                            break;
                    }
                }
            }
            //Moves the bombers X coordinate everytime update is called
            BomberRec.X = BomberRec.X + Direction;
        }

        //Allows the player to move the bucket
        private void BucketMovement()
        {
            //Checks if the buckets are at the edges
            if (mb.X <= 49)
            {
                BucketRec[0] = new Rectangle(50, 480, BucketDrawW, BucketDrawH);
                BucketRec[1] = new Rectangle(50, 540, BucketDrawW, BucketDrawH);
                BucketRec[2] = new Rectangle(50, 600, BucketDrawW, BucketDrawH);
                BucketBounds[0] = new Rectangle(50, 508, BucketDrawW, BucketDrawH / 2 + 2);
                BucketBounds[1] = new Rectangle(50, 568, BucketDrawW, BucketDrawH / 2 + 2);
                BucketBounds[2] = new Rectangle(50, 628, BucketDrawW, BucketDrawH / 2 + 2);
            }
            else if (mb.X >= 875)
            {
                BucketRec[0] = new Rectangle(874, 480, BucketDrawW, BucketDrawH);
                BucketRec[1] = new Rectangle(874, 540, BucketDrawW, BucketDrawH);
                BucketRec[2] = new Rectangle(874, 600, BucketDrawW, BucketDrawH);
                BucketBounds[0] = new Rectangle(874, 508, BucketDrawW, BucketDrawH / 2 + 2);
                BucketBounds[1] = new Rectangle(874, 568, BucketDrawW, BucketDrawH / 2 + 2);
                BucketBounds[2] = new Rectangle(874, 628, BucketDrawW, BucketDrawH / 2 + 2);
            }
            //Alows player to control the buckets via mouse
            else
            {
                BucketRec[0] = new Rectangle((mb.X), 480, BucketDrawW, BucketDrawH);
                BucketRec[1] = new Rectangle((mb.X), 540, BucketDrawW, BucketDrawH);
                BucketRec[2] = new Rectangle((mb.X), 600, BucketDrawW, BucketDrawH);
            }
        }

        //Tracks the lives left
        private void LifeCount()
        {
            switch (Lives)
            {
                //Removes buckets based on how many lives are left
                case 3:
                    spriteBatch.Draw(Bucket, BucketRec[0], BucketSrcRec[0], Color.White);
                    spriteBatch.Draw(Bucket, BucketRec[1], BucketSrcRec[1], Color.White);
                    spriteBatch.Draw(Bucket, BucketRec[2], BucketSrcRec[2], Color.White);
                    BucketBounds[0] = new Rectangle((mb.X), 508, BucketDrawW, BucketDrawH / 2 + 2);
                    BucketBounds[1] = new Rectangle((mb.X), 568, BucketDrawW, BucketDrawH / 2 + 2);
                    BucketBounds[2] = new Rectangle((mb.X), 628, BucketDrawW, BucketDrawH / 2 + 2);
                    break;
                case 2:
                    spriteBatch.Draw(Bucket, BucketRec[0], BucketSrcRec[0], Color.White);
                    spriteBatch.Draw(Bucket, BucketRec[1], BucketSrcRec[1], Color.White);
                    BucketBounds[0] = new Rectangle((mb.X), 508, BucketDrawW, BucketDrawH / 2 + 2);
                    BucketBounds[1] = new Rectangle((mb.X), 568, BucketDrawW, BucketDrawH / 2 + 2);
                    break;
                case 1:
                    spriteBatch.Draw(Bucket, BucketRec[0], BucketSrcRec[0], Color.White);
                    BucketBounds[0] = new Rectangle((mb.X), 508, BucketDrawW, BucketDrawH / 2 + 2);
                    break;
            }

        }

        //Tracks the speed of the bombs falling
        private void BombDropping()
        {
            //Changes the Y coordinates of any bomb which is on screen
            for (i = 0; i < RoundType * (Level); i++)
            {
                if (BombRec[i].Y >= 160)
                {
                    BombRec[i].Y = BombRec[i].Y + BombFall;
                    BombBounds[i].Y = BombBounds[i].Y + BombFall;
                }
            }
        }

        //Spawns in bombs for player to catch
        private void BombSpawn()
        {
            BombRate = BombRate + 1;

            //Spawns bomb below bomber on a timer
            if (BombRate % BombSpeed == 0)
            {
                if (BombCounter < RoundType * (Level))
                {
                    BombRec[BombCounter] = new Rectangle((BomberRec.X + 15), 160, BombDrawW, BombDrawH);
                    BombBounds[BombCounter] = new Rectangle((BomberRec.X + 15), 185, (BombDrawW - 14), (BombDrawH - 25));

                    //Limits the amount of bombs dropped
                    BombCounter = BombCounter + 1;
                }
                else
                {
                    GameStage = StageUpdate;
                }
            }
        }

        //Draws bombs on screen
        private void DrawBomb()
        {
            //Draws enough bombs for the round
            for (i = 0; i < RoundType * (Level); i++)
            {
                spriteBatch.Draw(Bomb, BombRec[i], BombSrcRec[i], Color.White);
            }
        }

        //Pre: An integer which corresponds with a mood
        //Post: Changes the mood of the bomber
        //Desc: Switch statement which matches the integer with a source rectangle
        private void BomberMood(int Mood)
        {
            switch (Mood)
            {
                case (1):
                    BomberSrcRec = new Rectangle((Bomber.Width * 1 / BomberFramesWide), 0, BomberW, BomberH);
                    break;
                case (2):
                    BomberSrcRec = new Rectangle((Bomber.Width * 2 / BomberFramesWide), 0, BomberW, BomberH);
                    break;
                default:
                    BomberSrcRec = new Rectangle(0, 0, BomberW, BomberH);
                    break;
            }
        }

        //Pre: Both the bucket and bomb rectangles
        //Post: Check collision with all the buckets and returns an integer corresponding to the result
        //Desc: If statement which checks if the two rectangles are touching
        private int CollisionBucket(Rectangle Bucket, Rectangle Bomb)
        {
            if (Bomb.Bottom < Bucket.Top ||
                Bomb.Right < Bucket.Left ||
                Bomb.Left > Bucket.Right ||
                Bomb.Top > Bucket.Bottom)
            {
                return NO_Collision;
            }
            else
            {
                return Bucket_Collision;
            }
        }

        //Pre: Both the floor and bomb rectangles
        //Post: Check collision with the floor and returns an integer corresponding to the result
        //Desc: If statement which checks if the two rectangles are touching
        private int CollisionFloor(Rectangle Floor, Rectangle Bomb)
        {
            if (Bomb.Bottom > Floor.Top)
            {
                return Floor_Collision;
            }
            else
            {
                return NO_Collision;
            }
        }

        //Checks both types of Collision and removes bombs once they are touched
        private void CollisionTest()
        {
            for (i = 0; i < 40; i++)
            {
                if (CollisionFloor(FloorBounds, BombBounds[i]) == Floor_Collision)
                {
                    //Remove a life point
                    Lives = Lives - 1;

                    //Reset timers/counter
                    FlickerNumber = 0;

                    SwapBomb = 0;

                    CaughtCounter = 0;

                    RoundType = SemiRound;

                    HitFloor = true;

                    //Queue explosion animation
                    Explosion();

                    //Change the bombers mood
                    BomberMood(1);

                    GameStage = StageOff;
                }
                else
                {
                    for (j = 0; j <= (Lives - 1); j++)
                    {
                        if (CollisionBucket(BucketBounds[j], BombBounds[i]) == Bucket_Collision)
                        {
                            BombBounds[i] = new Rectangle(-50, -50, BombDrawW, BombDrawH);
                            BombRec[i] = new Rectangle(-50, -50, BombDrawW, BombDrawH);

                            //Adds score
                            ScoreKeeper(Level);

                            //Tracks how many bombs are caught
                            CaughtCounter = CaughtCounter + 1;

                            //Queue splash animation
                            BucketAnimate = true;
                            BucketNumber = j;

                            //Play sound clip
                            DropBombInstance.Play();
                        }
                    }
                }
            }
        }

        //Animates bombs which appear on screen
        private void BombAnimation()
        {
            BombSmoothness = BombSmoothness + 1;

            if (BombSmoothness % 4 == 0)
            {
                for (i = 0; i <= 29; i++)
                {
                    //Only animation bombs which are on screen
                    if (BombRec[i].Y >= 160)
                    {
                        BombScrX = BombW * BombFrame[i];
                        BombSrcRec[i] = new Rectangle(BombScrX, 0, BombW, BombH);

                        BombFrame[i] = BombFrame[i] + 1;

                        //Resets if it gets pass the last frame
                        if (BombFrame[i] > 3)
                        {
                            BombFrame[i] = 0;
                        }
                    }
                }
            }
        }

        //Pre: The bucket which caught the bomb as well as boolean on/off switch
        //Post: Animates buckets once they catch a bomb
        //Desc: Replaces the source rectangle with a timer until the cycle is finished
        private void CatchAnimation(int BucketNumber, bool Animate)
        {
            if (Animate == true)
            {
                BucketSmoothness = BucketSmoothness + 1;

                if (BucketSmoothness % 3 == 0)
                {
                    BucketScrX[BucketNumber] = BucketW * BucketFrame[BucketNumber];

                    BucketSrcRec[BucketNumber] = new Rectangle(BucketScrX[BucketNumber], 0, BucketW, BucketH);

                    BucketFrame[BucketNumber] = BucketFrame[BucketNumber] + 1;
                }

                //Stops and reset the animation once it hits the last frame
                if (BucketFrame[BucketNumber] == 4)
                {
                    BucketAnimate = false;
                    BucketFrame[BucketNumber] = 0;
                    BucketSrcRec[BucketNumber] = new Rectangle(0, 0, BucketW, BucketH);
                }
            }
        }

        //Pre: The level which is about to start
        //Post: Checks which stage the player is on and applys appropriate speed values
        //Desc: Swicth statement which changes the vaules of and bomb and bomber based on level 
        private void StageCheck(int Level)
        {
            //Reset explosions
            for (i = 0; i < 8; i++)
            {
                ExploDone[i] = false;
                ExploSrcRec[i] = new Rectangle(-50, 0, ExploW, ExploH);
            }

            ExploNumber = 0;

            HitFloor = false;

            //Check which level the player is on
            switch (Level)
            {
                case (1):
                    BombSpeed = RoundOneSpeed;
                    BombFall = RoundOneFall;
                    MoveSpeed = BomberOneMS;
                    break;
                case (2):
                    BombSpeed = RoundTwoSpeed;
                    BombFall = RoundTwoFall;
                    MoveSpeed = BomberTwoMS;
                    break;
                case (3):
                    BombSpeed = RoundThreeSpeed;
                    BombFall = RoundThreeFall;
                    MoveSpeed = BomberThreeMS;
                    break;
            }
        }

        //Checks if the stage is cleared which allow the player to continue
        private void StageClear()
        {
            //Checks when the player has finished the round
            if (CaughtCounter == RoundType * Level)
            {
                CaughtCounter = 0;

                if (RoundType == SemiRound)
                {
                    RoundType = FullRound;
                }
                else
                {
                    Level = Level + 1;
                }

                GameStage = StageOff;
            }
        }

        //Pre: The level which the player is on
        //Post: Tracks the players score
        //Desc: Adds value to the players score
        private void ScoreKeeper(int Level)
        {
            ScoreOnes = ScoreOnes + Level;

            //Insures values can only a single digit number
            if (ScoreOnes >= 10)
            {
                ScoreOnes = ScoreOnes - 10;
                ScoreTens = ScoreTens + 1;
            }

            if (ScoreTens >= 10)
            {
                ScoreTens = ScoreTens - 10;
                ScoreHuns = ScoreHuns + 1;
            }
        }

        //Pre: The ones, tens and hundreds of the score
        //Post: Displays the score to the player
        //Desc: Switch statement whihc draws the appropriate number in the appropriate slot
        private void DisplayScore(int Ones, int Tens, int Huns)
        {
            switch (Ones)
            {
                case (0):
                    NumberSrcRec[0] = new Rectangle((NumberW * 0), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (1):
                    NumberSrcRec[0] = new Rectangle((NumberW * 1), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (2):
                    NumberSrcRec[0] = new Rectangle((NumberW * 2), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (3):
                    NumberSrcRec[0] = new Rectangle((NumberW * 3), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (4):
                    NumberSrcRec[0] = new Rectangle((NumberW * 4), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (5):
                    NumberSrcRec[0] = new Rectangle((NumberW * 5), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (6):
                    NumberSrcRec[0] = new Rectangle((NumberW * 6), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (7):
                    NumberSrcRec[0] = new Rectangle((NumberW * 7), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (8):
                    NumberSrcRec[0] = new Rectangle((NumberW * 8), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
                case (9):
                    NumberSrcRec[0] = new Rectangle((NumberW * 9), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[0], NumberSrcRec[0], Color.White);
                    break;
            }

            switch (Tens)
            {
                case (0):
                    if (Huns != 0)
                    {
                        NumberSrcRec[1] = new Rectangle((NumberW * 0), 0, NumberW, NumberH);
                        spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    }
                    break;
                case (1):
                    NumberSrcRec[1] = new Rectangle((NumberW * 1), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (2):
                    NumberSrcRec[1] = new Rectangle((NumberW * 2), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (3):
                    NumberSrcRec[1] = new Rectangle((NumberW * 3), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (4):
                    NumberSrcRec[1] = new Rectangle((NumberW * 4), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (5):
                    NumberSrcRec[1] = new Rectangle((NumberW * 5), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (6):
                    NumberSrcRec[1] = new Rectangle((NumberW * 6), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (7):
                    NumberSrcRec[1] = new Rectangle((NumberW * 7), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (8):
                    NumberSrcRec[1] = new Rectangle((NumberW * 8), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
                case (9):
                    NumberSrcRec[1] = new Rectangle((NumberW * 9), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[1], NumberSrcRec[1], Color.White);
                    break;
            }

            switch (Huns)
            {
                case (1):
                    NumberSrcRec[2] = new Rectangle((NumberW * 1), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (2):
                    NumberSrcRec[2] = new Rectangle((NumberW * 2), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (3):
                    NumberSrcRec[2] = new Rectangle((NumberW * 3), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (4):
                    NumberSrcRec[2] = new Rectangle((NumberW * 4), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (5):
                    NumberSrcRec[2] = new Rectangle((NumberW * 5), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (6):
                    NumberSrcRec[2] = new Rectangle((NumberW * 6), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (7):
                    NumberSrcRec[2] = new Rectangle((NumberW * 7), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (8):
                    NumberSrcRec[2] = new Rectangle((NumberW * 8), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
                case (9):
                    NumberSrcRec[2] = new Rectangle((NumberW * 9), 0, NumberW, NumberH);
                    spriteBatch.Draw(Number, NumberRec[2], NumberSrcRec[2], Color.White);
                    break;
            }
        }

        //Closes the game when finished or if they run out of lives
        private void GameOver()
        {
            //Checks if they finished the last level or was ran out of lives
            if (Lives < 1 || Level == 4)
            {
                spriteBatch.Draw(GameDone, BackgroundRec, BackgroundSrcRec, Color.White);

                GameStage = StageOff;
            }
        }

        //Replaces falling bombs with new rectangles ready to be animated
        private void Explosion()
        {
            //Move the boms off screen and replace them a hidden rectangles which will be animated later on
            for (i = 0; i < 40; i++)
            {
                if (BombRec[i].Y >= 160)
                {
                    ExploRec[SwapBomb] = new Rectangle(BombRec[i].X, BombRec[i].Y + 10, ExploDrawW, ExploDrawH);

                    BombRec[i] = new Rectangle(-50, -50, BombDrawW, BombDrawH);
                    BombBounds[i] = new Rectangle(-50, -50, BombDrawW, BombDrawH);

                    SwapBomb = SwapBomb + 1;
                }
            }
        }

        //Animates explosions if a bomb is dropped on the floor
        private void ExplosionAnimate()
        {
            ExploSmoothness = ExploSmoothness + 1;

            if (ExploSmoothness % 4 == 0)
            {
                if (ExploNumber < 8)
                {
                    ExploScrX = ExploW * ExploFrame;
                    ExploSrcRec[ExploNumber] = new Rectangle(ExploScrX, 0, ExploW, ExploH);
                    ExploFrame = ExploFrame + 1;

                    //Move the explosion off screen once finished
                    if (ExploFrame > 2)
                    {
                        ExploFrame = 0;

                        ExploSrcRec[ExploNumber] = new Rectangle(-50, 0, ExploW, ExploH);

                        ExploNumber = ExploNumber + 1;
                    }
                }
            }
        }

        //Animates the background if a bomb is dropped on the floor
        private void BackgroundFlicker()
        {
            //Changes the background sprite 4 times in quick secession
            if (FlickerNumber < 4)
            {
                BackgroundSmoothness = BackgroundSmoothness + 1;

                if (BackgroundSmoothness % 4 == 0)
                {
                    BackgroundScrX = BackgroundW * BackgroundFrame;
                    BackgroundSrcRec = new Rectangle(BackgroundScrX, 0, BackgroundW, BackgroundH);
                    BackgroundFrame = BackgroundFrame + 1;

                    if (BackgroundFrame > 1)
                    {
                        BackgroundFrame = 0;
                        FlickerNumber = FlickerNumber + 1;
                    }
                }
            }
            else
            {
                BackgroundSrcRec = new Rectangle(0, 0, BackgroundW, BackgroundH);
            }

        }
    }
}