using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SpacePirates.spaceShips
{
    class WeaponState_Gun : IWeapon
    {
        double fireRate; //The time, in milliseconds, from a shot is fired until another shot can be fired.
        double time; //The time, in milliseconds, when the previous shot was fired.
        Stopwatch clock; //The stopwatch object that handles the time

        public WeaponState_Gun()
        {
            fireRate = 500;
            clock = new Stopwatch();
            time = 0;
        }


        public void Fire()
        {
            //Records the current time in milliseconds, then checks it against when the last shot was fired.
            double temp = clock.ElapsedMilliseconds;
            if(temp-time >= fireRate)
            {
                //TODO Create new bullet obstacle
                time = temp;
            }
        }
    }
}
