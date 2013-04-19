﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips.Weapons
{
    class ConcreteWeaponFactory
    {
        private Dictionary<String, WeaponFactory> factories;
        private static ConcreteWeaponFactory instance;
        static readonly object padlock = new Object();
        
        private ConcreteWeaponFactory()
        {
            factories = new Dictionary<string, WeaponFactory>();
            factories.Add("gun", new Factory_Gun());
        }

        /// <summary>
        /// Get an instance of the factory
        /// </summary>
        /// <returns></returns>
        public static ConcreteWeaponFactory Instance() {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new ConcreteWeaponFactory();
                }
                return instance;
            }
        }

        public static IWeapon CreateWeapon(String weapon)
        {
            return Instance().factories[weapon].CreateWeapon();
        }

        public class Factory_Gun : WeaponFactory
        { // Executes third if OS:OSX
            public Factory_Gun() { }

            IWeapon WeaponFactory.CreateWeapon()
            {
                return new WeaponState_Gun();
            }
        }
    }
}
