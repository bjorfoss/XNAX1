using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    public class ConcreteShipFactory
    {
        private Dictionary<String, IShipFactory> factories;
        private static ConcreteShipFactory instance;
        static readonly object padlock = new Object();

        private ConcreteShipFactory()
        {
            factories = new Dictionary<string, IShipFactory>();
            factories.Add("fighter", new Factory_Fighter());
            factories.Add("eigthwing", new Factory_Eightwing());
        }

        /// <summary>
        /// Get an instance of the factory
        /// </summary>
        /// <returns></returns>
        public static ConcreteShipFactory Instance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new ConcreteShipFactory();
                }
                return instance;
            }
        }

        /// <summary>
        /// Create a ship
        /// </summary>
        /// <param name="ability">The ship type</param>
        /// <param name="ownership">The craft registration; link to owner</param>
        /// <param name="position">Where to build the ship</param>
        /// <param name="rotation">The ship orientation</param>
        /// <returns>The created ship</returns>
        public static ISpaceShip BuildSpaceship(String ship, Ownership ownership, Vector2 position, double rotation)
        {
            return Instance().factories[ship].BuildSpaceship(ownership, position, rotation);
        }

        /// <summary>
        /// Get the factories available.
        /// Consider using GetShipTypes and BuildSpaceship instead.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, IShipFactory> GetFactories()
        {
            return Instance().factories;
        }

        /// <summary>
        /// Get a list of available ship types.
        /// </summary>
        /// <returns></returns>
        public static List<String> GetShipTypes()
        {
            return Instance().factories.Keys.ToList();
        }

        class Factory_Fighter : IShipFactory
        {
            public Factory_Fighter() { }

            public ISpaceShip BuildSpaceship(Ownership ownership, Vector2 position, double rotation)
            {
                return new ConcreteShip_Fighter(ownership, position, rotation);
            }
        }

        class Factory_Eightwing : IShipFactory
        {
            public ISpaceShip BuildSpaceship(Ownership ownership, Vector2 position, double rotation)
            {
                return new ConcreteShip_Eightwing(ownership, position, rotation);
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
}
