using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates
{
    class Level
    {
        Texture2D background;
        HashSet<Rectangle> foreground;

        public void Draw(SpriteBatch sb);
    }
}
