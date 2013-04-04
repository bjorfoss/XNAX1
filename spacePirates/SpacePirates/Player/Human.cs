using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Human : IPlayer
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
        /// Handle input logic and call Spaceship interface methods.
        /// </summary>
        public void HandleInput()
        {
        }

        void HandleKeyboardInput()
        {
        }

        void HandleGamepadInput()
        {
        }

    }
}
