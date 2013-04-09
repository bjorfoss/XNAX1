using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    interface IShipFactory
    {
        ISpaceShip BuildSpaceship(Ownership registration, Vector2 position, double rotation);
    }
}
