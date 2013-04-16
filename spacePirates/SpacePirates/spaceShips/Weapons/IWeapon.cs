using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.spaceShips
{
    public interface IWeapon
    {
        void Fire(GameTime gameTime, Unit ship);
    }
}
