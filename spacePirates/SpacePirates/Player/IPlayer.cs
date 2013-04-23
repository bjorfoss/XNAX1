using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    public interface IPlayer
    {
        /// <summary>
        /// Set the link between a player and a ship. This can be considered to be
        /// like a vehicle registration slip.
        /// </summary>
        /// <param name="registration"></param>
        void SetOwnerShip(Ownership registration);

        /// <summary>
        /// Get the spaceship associated with the player (Human or Ai)
        /// </summary>
        /// <returns>Ship implementing ISpaceship</returns>
        ISpaceShip GetShip();

        /// <summary>
        /// Get the name of the player
        /// </summary>
        /// <returns></returns>
        String GetName();

        /// <summary>
        /// Check if the ship has received an upgrade.
        /// If yes, it's automatically reset to false after this check.
        /// </summary>
        /// <returns></returns>
        bool WasShipUpgraded();

        /// <summary>
        /// Use to request ship upgrade synchronization over network
        /// </summary>
        void ShipUpgraded();

        /// <summary>
        /// Use to print money!
        /// </summary>
        /// <returns></returns>
        int GetCurrency();
        


       // IPlayer createController();
    }
}
