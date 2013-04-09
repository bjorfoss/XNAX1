using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.spaceShips
{
    interface IShipFactory
    {
        ISpaceShip BuildSpaceship(Vector2 position, double rotation);
    }
}
