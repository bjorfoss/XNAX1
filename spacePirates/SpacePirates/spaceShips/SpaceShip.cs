using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    class SpaceShip : Unit
    {

        protected double maxTurnSpeed = MathHelper.Pi; //the maximum turn speed the ship itself can generate (degrees per second)
        protected double maxThrust = 100000; //maximum force in Newtons output by the ship's engine(s)
        protected double currentThrust; //The current thrust, in percent, of the maximum thrust

        protected double animationTime; //The time, in milliseconds, since the last time the animation frame was changed

        protected IWeapon currentWeapon; //the currently selected weapon used by fire()
        protected IWeapon[] weapons; //the weapons installed on the ship

        protected IAbility currentAbility; //the ability used by useAbility()
        protected IAbility[] abilities; //the abilities installed on the ship

        protected Ownership registration; //vehicle registration. The player can be retrieved from this

        public SpaceShip(Ownership registration, Vector2 position, double rotation)
        {
            /*
             *  Vector2 velocity;
             *  Vector2 acceleration;
             * double mass;
        
             * Vector2 position;        
             * double rotation;        
             * double rotationSpeed;

             * Texture2D graphics;
        
             * double health;
             * double maxHealth;
        
             * double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
             * double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it
        
             * double blastRadius;
             * double blastDamage;
             */
            this.position = position;
            this.rotation = rotation;
            
            //start at a standstill
            rotationSpeed = 0;
            velocity = new Vector2(0);
            acceleration = new Vector2(0);

            this.registration = registration;
        }

        public virtual void Turn(double turnRate)
        {
            //Check direction of turning
            if (turnRate > 0)
            {
                //Don't exceed turn speed capability
                turnRate = Math.Min(turnRate, 100);
            }
            else if (turnRate < 0)
            {
                //Don't exceed turn speed capability
                turnRate = Math.Max(turnRate, -100);
            }
            turnRate *= maxTurnSpeed;
            rotationSpeed = turnRate;
        }

        public virtual void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, 100);
            thrust = thrust * maxThrust;
            //we regard thrust as Force when passed, divide by mass to get acceleration
            double acceleration = thrust / mass;

            //decompose acceleration into vectors:
            this.acceleration.X = (float) (Math.Sin(rotation) * acceleration);
            this.acceleration.Y = (float)(Math.Cos(rotation) * acceleration);
        }

        public virtual void Fire()
        {
            currentWeapon.Fire();
        }

        public virtual void NextWeapon()
        {
            int index = Array.IndexOf(weapons, currentWeapon);
            //increment weapon or go to start of weapons array
            if (index + 1 < weapons.Length)
            {
                currentWeapon = weapons[index + 1];
            }
            else
            {
                currentWeapon = weapons[0];
            }
        }

        public virtual void PreviousWeapon()
        {
            int index = Array.IndexOf(weapons, currentWeapon);
            //decrement weapon or go to end of weapons array
            if (index != 0)
            {
                currentWeapon = weapons[index - 1];
            }
            else
            {
                currentWeapon = weapons[weapons.Length - 1];
            }
        }

        public virtual void UseAbility()
        {
            currentAbility.Activate();
        }

        public virtual void NextAbility()
        {
            int index = Array.IndexOf(abilities, currentAbility);
            //increment ability or go to start of abilities array
            if (index + 1 < abilities.Length)
            {
                currentAbility = abilities[index + 1];
            }
            else
            {
                currentAbility = abilities[0];
            }
        }

        public virtual void PreviousAbility()
        {
            int index = Array.IndexOf(abilities, currentAbility);
            //decrement ability or go to end of abilities array
            if (index != 0)
            {
                currentAbility = abilities[index - 1];
            }
            else
            {
                currentAbility = abilities[abilities.Length - 1];
            }
        }

        /*public void draw(SpriteBatch batch)
        {

            this.draw(batch, graphics);
        }*/

        public IPlayer GetOwner()
        {
            return registration.GetOwner();
        }
    }
}
