using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace SpacePirates
{
    class Button
    {
        Texture2D normal;
        Texture2D pressed;
        Texture2D hover;
        Texture2D released;

        Vector2 position;
        
        Texture2D currentState;

        public Button(Texture2D normal, Texture2D pressed, Texture2D hover, Texture2D released, Vector2 position)
        {
            Button self     = this;
            self.normal     = normal;
            self.pressed    = pressed;
            self.hover      = hover;
            self.released   = released;

            self.currentState = self.normal;

            self.position = position;
        }

        public Button(Texture2D normal, Texture2D pressed, Texture2D hover, Vector2 position)
        {
            new Button(normal, pressed, hover, normal, position);
        }

        public Button(Texture2D normal, Texture2D pressed, Vector2 position)
        {
            new Button(normal, pressed, normal, normal, position);
        }

        public Button(Texture2D normal, Vector2 position)
        {
            new Button(normal, normal, normal, normal, position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentState, position, Color.White);
        }
    }

    class MenuObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static MenuObject instance;
        static readonly object padlock = new Object();

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;

        //Placeholders for keeping track of menu positions.
        private int mainmenu = 0;
        private int createdlobby = 1;
        private int joinedlobby = 2;

        private int currentMenu = 0; //Main menu.

        //Displayed game banner
        Texture2D banner;

        //"Buttons" for the main menu. Could likely get converted into true buttons but I've so far been unable to draw them properly.
        Texture2D newSession;
        Vector2 newSessionPos;
        Texture2D joinSession;
        Vector2 joinSessionPos;
        Texture2D quitSession;
        Vector2 quitSessionPos;
        Texture2D backButton;
        Vector2 backButtonPos;
        Texture2D startGame;
        Vector2 startGamePos;
        Texture2D readyButton;
        Vector2 readyButtonPos;

        Button newGame;

        Vector2 bannerPosition;

        private MenuObject(int w, int h, ContentManager Content)
        {
            MenuObject self = this;

            self.windowWidth = w;
            self.windowHeight = h;

            self.bannerPosition = new Vector2(0, 0);

            self.banner = Content.Load<Texture2D>("banner");

            self.newSession = Content.Load<Texture2D>("MenuButtons/NewSession");
            self.newSessionPos = new Vector2(10, banner.Height + 10);
            self.joinSession = Content.Load<Texture2D>("MenuButtons/JoinSession");
            self.joinSessionPos = new Vector2(10, newSessionPos.Y + newSession.Height + 10);
            self.quitSession = Content.Load<Texture2D>("MenuButtons/Quit");
            self.quitSessionPos = new Vector2(10, joinSessionPos.Y + joinSession.Height + 10);
            self.startGame = Content.Load<Texture2D>("MenuButtons/StartGame");
            self.startGamePos = new Vector2(10, banner.Height + 10);
            self.readyButton = Content.Load<Texture2D>("MenuButtons/Ready");
            self.readyButtonPos = new Vector2(10, banner.Height + 10);
            self.backButton = Content.Load<Texture2D>("MenuButtons/BackButton");
            self.backButtonPos = new Vector2(10, windowHeight - 10 - backButton.Height);
            self.newGame = new Button(newSession, new Vector2(10, quitSessionPos.Y + quitSession.Height + 10));
           
        }


        public static MenuObject Instance(int w, int h, ContentManager Content)
        {
            lock (padlock) {
                if (instance == null) {
                    instance = new MenuObject(w, h, Content);
                }
                return instance;
            }
        }

        public void executeGameLogic(float elapsed)
        {
            if(currentMenu == mainmenu)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.N))//New Session.
                {
                    currentMenu = createdlobby;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.J))//Join Session.
                {
                    currentMenu = joinedlobby;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Q))//Quit game.
                {
                    Environment.Exit(0);
                }
            }
            else if (currentMenu == createdlobby)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.B))//Back to Main menu.
                {
                    currentMenu = mainmenu;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))//Start the game?
                {
                        
                }
            }
            else if (currentMenu == joinedlobby)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.B))//Back to Main menu.
                {
                    currentMenu = mainmenu;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.R))//Indicate ready.
                {

                }
            }
        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            if (currentMenu == mainmenu)
            {
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(newSession, newSessionPos, Color.White);
                spriteBatch.Draw(joinSession, joinSessionPos, Color.White);
                spriteBatch.Draw(quitSession, quitSessionPos, Color.White);
                //newGame.Draw(spriteBatch);
                //spriteBatch.Draw(self.newGame.currentState, self.newGame.position, Color.White);
            }
            else if (currentMenu == createdlobby)
            {
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(startGame, startGamePos, Color.White);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);
            }
            else if (currentMenu == joinedlobby)
            {
                spriteBatch.Draw(banner, bannerPosition, Color.White);
                spriteBatch.Draw(readyButton, readyButtonPos, Color.White);
                spriteBatch.Draw(backButton, backButtonPos, Color.White);
            }
        }
    }
}

