using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    interface IPlayer
    {
        public void SetOwnerShip(Ownership registration);

        public ISpaceShip GetShip();

        public String GetName();
    }
}
