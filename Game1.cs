using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Threading.Tasks.Sources;

namespace Circus_Bull_Charlie
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Assets
        Model barrel;
        Model bull;
        Model ground;
        SpriteFont gameFont;
        SpriteFont gameFont12;
        SpriteFont gameFont48;
        Song yeahBoy;
        Song badBone;
        Song scream;
        Texture2D circus;

        //Matrices, vectors, etc.
        Matrix barrelWorld;
        Matrix bullWorld;
        Matrix camera;
        Matrix groundWorld;
        Matrix perspective;
        MouseState mState;
        KeyboardState kState;
        Vector2 barrelPos2D;
        Vector2 bullPos2D;
        Vector3 barrelPos;
        Vector3 bullPos;
        Vector3 cameraPos;
        Vector3 cameraLook;
        Vector3 cameraOrient;
        Viewport view2D;

        //Booleans
        bool gameStart = false;
        bool gameOver = false;
        bool kReleased = true;
        bool mReleased = true;

        //Data variables for matrices and miscellaneous
        float barrelRotY;
        float bullRotY;
        float delta;
        float deltaX;
        float targetRadius;
        float targetRadiusDist;
        float spawnTimer = 2f;
        float timer = 0f;
        float timer2 = 1.5f;

        //Int data variables
        int score = 0;
        int lives = 5;
        int cases;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            barrelPos = new(20f, 0f, -10f);
            //barrelRotX = 0f;
            barrelRotY = 90f;

            bullPos = new(-7f, 0f, -10f);
            //bullRotX = 0f;
            bullRotY = 90f;

            cameraPos = new(0f, 3f, 10f);
            cameraLook = Vector3.Zero;
            cameraOrient = new(0, 10, 10);

            delta = 0.2f;
            deltaX = 20f;
            targetRadius = 100f;

            camera = Matrix.CreateLookAt(cameraPos, cameraLook, cameraOrient);
            perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1f, 0.001f, 1000f);
            view2D = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            barrelWorld = Matrix.CreateScale(0.2f) * Matrix.CreateRotationY(MathHelper.ToRadians(barrelRotY)) *
                Matrix.CreateTranslation(barrelPos);
            barrelPos2D = new Vector2(view2D.Project(barrelPos, perspective, camera, barrelWorld).X,
                view2D.Project(barrelPos, perspective, camera, barrelWorld).Y);
            bullWorld = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(MathHelper.ToRadians(bullRotY))
                * Matrix.CreateTranslation(bullPos);
            bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                view2D.Project(bullPos, perspective, camera, bullWorld).Y);
            groundWorld = Matrix.CreateScale(10f, 0.001f, 10f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            bull = Content.Load<Model>("Animal_Rigged_Zebu_01");
            ground = Content.Load<Model>("Uneven_Ground_Dirt_01");
            barrel = Content.Load<Model>("barrel");
            gameFont = Content.Load<SpriteFont>("gameFont");
            gameFont12 = Content.Load<SpriteFont>("gameFont12x");
            gameFont48 = Content.Load<SpriteFont>("gameFont48x");
            circus = Content.Load<Texture2D>("circus-tent-backgroundUpscaled");
            yeahBoy = Content.Load<Song>("yeah-boy-114748");
            badBone = Content.Load<Song>("badBone");
            scream = Content.Load<Song>("man-scream-121085");
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mState = Mouse.GetState();
            kState = Keyboard.GetState();

            targetRadiusDist = Vector2.Distance(bullPos2D, barrelPos2D);

            if (gameStart)
            {
                if(timer >= 0)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if(spawnTimer > 0)
                {
                    spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                }

                barrelPos.X -= delta;
                barrelPos2D = new Vector2(view2D.Project(barrelPos, perspective, camera, barrelWorld).X,
                    view2D.Project(barrelPos, perspective, camera, barrelWorld).Y);
                if(barrelPos2D.X < -200)
                {
                    barrelPos.X = 20f;
                    barrelPos2D = new Vector2(view2D.Project(barrelPos, perspective, camera, barrelWorld).X,
                        view2D.Project(barrelPos, perspective, camera, barrelWorld).Y);
                }
                barrelWorld = Matrix.CreateScale(0.25f) * Matrix.CreateRotationY(MathHelper.ToRadians(barrelRotY)) *
                    Matrix.CreateTranslation(barrelPos);



                bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                    view2D.Project(bullPos, perspective, camera, bullWorld).Y);
                if((kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) && kReleased == true)
                {
                    bullPos.Z = -5f;
                    bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                    view2D.Project(bullPos, perspective, camera, bullWorld).Y);
                    bullWorld = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(MathHelper.ToRadians(bullRotY))
                        * Matrix.CreateTranslation(bullPos);

                    if((bullPos.Z != barrelPos.Z) && targetRadiusDist < targetRadius)
                    {
                        score++;
                        MediaPlayer.Play(yeahBoy);
                        if(score%10 == 0 && score <= 50)
                        {
                            delta += 0.1f;
                        }
                        if (kReleased == true && bullPos.Z == barrelPos.Z && targetRadiusDist < targetRadius)
                        {
                            lives--; //I spent hours trying to get lives to decrement properly. I could not get it done
                        }
                    }
                    kReleased = false;
                }
                else if((kState.IsKeyUp(Keys.Down) || kState.IsKeyUp(Keys.W)) && kReleased == false)
                {
                    if (timer2 > 0)
                    {
                        timer2 -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if(timer2 < 1)
                    {
                        bullPos.Z = -10f;
                        bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                        view2D.Project(bullPos, perspective, camera, bullWorld).Y);
                        bullWorld = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(MathHelper.ToRadians(bullRotY))
                            * Matrix.CreateTranslation(bullPos);
                        timer2 = 2f;
                    }

                }


                if ((kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) && kReleased == true)
                {
                    bullPos.Z = -15f;
                    bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                    view2D.Project(bullPos, perspective, camera, bullWorld).Y);
                    bullWorld = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(MathHelper.ToRadians(bullRotY))
                        * Matrix.CreateTranslation(bullPos);

                    if ((bullPos.Z != barrelPos.Z) && targetRadiusDist < targetRadius)
                    {
                        score++;
                        MediaPlayer.Play(badBone);
                        if(score%10 == 0 && score<= 50)
                        {
                            delta += 0.1f;
                        }
                        if(kReleased == true && bullPos.Z == barrelPos.Z && targetRadiusDist < targetRadius)
                        {
                            lives--; //I spent hours trying to get lives to decrement properly. I could not get it done
                        }
                    }
                    kReleased = false;
                }
                else if ((kState.IsKeyUp(Keys.Up) || kState.IsKeyUp(Keys.W)) && kReleased == false)
                {
                    if (timer2 > 0)
                    {
                        timer2 -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (timer2 < 1)
                    {
                        bullPos.Z = -10f;
                        bullPos2D = new Vector2(view2D.Project(bullPos, perspective, camera, bullWorld).X,
                        view2D.Project(bullPos, perspective, camera, bullWorld).Y);
                        bullWorld = Matrix.CreateScale(0.005f) * Matrix.CreateRotationY(MathHelper.ToRadians(bullRotY))
                            * Matrix.CreateTranslation(bullPos);
                        timer2 = 2f;
                        kReleased = true;
                    }

                }               

            }
            if(lives == 0)
            {
                gameOver = true;
                MediaPlayer.Play(scream);
            }
            if (!gameStart)
            {
                mState = Mouse.GetState();
                if(mState.LeftButton == ButtonState.Pressed && mReleased == true)
                {
                    gameStart = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _spriteBatch.Begin();

            if (!gameOver) //Checks wether the game is set to play or game over
            {
                // Batch of debug code for testing the position of the HUD elements
                //_spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(10, 25), Color.Black);
                //_spriteBatch.DrawString(gameFont, "Timer: " + (int)timer, new Vector2(10, 0), Color.Black);
                //_spriteBatch.DrawString(gameFont, "Lives: " + lives, new Vector2(10, 50), Color.Black);
                //_spriteBatch.DrawString(gameFont, "Circus Bull Charlie", new Vector2(325, 100), Color.Black);
                

                //If game is set to play will call these statements
                if (gameStart) //if statement calling for the drawing of all assets while the game is in play
                {
                    _spriteBatch.DrawString(gameFont, "Timer: " + (int)timer, new Vector2(10, 0), Color.Black);
                    _spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(10, 25), Color.Black);
                    _spriteBatch.DrawString(gameFont, "Lives: " + lives, new Vector2(10, 50), Color.Black);

                    //More debug codes for position and timers
                    //_spriteBatch.DrawString(gameFont, "Barrel X: " + (int)barrelPos2D.X, new Vector2(10, 75), Color.Red);
                    //_spriteBatch.DrawString(gameFont, "Jump Timer: " + (int)timer2, new Vector2(10, 100), Color.Red);
                    //_spriteBatch.DrawString(gameFont, "Spawn Timer:" + (int)spawnTimer, new Vector2(10, 125), Color.Red);

                    //ground.Draw(groundWorld, camera, perspective);
                    ModelDraw(ground, groundWorld, camera, perspective, new Vector3(1, 0.804f, 0.851f), true);
                    //barrel.Draw(barrelWorld, camera, perspective);
                    ModelDraw(barrel, barrelWorld, camera, perspective, new Vector3(277, 277, 277), true);
                    //bull.Draw(bullWorld, camera, perspective);
                    ModelDraw(bull, bullWorld, camera, perspective, new Vector3(0.771f, 0.578f, 0.163f), true) ;
                }
                if (!gameStart) //if statement that calls to draw the start screen
                {
                    _spriteBatch.Draw(circus, new Vector2(), Color.White);
                    _spriteBatch.DrawString(gameFont48, "Circus Bull Charlie", new Vector2(150, 100), Color.Black);
                    _spriteBatch.DrawString(gameFont12, "Developed by", new Vector2(685, 435), Color.Black);
                    _spriteBatch.DrawString(gameFont12, "Nicholas Cordial", new Vector2(675, 455), Color.Black);
                }
            }
            else
            {
                //If game is set to game over, will call these statements
                _spriteBatch.Draw(circus, new Vector2(), Color.White);
                _spriteBatch.DrawString(gameFont48, "Game Over", new Vector2(250, 100), Color.Black);
                _spriteBatch.DrawString(gameFont48, "Play Again?", new Vector2(250, 225), Color.Black);
                _spriteBatch.DrawString(gameFont, "Total Time: " + (int)timer, new Vector2(250, 25), Color.Black);
                _spriteBatch.DrawString(gameFont, "High Score: " + score, new Vector2(450, 25), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ModelDraw(Model model, Matrix world, Matrix camera, Matrix projection, Vector3 color, bool shadows)
        {
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = camera;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
