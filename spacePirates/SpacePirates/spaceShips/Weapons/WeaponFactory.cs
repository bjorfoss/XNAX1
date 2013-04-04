using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips.Weapons
{
    interface WeaponFactory
    {
        IWeapon CreateWeapon();
    }
}
