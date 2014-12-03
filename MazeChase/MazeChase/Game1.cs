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

#region REQUIREMENTS

/*
General:
•	The game will have appropriate sound.

Chase Bots:
•	There need to be at least four (4) different flavors of chase bot.  Each type of chase bot must employ a different chase strategy.  Chase bots may also cooperate with each other (share information) to a limited degree.

Extra Credit:
•	Implementation of levels

*/

#endregion

namespace MazeChase
{
    public enum direction { STILL, UP, DOWN, LEFT, RIGHT };

    enum mode { SCATTER, ATTACK, FLEE, REGENERATE };

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H",
           "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", 
           "X", "Y", "Z" };

        public string[,] highScores = new string[10, 3];
        public List<HighScore> highScoreList = new List<HighScore>();
        public List<HighScore> secondList = new List<HighScore>();
        public bool highScoreFix = false;

        int currentIndex = 0;
        int[] hiScoreIndex = { 0, 0, 0 };
        string hiScoreName = "";
        
        int gameMode = 1;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager inputManager;
        MapManager mapManager;
        ScoreManager scoreManager;
        Player player;
        Ghost red, blue, pink, orange;
        Vector2 origin;

        SoundEffect deathSound, newLifeSound, eatSound, beginningMusic, congrats;
        SoundEffectInstance deathSoundInstance, newLifeInstance, eatInstance, musicInstance, congratsInstance;

        KeyboardState keyPresses, oldKeyPresses;

        SpriteFont bigFont, hugeFont;

        public static bool pause = true;

        // Textures
        Texture2D ghostTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize mapManager
            mapManager = new MapManager(this.Content, this.GraphicsDevice, this.scoreManager);

            // Load map content
            mapManager.LoadContent();
            mapManager.Initialize();

            // Initialize InputManager
            inputManager = new InputManager();


            // Initialize Ghost
            red = new Ghost(this.Content, 1, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((62 * 16) + 8, (0 * 16) + 8));// top right
            blue = new Ghost(this.Content, 2, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((62 * 16) + 8, (44 * 16) + 8));// bottom right
            pink = new Ghost(this.Content, 3, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((0 * 16) + 8, (0 * 16) + 8));// top left
            orange = new Ghost(this.Content, 4, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((0 * 16) + 8, (44 * 16) + 8));// botttom left
        }

        protected override void LoadContent()
        {
            deathSound = Content.Load<SoundEffect>(@"pacman_death");
            deathSoundInstance = deathSound.CreateInstance();

            newLifeSound = Content.Load<SoundEffect>(@"pacman_extrapac");
            newLifeInstance = newLifeSound.CreateInstance();  

            congrats = Content.Load<SoundEffect>(@"fanfare");
            congratsInstance = congrats.CreateInstance();

            bigFont = Content.Load<SpriteFont>(@"LargeFont");
            hugeFont = Content.Load<SpriteFont>(@"HugeFont");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize ScoreManager
            scoreManager = new ScoreManager(Content.Load<SpriteFont>("SpriteFont"), newLifeInstance);

            // Viewport Origin
            origin = new Vector2(400, 336);

            // Load Textures
            ghostTexture = Content.Load<Texture2D>(@"Ghosts2");

            importHighScores();

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                exportHighScores();
                this.Exit();
            }

            //Console.WriteLine(gameMode);
            //Console.WriteLine(scoreManager.getRemainingLives());

            keyPresses = Keyboard.GetState();

            if (scoreManager.getRemainingLives() == 0)
            {
                if (checkHighScore(scoreManager.getScore()))
                {
                    int slot = 10;

                    for (int i = 9; i >= 0; i--)
                    {
                        if (scoreManager.getScore() > highScoreList[i].Score)
                        {
                            slot = i;
                        }
                    }

                    if (highScoreFix == true)
                    {
                        if (slot != 0)
                        {
                            for (int i = 0; i < slot; i++)
                            {
                                secondList.Insert(i, highScoreList[i]);
                            }
                        }
                        secondList.Insert(slot, new HighScore(slot + 1, getNameReturn(), scoreManager.getScore()));
                        for (int i = slot; i < 10; i++)
                        {
                            secondList.Insert(i + 1, new HighScore(highScoreList[i].Rank + 1, highScoreList[i].Name, highScoreList[i].Score));
                        }

                        highScoreList = new List<HighScore>(secondList);
                        scoreManager.setLives(-1);
                        highScoreFix = false;
                    }
                    else
                    {
                        getName();

                        if (nameReturn() != "ERR" && nameReturn() != "")
                        {
                            highScoreFix = true;
                        }
                    }


                }

                if (gameMode == 1)
                {
                    if (keyPresses.IsKeyDown(Keys.Enter) && keyPresses != oldKeyPresses)
                    {
                        scoreManager.setLives(3);
                        scoreManager.reset();
                        //currentLevel = 0
                        player = new Player(this.Content, inputManager, mapManager, scoreManager, origin);
                        red = new Ghost(this.Content, 1, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((61 * 16) + 8, (1 * 16) + 8));// top right
                        blue = new Ghost(this.Content, 2, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((61 * 16) + 8, (43 * 16) + 8));// bottom right
                        pink = new Ghost(this.Content, 3, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((1 * 16) + 8, (1 * 16) + 8));// top left
                        orange = new Ghost(this.Content, 4, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((1 * 16) + 8, (43 * 16) + 8));// botttom left
                        gameMode = 2;
                        hiScoreName = "";
                        currentIndex = 0;
                        hiScoreIndex = new int[] { 0, 0, 0 };
                    }
                    if (keyPresses.IsKeyDown(Keys.Space) && keyPresses != oldKeyPresses)
                    {
                        gameMode = 3;
                    }
                }
                else if (gameMode == 2)
                {
                    
                }
                else if (gameMode == 3)
                {
                    if (keyPresses.IsKeyDown(Keys.Enter) && keyPresses != oldKeyPresses)
                    {
                        scoreManager.setLives(3);
                        scoreManager.reset();
                        //currentLevel = 0
                        player = new Player(this.Content, inputManager, mapManager, scoreManager, origin);
                        red = new Ghost(this.Content, 1, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((61 * 16) + 8, (1 * 16) + 8));// top right
                        blue = new Ghost(this.Content, 2,  mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((61 * 16) + 8, (43 * 16) + 8));// bottom right
                        pink = new Ghost(this.Content, 3, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((1 * 16) + 8, (1 * 16) + 8));// top left
                        orange = new Ghost(this.Content, 4, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((1 * 16) + 8, (43 * 16) + 8));// botttom left
                        gameMode = 2;
                        hiScoreName = "";
                        currentIndex = 0;
                        hiScoreIndex = new int[] { 0, 0, 0 };
                    }
                    if (keyPresses.IsKeyDown(Keys.Space) && keyPresses != oldKeyPresses)
                    {
                        gameMode = 1;
                    }
                }
                else if (gameMode == 4)
                {
                    // Enter initials
                    if (keyPresses.IsKeyDown(Keys.Enter) && keyPresses != oldKeyPresses)
                    {
                        gameMode = 3;
                        hiScoreName = Game1.alphabet[hiScoreIndex[0]] + Game1.alphabet[hiScoreIndex[1]] + Game1.alphabet[hiScoreIndex[2]];
                    }

                    if (keyPresses.IsKeyDown(Keys.Left) && keyPresses != oldKeyPresses)
                    {
                        currentIndex--;
                        if (currentIndex < 0)
                        {
                            currentIndex = 2;
                        }
                    }
                    else if (keyPresses.IsKeyDown(Keys.Right) && keyPresses != oldKeyPresses)
                    {
                        currentIndex++;
                        if (currentIndex > 2)
                        {
                            currentIndex = 0;
                        }
                    }
                    else if (keyPresses.IsKeyDown(Keys.Down) && keyPresses != oldKeyPresses)
                    {
                        hiScoreIndex[currentIndex]--;
                        if (hiScoreIndex[currentIndex] < 0)
                        {
                            hiScoreIndex[currentIndex] = 25;
                        }
                    }
                    else if (keyPresses.IsKeyDown(Keys.Up) && keyPresses != oldKeyPresses)
                    {
                        hiScoreIndex[currentIndex]++;
                        if (hiScoreIndex[currentIndex] > 25)
                        {
                            hiScoreIndex[currentIndex] = 0;
                        }
                    }
                }
            }
            else if (scoreManager.getRemainingLives() == -1)
            {
                if (keyPresses.IsKeyDown(Keys.Enter) && keyPresses != oldKeyPresses)
                {
                    scoreManager.setLives(3);
                    scoreManager.reset();
                    //currentLevel = 0
                    player = new Player(this.Content, inputManager, mapManager, scoreManager, origin);
                    red = new Ghost(this.Content, 1, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((61 * 16) + 8, (1 * 16) + 8));// top right
                    blue = new Ghost(this.Content, 2, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((61 * 16) + 8, (43 * 16) + 8));// bottom right
                    pink = new Ghost(this.Content, 3, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((1 * 16) + 8, (1 * 16) + 8));// top left
                    orange = new Ghost(this.Content, 4, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((1 * 16) + 8, (43 * 16) + 8));// botttom left
                    gameMode = 2;
                    hiScoreName = "";
                    currentIndex = 0;
                    hiScoreIndex = new int[] { 0, 0, 0 };
                }
                if (keyPresses.IsKeyDown(Keys.Space) && keyPresses != oldKeyPresses)
                {
                    gameMode = (gameMode == 1) ? 3 : 1;
                }
            }

            if (gameMode == 1)
            {
                // Menu mode
            }
            else if (gameMode == 2)
            {
                // Game mode

                if (scoreManager.getRemainingLives() < 0)
                {
                    gameMode = 3;
                }
                else
                {
                    scoreManager.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.P) && keyPresses != oldKeyPresses)
            {
                pause = !pause;
            }

            if (!pause && !player.isDead)
            {
                // Check for input
                inputManager.Update(gameTime);

                // Update map
                mapManager.Update(gameTime);
                if (mapManager.isFoodUnderPlayer((int)player.getPosition().X, (int)player.getPosition().Y) == 20)
                {
                    player.modeSwap(500);
                }

                // Update player
                player.Update(gameTime);

                // Update ghosts
                red.Update(gameTime);
                blue.Update(gameTime);
                pink.Update(gameTime);
                orange.Update(gameTime);
            }
            else if (player.isDead)
            {
                player.deathAnimation(gameTime);
                        red = new Ghost(this.Content, 1, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 0), new Vector2(7, 0), new Vector2((28 * 16) + 8, (22 * 16) + 8), new Vector2((61 * 16) + 8, (1 * 16) + 8));// top right
                        blue = new Ghost(this.Content, 2, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 1), new Vector2(7, 1), new Vector2((31 * 16) + 8, (19 * 16) + 8), new Vector2((61 * 16) + 8, (43 * 16) + 8));// bottom right
                        pink = new Ghost(this.Content, 3, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 2), new Vector2(7, 2), new Vector2((34 * 16) + 8, (19 * 16) + 8), new Vector2((1 * 16) + 8, (1 * 16) + 8));// top left
                        orange = new Ghost(this.Content, 4, mapManager, scoreManager, player, ghostTexture, new Vector2(0, 3), new Vector2(7, 3), new Vector2((37 * 16) + 8, (22 * 16) + 8), new Vector2((1 * 16) + 8, (43 * 16) + 8));// botttom left
                    }
            }

            }
            else if (gameMode == 3)
            {
                // High scores
            }
            else if (gameMode == 4)
            {
                //????
            }

            oldKeyPresses = keyPresses;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw sprites
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);


            if (gameMode == 1)
            {
                spriteBatch.DrawString(hugeFont, "MAZE CHASE", new Vector2(Window.ClientBounds.Width / 2 - hugeFont.MeasureString("MAZE CHASE").X / 2, Window.ClientBounds.Height / 4), Color.White);

                spriteBatch.DrawString(bigFont, "Press Enter to Play", new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("Press Enter to Play").X / 2, Window.ClientBounds.Height / 2 - bigFont.MeasureString("Press Enter to Play").Y / 2), Color.White);
                spriteBatch.DrawString(bigFont, "Press Space to View High Scores", new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("Press Space to View High Scores").X / 2, Window.ClientBounds.Height / 2 + bigFont.MeasureString("Press Enter to Play").Y - bigFont.MeasureString("Press Space to View High Scores").Y / 2), Color.White);
            }
            else if (gameMode == 2)
            {
                // Draw map
                mapManager.Draw();

                player.Draw(spriteBatch);
                if (!player.isDead)
                {
                    red.Draw(spriteBatch);
                    blue.Draw(spriteBatch);
                    pink.Draw(spriteBatch);
                    orange.Draw(spriteBatch);
                }

                scoreManager.Draw(spriteBatch);

                if (pause)
                {
                    spriteBatch.DrawString(hugeFont, "PAUSED", new Vector2(Window.ClientBounds.Width / 2 - hugeFont.MeasureString("PAUSED").X / 2, Window.ClientBounds.Height / 2 - hugeFont.MeasureString("PAUSED").Y / 2), Color.Gold);
                }
                
            }
            else if (gameMode == 3)
            {
                for (int i = 0; i < 10; i++)
                {
                    string a = highScoreList[i].Rank + " ";
                    string b = highScoreList[i].Name + " ";
                    string c = highScoreList[i].Score + " ";

                    spriteBatch.DrawString(bigFont, a, new Vector2(Window.ClientBounds.Width / 3, 28.0f * i), Color.White);
                    spriteBatch.DrawString(bigFont, b, new Vector2(Window.ClientBounds.Width / 2, 28.0f * i), Color.White);
                    spriteBatch.DrawString(bigFont, c, new Vector2(2 * Window.ClientBounds.Width / 3, 28.0f * i), Color.White);
                }

                spriteBatch.DrawString(bigFont, "Press Enter to Play", new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("Press Enter to Play").X / 2, 3 * Window.ClientBounds.Height / 4 - bigFont.MeasureString("Press Enter to Play").Y / 2), Color.White);
                spriteBatch.DrawString(bigFont, "Press Space to Return to Menu", new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("Press Space to Return to Menu").X / 2, 3 * Window.ClientBounds.Height / 4 + bigFont.MeasureString("Press Enter to Play").Y - bigFont.MeasureString("Press Space to View High Scores").Y / 2), Color.White);
            }
            else if (gameMode == 4)
            {
                Color[] drawColor = { Color.White, Color.White, Color.White };
                drawColor[currentIndex] = Color.Red;

                spriteBatch.DrawString(bigFont, "GAME OVER - SCORE: " + scoreManager.getScore(), new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("GAME OVER - SCORE: " + scoreManager.getScore()).X / 2, Window.ClientBounds.Height / 4), Color.White);
                spriteBatch.DrawString(bigFont, "Enter Name: ", new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString("Enter Name: ").X / 2, Window.ClientBounds.Height / 2), Color.White);
                spriteBatch.DrawString(bigFont, alphabet[hiScoreIndex[0]], new Vector2(Window.ClientBounds.Width / 2 - bigFont.MeasureString(alphabet[hiScoreIndex[0]] + " ").X, Window.ClientBounds.Height / 2 + bigFont.MeasureString("Enter Name: ").Y + 20.0f), drawColor[0]);
                spriteBatch.DrawString(bigFont, alphabet[hiScoreIndex[1]], new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2 + bigFont.MeasureString("Enter Name: ").Y + 20.0f), drawColor[1]);
                spriteBatch.DrawString(bigFont, alphabet[hiScoreIndex[2]], new Vector2(Window.ClientBounds.Width / 2 + bigFont.MeasureString(alphabet[hiScoreIndex[2]] + " ").X, Window.ClientBounds.Height / 2 + bigFont.MeasureString("Enter Name: ").Y + 20.0f), drawColor[2]);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public struct HighScore
        {
            public int Rank;
            public string Name;
            public int Score;

            public HighScore(int myRank, string myName, int myScore)
            {
                Rank = myRank;
                Name = myName;
                Score = myScore;
            }

            public void fix()
            {
                this.Rank++;
            }
        }

        bool checkHighScore(int checkScore)
        {
            return checkScore > highScoreList.Last().Score;
        }

        void importHighScores()
        {
            string[] parts;

            Stream stream = TitleContainer.OpenStream("scores.txt");
            StreamReader reader = new StreamReader(stream);

            do
            {
                parts = reader.ReadLine().Split(' ');
                highScoreList.Add(new HighScore(Convert.ToInt32(parts[0]), parts[1], Convert.ToInt32(parts[2])));
            } while (reader.Peek() != -1);

            stream.Close();
        }

        void exportHighScores()
        {
            FileStream fs = new FileStream("scores.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < 10; i++)
            {
                sw.WriteLine(highScoreList[i].Rank + " " + highScoreList[i].Name + " " + highScoreList[i].Score);
            }

            sw.Close();
            fs.Close();
        }

        public string getNameReturn()
        {
            if (nameReturn() != "")
            {
                return nameReturn();
            }

            return "ERR";
        }
        
        public void getName()
        {
            if (gameMode != 3 && gameMode != 4)
            {
                congratsInstance.Play();
                gameMode = 4;
            }

        }

        public string nameReturn()
        {
            return hiScoreName;
        }
    }
}
