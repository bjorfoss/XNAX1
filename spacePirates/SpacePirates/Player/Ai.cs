using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Ai : IPlayer
    {
        Ownership ownerLink;
        String name;

        public String GetName()
        {
            return name;
        }

        public ISpaceShip GetShip()
        {
            return ownerLink.GetShip();
        }

        public void SetOwnerShip(Ownership ownerLink)
        {
            this.ownerLink = ownerLink;
        }

        /// <summary>
        /// Determine general goal/strategy
        /// </summary>
        void MakeGeneralPlan()
        {
        }

        /// <summary>
        /// Make local decisions supporting the strategy
        /// </summary>
        void MakeLocalPlan()
        {
        }




        public IPlayer createController()
        {
            throw new NotImplementedException();
        }
    }
}
