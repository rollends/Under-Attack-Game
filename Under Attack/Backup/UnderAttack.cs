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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace UnderAttack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class UnderAttack : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Sprites
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        //Textures
        Texture2D backGround = null;
        Texture2D mainMenuBG = null;
        Texture2D btnRestartGame = null;

        //Game Units
        List<GameUnit> units = new List<GameUnit>();
        List<GameUnit> enemies = new List<GameUnit>();
        List<GameUnit> animations = new List<GameUnit>();

        List<GameUnit> deadAnims = new List<GameUnit>();
        List<GameUnit> deadBullets = new List<GameUnit>();
        List<GameUnit> deadUnits = new List<GameUnit>();

        //Player
        Player playerOne = null;
        double timeAlive = 0;

        //Last Updates..
        //double lastMotionUpdate = 0;
        double lastAnimUpdate = 0;
        double lastShotUpdate = 0;

        //Randomness
        Random randGen = new Random();

        //Sounds
        SoundEffect soundBoom = null;

        //Other 
        bool gamePaused = true;
        bool gamePausedKeyDown = false;
        bool gameResetKeyDown = false;
        bool gameScreenieKeyDown = false;
        bool gameShootKeyDown = false;

        bool gameOver = false;
        SpriteFont font = null;
        SpriteFont alertFont = null;

        //Click Regions
        List<Rectangle> clickRegions = new List<Rectangle>();
        List<string> clickCommandList = new List<string>();
        string clickCommand = "";

        //Effects
        Effect eff_time = null;
        RenderTarget2D renderTarget = null;
        Texture2D shaderTexture = null;
        EffectParameter timeParam = null;
        float timePassed = 0;

        Effect eff_wave = null;
        RenderTarget2D renderTarget2 = null;

        public UnderAttack( )
        {
            graphics = new GraphicsDeviceManager(this);
            
            //reset Window Size
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize( )
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        public static RenderTarget2D CloneRenderTarget( GraphicsDevice device, int numberLevels )
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                numberLevels,
                device.DisplayMode.Format,
                device.PresentationParameters.MultiSampleType,
                device.PresentationParameters.MultiSampleQuality
            );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent( )
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Generic
            backGround = this.Content.Load<Texture2D>("Environment\\background");
            
            //Menu Stuff
            mainMenuBG = this.Content.Load<Texture2D>("Other\\menuBG");
            btnRestartGame = this.Content.Load<Texture2D>("Other\\btnRestartGame");

            font = this.Content.Load<SpriteFont>("mainFont");
            alertFont = this.Content.Load<SpriteFont>("alertFont");

            //Effects Rendering
            eff_time = this.Content.Load<Effect>("Effects\\time");
            eff_wave = this.Content.Load<Effect>("Effects\\wave");
            timeParam = eff_time.Parameters["time"];
            renderTarget = CloneRenderTarget(GraphicsDevice, 1);
            renderTarget2 = CloneRenderTarget(GraphicsDevice, 1);
            shaderTexture = new Texture2D(GraphicsDevice,
                                          renderTarget.Width,
                                          renderTarget.Height, 
                                          1,
                                          TextureUsage.None,
                                          renderTarget.Format);

            SetupClickRegions();

            LoadBullet();

            LoadExplosion();

            //Order Matters Here... Last Function called Is ON TOP
            LoadTree();

            LoadClouds();

            LoadEnemies();

            LoadPlayer();
        }

        protected void LoadExplosion( )
        {
            Texture2D explTex = this.Content.Load<Texture2D>("Sprites\\boomAnim");

            Sprite explosion = new Sprite(explTex, new Rectangle(0, 0, 64, 64), 10);

            sprites.Add("explosion", explosion);

            //Sound
            soundBoom = this.Content.Load<SoundEffect>("Sound Effects\\explosion");

        }

        protected void LoadBullet( )
        {
            Texture2D bulltex = this.Content.Load<Texture2D>("Sprites\\bullet");

            sprites.Add("bullet", new Sprite(bulltex, new Rectangle(0, 0, 16, 16), 1));
            sprites.Add("ebullet", sprites["bullet"]);
        }

        protected void LoadEnemies( )
        {
            Texture2D enemyTex = this.Content.Load<Texture2D>("Sprites\\Enemy");

            sprites.Add("enemy", new Sprite(enemyTex, new Rectangle(0, 0, 64, 64), 1));

            //Place 5 Enemies In Total..which will be recycled...
            for (int i = 0; i < 5; i++)
            {
                GameUnit e = new GameUnit("enemy", new Vector2(i * 64 + 70, 0), new Vector2(64));
                e.Velocity = new Vector2(0, 1f);

                e.Position = e.Position + new Vector2(randGen.Next(-10, 10), randGen.Next(-600, -100));
                units.Add(e);
                enemies.Add(e);
            }
        }

        protected void LoadTree( )
        {
            Texture2D treeTex = this.Content.Load<Texture2D>("Environment\\Tree");

            sprites.Add("tree", new Sprite(treeTex, new Rectangle(0, 0, 64, 64), 1));

            for (int i = 0; i < 18; i++)
            {
                for (int y = 0; y < 21; y++)
                {
                    GameUnit tree = new GameUnit("tree", new Vector2(i * 32, y * 32 - 32), new Vector2(64));
                    tree.Velocity = new Vector2(0, 0.5f);

                    tree.Position = tree.Position + new Vector2(randGen.Next(-10,10), 0);
                    units.Add(tree);
                }
            }
        }

        protected void LoadPlayer( )
        {
            Texture2D playerTex = this.Content.Load<Texture2D>("Sprites\\SpaceShip");

            playerOne = new Player("player", new Vector2(200,332), new Vector2(64f, 64f));
            playerOne.Velocity = new Vector2(2, 2);

            sprites.Add("player", new Sprite(playerTex, new Rectangle(0, 0, 64, 64), 1));
            units.Add(playerOne);
        }

        protected void LoadClouds( )
        {
            Texture2D cloud1 = this.Content.Load<Texture2D>("Environment\\Cloud1");
        
            sprites.Add("cloud1", new Sprite(cloud1, new Rectangle(0, 0, 128, 128), 1));

            for (int i = 0; i < 10; i++)
            {
                GameUnit cloud = new GameUnit("cloud1", new Vector2(200, 0), new Vector2(128));
                cloud.Velocity = new Vector2(0, 1f);

                cloud.Position = cloud.Position + new Vector2(randGen.Next(-180,180), randGen.Next(-600,0));
                units.Add(cloud);
            }
        }

        protected void SetupClickRegions( )
        {
            clickRegions.Add(new Rectangle(100, 100, 200, 50));
            clickCommandList.Add("restart");
        }

        public void Reset( )
        {
            gameOver = false;
            gamePaused = true;

            units.Clear();
            enemies.Clear();
            animations.Clear();
            sprites.Clear();

            deadAnims.Clear();
            deadBullets.Clear();
            deadUnits.Clear();

            LoadBullet();
            LoadExplosion();
            LoadTree();
            LoadClouds();
            LoadEnemies();
            LoadPlayer();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent( )
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {

            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if ((padState.Buttons.Back == ButtonState.Pressed)
                || (keyState.IsKeyDown(Keys.Escape)))
            {
                this.Exit();
            }

            //Reset
            if (keyState.IsKeyDown(Keys.R))
            {
                gameResetKeyDown = true;
            }
            else if (keyState.IsKeyUp(Keys.R) && gameResetKeyDown)
            {
                gameResetKeyDown = false;
                
                //Reset The Game Here
                Reset();
            }

            CheckPauseMenu(keyState, Mouse.GetState());

            //Screenie
            if (keyState.IsKeyDown(Keys.PrintScreen))
            {
                gameScreenieKeyDown = true;
            }
            else if (keyState.IsKeyUp(Keys.PrintScreen) && gameScreenieKeyDown)
            {
                gameScreenieKeyDown = false;
                gamePaused = true;

                ResolveTexture2D screenShot = new ResolveTexture2D(graphics.GraphicsDevice, 400, 600, 0, SurfaceFormat.Vector4);

                graphics.GraphicsDevice.ResolveBackBuffer(screenShot);

                //Find A Place TO Save Screenshot :)
                ulong append = 0;

                if ( !Directory.Exists("Screenshots") ) 
                {
                    Directory.CreateDirectory("Screenshots");
                }

                while (File.Exists("Screenshots\\underAttackScreen-" + append + ".png"))
                {
                    append++;
                }

                screenShot.Save("Screenshots\\underAttackScreen-" + append + ".png", ImageFileFormat.Png);

            }

            if ((gameTime.TotalGameTime.TotalMilliseconds - lastAnimUpdate) > 100)
            {

                foreach (GameUnit unit in animations)
                {
                    if (unit.IsAnimating)
                    {
                        unit.CurrentFrame = unit.CurrentFrame + 1;
                        if (unit.CurrentFrame >= (sprites[unit.SpriteName].FrameCount))
                        {
                            deadAnims.Add(unit);
                        }
                    }
                }

                //Time Updating here too - ONLY when game is running
                if (!gameOver && !gamePaused)
                {
                    timePassed++;
                    timeParam.SetValue(timePassed / 20f);
                }
                lastAnimUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (!gameOver && !gamePaused)
            {
                
                //Count time alive
                timeAlive += gameTime.ElapsedGameTime.TotalSeconds;

                //Motion Input
                CheckPlayerInput(keyState);

                //Move all Units where auto-motion is req.
                MoveGameUnits();

                //Check For A Press And Release To Fire...Make Sure Every Release Is atleast 250 Ms apart
                if (keyState.IsKeyDown(Keys.Space) && !gameShootKeyDown && ((gameTime.TotalGameTime.TotalMilliseconds - lastShotUpdate) > 200) )
                {
                    gameShootKeyDown = true;
                }
                else if (keyState.IsKeyUp(Keys.Space) && gameShootKeyDown)
                {
                    GameUnit bullet = new GameUnit("bullet", playerOne.Position - new Vector2(0, 32), new Vector2(4f, 4f));
                    bullet.Velocity = new Vector2(0, -2);
                    units.Add(bullet);

                    //Update last shot time
                    lastShotUpdate = gameTime.TotalGameTime.TotalMilliseconds;

                    //Reset Press variable
                    gameShootKeyDown = false;
                }
                
                //Remove Dead Units
                foreach (GameUnit e in deadBullets)
                {
                    units.Remove(e);
                }

                foreach (GameUnit e in deadUnits)
                {
                    units.Remove(e);
                }

                foreach (GameUnit e in deadAnims)
                {
                    units.Remove(e);
                }

                //Clear already dead units.
                deadBullets.Clear();
                deadUnits.Clear();
                deadAnims.Clear();
            }
            base.Update(gameTime);
        }

        protected void MoveGameUnits( )
        {
            List<GameUnit> newBullets = new List<GameUnit>();

            foreach (GameUnit unit in units)
            {
                if (!unit.SpriteName.Contains("player"))
                {
                    //Move the Game Unit
                    unit.Position = unit.Position + unit.Velocity;

                    //Randomly Shoot at Player [If You ARE AN ENEMY]
                    if ((randGen.Next(0, 100) <= 10) && unit.SpriteName.Contains("enemy"))
                    {
                        //Will The Shot Be Within Means? (Within My Scope Of Fire)
                        GameUnit bullet = new GameUnit("ebullet", unit.Position + new Vector2(0, 32), new Vector2(4f, 4f));
                        Vector2 bulletVelocity = 2 * Vector2.Normalize(playerOne.Position - bullet.Position + new Vector2(randGen.Next(-2, 2), randGen.Next(-2, 2)));

                        if (bulletVelocity.X > -2 && bulletVelocity.X < 2)
                        {
                            //Now To Check Whether I have to cool down or not..
                            if (unit.CoolDown == 0)
                            {
                                //No Cooldown? Cool...Let's Shoot at the player =D 
                                bullet.Velocity = bulletVelocity;
                                newBullets.Add(bullet);
                                unit.CoolDown = 75;
                            }
                            else
                            {
                                unit.CoolDown--;
                            }
                        }
                    }

                    CheckCollisions(unit);
                }
            }

            units.AddRange(newBullets);

            newBullets.Clear();
        }

        protected void CheckPauseMenu( KeyboardState keyState, MouseState mouseState )
        {
            //Pausing
            if (keyState.IsKeyDown(Keys.P))
            {
                gamePausedKeyDown = true;
            }
            else if (keyState.IsKeyUp(Keys.P) && gamePausedKeyDown)
            {
                gamePausedKeyDown = false;
                gamePaused = !gamePaused;

                //If the game is now paused make the mouse visible..otherwise..make it invisible
                if (gamePaused)
                {
                    this.IsMouseVisible = true;
                }
                else
                {
                    //Game was UNpaused...remove any commands not used
                    clickCommand = "";
                    this.IsMouseVisible = false;
                }
            }

            //Clicking Button
            if ( gamePaused ) 
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    for( int i = 0; i < clickRegions.Count; i++ )
                    {
                        if (clickRegions[i].Contains(mouseState.X, mouseState.Y))
                        {
                            //Found Region - Set Command
                            clickCommand = clickCommandList[i];
                            break;
                        }
                    }
                }
                else if ((mouseState.LeftButton == ButtonState.Released) && (clickCommand != "") )
                {
                    //Perform Command Upon Release
                    if (clickCommand == "restart")
                    {
                        Reset();
                    }
                    clickCommand = "";
                }
            }
        }

        private void CheckCollisions( GameUnit unit )
        {
            if (unit.SpriteName.Contains("cloud"))
            {
                if (unit.Bounds.Top > 600)
                {
                    unit.Position = new Vector2(200,-64) + new Vector2(randGen.Next(-180, 180), 0);
                }
            }
            else if (unit.SpriteName.Contains("tree"))
            {
                if (unit.Bounds.Top > 600)
                {
                    unit.Position = unit.Position * Vector2.UnitX  + new Vector2(randGen.Next(-10, 10), -64);
                }
            }
            else if (unit.SpriteName.Contains("ebullet"))
            {
                if (unit.Bounds.Top > 600)
                {
                    deadBullets.Add(unit);
                }
                else
                {
                    if (unit.IsCollidingWith(playerOne))
                    {
                        //GAME OVER D=
                        deadBullets.Add(unit);

                        //Run BOOM Animation
                        GameUnit anim = new GameUnit("explosion", playerOne.Position, new Vector2(64, 64));
                        anim.CurrentFrame = 0;
                        anim.IsAnimating = true;

                        animations.Add(anim);

                        //Run Sound Effect
                        soundBoom.Play();

                        //Dead
                        deadUnits.Add(playerOne);
                        gameOver = true;
                    }
                }
                    
            }
            else if (unit.SpriteName.Contains("bullet"))
            {
                if (unit.Bounds.Top < 0)
                {
                    deadBullets.Add(unit);
                }
                else
                {
                    foreach (GameUnit e in enemies)
                    {
                        if (unit.IsCollidingWith(e))
                        {

                            deadBullets.Add(unit);

                            //Run BOOM Animation
                            GameUnit anim = new GameUnit("explosion", e.Position, new Vector2(64, 64));
                            anim.CurrentFrame = 0;
                            anim.IsAnimating = true;

                            animations.Add(anim);

                            //Run Sound Effect
                            soundBoom.Play();

                            playerOne.Score += (float)(Math.Pow(100, ((double)e.Position.Y / 600d))) * (float)(1f / 600f * (float)e.Position.Y);

                            //Spawn 'New' Enemy
                            e.Position = new Vector2(e.Position.X, 0) + new Vector2(0, randGen.Next(-200, -64));

                            playerOne.Score += (float)timeAlive;
                            break;
                        }
                    }
                }
            }
            else if (unit.SpriteName.Contains("enemy"))
            {
                //ENEMY!
                if (unit.IsCollidingWith(playerOne) || unit.Bounds.Bottom >= 600)
                {
                    //Game Over...
                    //Run BOOM Animation
                    GameUnit anim = new GameUnit("explosion", unit.Position, new Vector2(64, 64));
                    anim.CurrentFrame = 0;
                    anim.IsAnimating = true;

                    GameUnit anim2 = new GameUnit("explosion", playerOne.Position, new Vector2(64, 64));
                    anim2.CurrentFrame = 0;
                    anim2.IsAnimating = true;

                    animations.Add(anim);
                    animations.Add(anim2);

                    //Run Sound Effect
                    soundBoom.Play();
                    soundBoom.Play();

                    deadUnits.Add(playerOne);
                    deadUnits.Add(unit);

                    enemies.Remove(unit);

                    playerOne.Score += (float)timeAlive;
                    gameOver = true;
                }
            }
        }

        protected void CheckPlayerInput( KeyboardState state )
        {
            //Horizontal Movement
            if (state.IsKeyDown(Keys.Right) && playerOne.Position.X < 368)
            {
                playerOne.Position = playerOne.Position + playerOne.Velocity * Vector2.UnitX;
            }
            else if (state.IsKeyDown(Keys.Left) && playerOne.Position.X > 32)
            {
                playerOne.Position = playerOne.Position - playerOne.Velocity * Vector2.UnitX;
            }

            //Vertical Movement
            if (state.IsKeyDown(Keys.Up) && playerOne.Position.Y > 300 )
            {
                playerOne.Position = playerOne.Position - playerOne.Velocity * Vector2.UnitY;
            }
            else if (state.IsKeyDown(Keys.Down) && playerOne.Position.Y < 568)
            {
                playerOne.Position = playerOne.Position + playerOne.Velocity * Vector2.UnitY;
            }

           
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {

            //Cache Render Target
            RenderTarget2D effectSurface = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            GraphicsDevice.SetRenderTarget(0, renderTarget);

            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            //Draw BG
            spriteBatch.Draw(backGround, Vector2.Zero, Color.White);

            foreach (GameUnit unit in units)
            {
                Sprite sprite = sprites[unit.SpriteName];
                Rectangle rect = sprite.GetFrame(unit.CurrentFrame);
                spriteBatch.Draw(sprite.Texture, unit.Bounds, rect, Color.White);
            }

            foreach (GameUnit unit in animations)
            {
                Sprite sprite = sprites[unit.SpriteName];
                Rectangle rect = sprite.GetFrame(unit.CurrentFrame);
                spriteBatch.Draw(sprite.Texture, unit.Bounds, rect, Color.White);
            }

            //Draw The main Menu when paused
            if (gamePaused || gameOver)
            {
                spriteBatch.Draw(mainMenuBG, new Rectangle(0, 0, 400, 600), Color.White);

                //Draw Highlighted Command if Command still not used
                if ( clickCommand == "" )
                    spriteBatch.Draw(btnRestartGame, new Rectangle(100, 100, 200, 50), Color.White);
                else
                    spriteBatch.Draw(btnRestartGame, new Rectangle(100, 100, 200, 50), Color.CornflowerBlue);
            }

            DrawText();

            spriteBatch.End();


            //Switch Back To Other Target 
            GraphicsDevice.SetRenderTarget(0, effectSurface);
            shaderTexture = renderTarget.GetTexture();

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            eff_time.Begin();
            eff_time.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(shaderTexture, Vector2.Zero, Color.White);

            eff_time.CurrentTechnique.Passes[0].End();
            eff_time.End();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText( )
        {
            spriteBatch.Draw(mainMenuBG, new Rectangle(0, 525, 400, 75), new Rectangle(0, 525, 400, 75), Color.White);

            if (gameOver)
            {
                spriteBatch.DrawString(font, "You Lost. " + playerOne.IntegerScore + " Points Earned" + "\nRestart And Press P To Unpause.", new Vector2(0, 525), Color.White);
            }
            else if (gamePaused)
            {
                spriteBatch.DrawString(font, "Score: " + playerOne.IntegerScore + "\nPAUSED - PRESS P TO UNPAUSE", new Vector2(0, 525), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font, "Score: " + playerOne.IntegerScore, new Vector2(0, 525), Color.White);
            }
        }
    }
}
