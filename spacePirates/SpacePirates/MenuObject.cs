using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        
    }

    class MenuObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static MenuObject instance;
        static readonly object padlock = new Object();

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;

        Texture2D banner;

        Vector2 bannerPosition;

        private MenuObject(int w, int h, ContentManager Content)
        {
            MenuObject self = this;

            self.windowWidth = w;
            self.windowHeight = h;

            self.bannerPosition = new Vector2(0, 0);

            self.banner = Content.Load<Texture2D>("banner");
           
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
        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(banner, bannerPosition, Color.White);
        }
    }
}

