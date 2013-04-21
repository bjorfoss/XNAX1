using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SpacePirates.spaceShips
{
    class AbilityState_Shield : IAbility
    {
        double cooldown; //The time, in milliseconds, from the shield expires until it can be used again.
        double duration; //The time, in milliseconds, from the shield is activated until it expires.
        double maxHealth; //The shield's health at activation.
        double health; //The shield's health until it breaks.

        string name; //Name of the ability.

        bool active; //Holds whether the shield is currently active.
        double time; //Holds when the shield last either activated or expired.
        Stopwatch clock; //Handles the passage of time.

        /// <summary>
        /// Creates a new shield
        /// </summary>
        /// <param name="cooldown">The time, in milliseconds, from the shield expires until it can be used again.</param>
        /// <param name="duration">The time, in milliseconds, from the shield is activated until it expires.</param>
        /// <param name="health">The shield's health at activation.</param>
        public AbilityState_Shield(double cooldown, double duration, double health)
        {
            this.cooldown = cooldown;
            this.duration = duration;
            maxHealth = health;
            this.health = 0;

            name = "Shield";

            active = false;
            time = -cooldown; //Set the time to negative cooldown, so the shield can be activated immediately
        }

        public string GetType()
        {

            return "shield";
        }
        /// <summary>
        /// Activates the shield, given that the cooldown has expired.
        /// This gives the shield health, and sets active to true.
        /// </summary>
        public void Activate()
        {
            if (clock == null)
                clock = new Stopwatch();

            double temp = clock.ElapsedMilliseconds;
            if (temp - time >= cooldown)
            {
                health = maxHealth;
                active = true;
            }
            time = temp;
        }

        /// <summary>
        /// Checks whether the shield should expire.
        /// </summary>
        public void Update()
        {
            if (active)
            {
                double temp = clock.ElapsedMilliseconds;
                if (temp - time > duration || health <= 0)
                {
                    active = false;
                    time = temp;
                }
            }
        }

        /// <summary>
        /// Deals damage to the shield
        /// </summary>
        /// <param name="damage"></param>
        public void Damage(double damage)
        {
            health -= damage;
            Update();
        }

        /// <summary>
        /// Checks whether the shield is currently active
        /// </summary>
        /// <returns></returns>
        public bool Active()
        {
            return active;
        }

        public string GetName()
        {
            return name;
        }
    }
}
