using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips
{
    public class Factory_Fighter : ShipFactory
    { 
        SpaceShip ShipFactory.BuildSpaceship()
        {
            return new ConcreteShip_Fighter();
        }
    }

   
}
