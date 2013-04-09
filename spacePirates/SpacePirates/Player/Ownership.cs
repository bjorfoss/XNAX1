using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    /// <summary>
    /// A link between a spaceship and its owner.
    /// Consider it to be like a vehicle registration.
    /// </summary>
    public class Ownership
    {
        private IPlayer owner;
        private ISpaceShip spaceShip;

        public Ownership() {}

        public ISpaceShip GetShip() {
            return spaceShip;
        }

        public void SetShip(ISpaceShip ship)
        {
            this.spaceShip = ship;    
        }

        public IPlayer GetOwner()
        {
            return owner;
        }

        public void SetOwner(IPlayer owner) {
            this.owner = owner;
        }

        /// <summary>
        /// Check if both owner and ship is set
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return (spaceShip != null && owner != null);
        }
    }
}
