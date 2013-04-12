using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpacePirates
{
    interface GameStates
    {

        void executeGameLogic(GameTime gameTime);

        void executeDraw(SpriteBatch spriteBatch);

        bool isActive();

    }
}
