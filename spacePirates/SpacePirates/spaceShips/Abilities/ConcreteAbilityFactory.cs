using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips.Abilities
{
    class ConcreteAbilityFactory
    {
        public class Factory_Shield : AbilityFactory
        {
            IAbility AbilityFactory.createAbility()
            {
                double duration = 5000;
                double cooldown = 15000;
                double health = 20000;
                return new AbilityState_Shield(cooldown, duration, health);
            }
        }
    }
}
