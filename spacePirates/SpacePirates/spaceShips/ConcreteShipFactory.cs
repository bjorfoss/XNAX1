﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    public class Factory_Fighter : IShipFactory
    {
        public Factory_Fighter() { }

        public ISpaceShip BuildSpaceship(Ownership ownership, Vector2 position, double rotation)
        {
            return new ConcreteShip_Fighter(ownership, position, rotation);
        }
    }

    //public class Factory_Bomber : IShipFactory
    //{
    //    ISpaceShip IShipFactory.BuildSpaceship(Vector2 position, double rotation)
    //    {
    //        return new ConcreteShip_Bomber(position, rotation);
    //    }
    //}
   
}
