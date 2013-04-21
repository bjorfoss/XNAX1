using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips.Abilities
{
    public class ConcreteAbilityFactory
    {
        private Dictionary<String, AbilityFactory> factories;
        private static ConcreteAbilityFactory instance;
        static readonly object padlock = new Object();

        private ConcreteAbilityFactory()
        {
            factories = new Dictionary<string, AbilityFactory>();
            factories.Add("shield", new Factory_Shield());
        }

        /// <summary>
        /// Get an instance of the factory
        /// </summary>
        /// <returns></returns>
        public static ConcreteAbilityFactory Instance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new ConcreteAbilityFactory();
                }
                return instance;
            }
        }

        /// <summary>
        /// Create an ability
        /// </summary>
        /// <param name="ability">The ability type</param>
        /// <returns>The created ability</returns>
        public static IAbility CreateAbility(String ability)
        {
            return Instance().factories[ability].CreateAbility();
        }

        class Factory_Shield : AbilityFactory
        {
            public Factory_Shield() { }

            IAbility AbilityFactory.CreateAbility()
            {
                double duration = 5000;
                double cooldown = 15000;
                double health = 10000;
                return new AbilityState_Shield(cooldown, duration, health);
            }
        }
    }
}
