using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    interface IPlayer
    {
        void SetOwnerShip(Ownership registration);

        ISpaceShip GetShip();

        String GetName();
    }
}
