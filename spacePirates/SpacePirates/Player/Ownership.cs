using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Ownership
    {
        Player owner;
        SpaceShip spaceShip;

        public SpaceShip GetShip() {
            return spaceShip;
        }

        public Player GetOwner()
        {
            return owner;
        }
    }
}
