using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.spaceShips
{
    public class Factory_Fighter : ShipFactory
    { 
        ISpaceShip ShipFactory.BuildSpaceship(Vector2 position, double rotation)
        {
            return new ConcreteShip_Fighter(position, rotation);
        }
    }

   
}
