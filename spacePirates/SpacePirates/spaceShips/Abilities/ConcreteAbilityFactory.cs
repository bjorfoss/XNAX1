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
            Ability AbilityFactory.createAbility()
            {
                return new AbilityState_Shield();
            }
        }
    }
}
