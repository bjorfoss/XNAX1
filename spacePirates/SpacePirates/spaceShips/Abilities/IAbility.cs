using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates.spaceShips
{
    public interface IAbility
    {
        void Activate(GameTime gameTime);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch batch, SpaceShip ship);

        string GetName();

        bool getActive();

        double getTimer();
    }
}
