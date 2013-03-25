using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips
{
    class ConcreteShip_Fighter : Unit, SpaceShip
    {
        public void Turn(double turnRate) { }

        public void Thrust(double thrust) { }

        public void Fire() { }

        public void NextWeapon() { }

        public void PreviousWeapon() { }

        public void UseAbility() { }

        public void NextAbility() { }

        public void PreviousAbility() { }
        
    }
}
