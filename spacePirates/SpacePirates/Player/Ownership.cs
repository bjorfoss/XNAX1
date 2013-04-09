using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Ownership
    {
        IPlayer owner;
        ISpaceShip spaceShip;

        public Ownership(IPlayer owner, ISpaceShip spaceShip)
        {
            this.owner = owner;
            this.spaceShip = spaceShip;
        }

        public ISpaceShip GetShip() {
            return spaceShip;
        }

        public IPlayer GetOwner()
        {
            return owner;
        }
    }
}
