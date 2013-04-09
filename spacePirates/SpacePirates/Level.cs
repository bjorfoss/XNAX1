using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpacePirates
{
    class Level
    {
        Texture2D background;
        HashSet<Rectangle> foreground;

        public Level(ContentManager Content)
        {
            Level self = this;

            self.background = Content.Load<Texture2D>("background");
        }


        public void executeDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0,0), Color.White);
        }
    }
}
