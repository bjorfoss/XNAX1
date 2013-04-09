using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates.spaceShips
{
    class ConcreteShip_Fighter : Unit, ISpaceShip
    {
        double maxTurnSpeed = MathHelper.Pi; //the maximum turn speed the ship itself can generate (degrees per second)
        double maxThrust = 100000; //maximum force in Newtons output by the ship's engine(s)
        
        IWeapon currentWeapon; //the currently selected weapon used by fire()
        IWeapon[] weapons; //the weapons installed on the ship
        
        IAbility currentAbility; //the ability used by useAbility()
        IAbility[] abilities; //the abilities installed on the ship




        /// <summary>
        /// Instance a basic fighter. Supply the initial position and facing
        /// </summary>
        public ConcreteShip_Fighter(Vector2 position, double rotation)
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

            mass = 20000; //twenty tons
            maxHealth = 10000;
            health = maxHealth;

            armorEffectiveness = 100;
            armorThreshold = 50;

            blastRadius = 30;
            blastDamage = 40;

            //TODO: graphics;
            graphics = GameObject.GetContentManager().Load<Texture2D>("Graphics/Ships/NFighterSheeth");
        }

        public void Turn(double turnRate)
        {
            //Check direction of turning
            if (turnRate > 0)
            {
                //Don't exceed turn speed capability
                turnRate = Math.Min(turnRate, maxTurnSpeed);
            }
            else
            {
                //Don't exceed turn speed capability
                turnRate = Math.Max(turnRate, maxTurnSpeed * -1);
            }
            rotationSpeed = turnRate;
        }

        public void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, maxThrust);
            //we regard thrust as Force when passed, divide by mass to get acceleration
            double acceleration = thrust / mass;

            //decompose acceleration into vectors:
            this.acceleration.X = (float) (Math.Sin(rotation) * acceleration);
            this.acceleration.Y = (float)(Math.Cos(rotation) * acceleration);
        }

        public void Fire()
        {
            currentWeapon.Fire();
        }

        public void NextWeapon()
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

        public void PreviousWeapon()
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

        public void UseAbility()
        {
            currentAbility.Activate();
        }

        public void NextAbility()
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

        public void PreviousAbility()
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
    }
}
