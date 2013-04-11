using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    public interface IPlayer
    {
        void SetOwnerShip(Ownership registration);

        ISpaceShip GetShip();

        String GetName();

       // IPlayer createController();
    }
}
