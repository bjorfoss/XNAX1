using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    public interface IShipFactory
    {
        /// <summary>
        /// Builds a spaceship.
        /// </summary>
        /// <param name="registration">The link between a ship and its owner</param>
        /// <param name="position">The ship's position</param>
        /// <param name="rotation">Which way the ship is oriented</param>
        /// <returns></returns>
        ISpaceShip BuildSpaceship(Ownership registration, Vector2 position, double rotation);
    }
}
