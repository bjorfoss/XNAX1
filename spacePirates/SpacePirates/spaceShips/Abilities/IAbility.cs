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

<<<<<<< HEAD
        string GetType();
=======
        bool getActive();

        double getTimer();
>>>>>>> 7ce7a18626b68de95317e4fb7619845c179282d8
    }
}
