using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    interface Player
    {
        Ownership registration;

        void SetOwnerShip(Ownership registration);

        public SpaceShip GetShip()
        {
            return registration.GetShip();
        }
    }
}
