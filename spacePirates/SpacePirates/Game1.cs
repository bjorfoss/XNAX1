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

namespace SpacePirates
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject gameObject;
        MenuObject menuObject;
        NetworkObject networkObject;

        GameStates currentState;

        Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Components.Add(new GamerServicesComponent(this));
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameObject = GameObject.Instance(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, Content);
            menuObject = MenuObject.Instance(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, Content);

            networkObject = NetworkObject.Instance();

            menuObject.active = true;

            currentState = menuObject;

            // TODO: use this.Content to load your game content here

            song = Content.Load<Song>("Sound/Stratospheres");
            MediaPlayer.Play(song);
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
            else if (!currentState.isActive() && currentState.Equals(menuObject))
            {
                currentState = gameObject;
                gameObject.active = true;
               
            }

           
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            currentState.executeGameLogic((float)gameTime.ElapsedGameTime.TotalSeconds);          

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

           

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            currentState.executeDraw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
