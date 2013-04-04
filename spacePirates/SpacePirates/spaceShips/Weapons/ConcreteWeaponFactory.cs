using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips.Weapons
{
    class ConcreteWeaponFactory
    {
        public class Factory_Gun : WeaponFactory
        { // Executes third if OS:OSX

            IWeapon WeaponFactory.CreateWeapon()
            {
                return new WeaponState_Gun();
            }
        }
    }
}
