using System;
using System.Configuration;
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
using SpacePirates.Utilities;

namespace SpacePirates
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public GraphicsDevice graphicsDevice;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject gameObject;
        MenuObject menuObject;
        NetworkObject networkObject;

        GameStates currentState;

        double muteCd;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Components.Add(new GamerServicesComponent(this));

            Content.RootDirectory = "Content";
            muteCd = 500;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Deal with configuration file - should be saved in
            //local appdata/org.ntnu.tdt4240.xna1/xxx/xxx/user.config
            //setting values to ensure local config values
            Settings1 s = Settings1.Default;
            s.ResolutionHorizontal = s.ResolutionHorizontal;
            s.ResolutionVertical = s.ResolutionVertical;
            s.Fullscreen = s.Fullscreen;
            s.Save();
            
            graphics.PreferredBackBufferWidth = s.ResolutionHorizontal;
            graphics.PreferredBackBufferHeight = s.ResolutionVertical;
            graphics.IsFullScreen = s.Fullscreen;
            graphics.ApplyChanges();
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

            gameObject = GameObject.Instance(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, Content, GraphicsDevice);
            menuObject = MenuObject.Instance(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, Content);

            networkObject = NetworkObject.Instance();

            menuObject.active = true;

            currentState = menuObject;

            // TODO: use this.Content to load your game content here

            MediaPlayer.Play((Content.Load<Song>("Sound/Stratospheres")));
            MediaPlayer.IsRepeating = true;
        }

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
            if (!currentState.isActive() && currentState.Equals(gameObject))
            {
                currentState = menuObject;
                menuObject.active = true;
            }
            else if (!currentState.isActive() && currentState.Equals(menuObject) || (NetworkObject.Instance().getNetworksession() != null && NetworkObject.Instance().getNetworksession().SessionState == NetworkSessionState.Playing))
            {
                currentState = gameObject;
                gameObject.active = true;
               
            }

           
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            currentState.executeGameLogic(gameTime);

            if(Keyboard.GetState().IsKeyDown(Keys.M) && muteCd <= 0){
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                muteCd = 500;
            }
            else if (muteCd > 0)
            {
                muteCd -= gameTime.ElapsedGameTime.TotalMilliseconds;
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

            //Use clamp on menu screen to support the limited Reach api (it contains
            //non-power of two textures) for older systems
            if (currentState is GameObject)
            {
                //add samplerstate to wrap background
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            }
            else if (currentState is MenuObject)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            }

            currentState.executeDraw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
