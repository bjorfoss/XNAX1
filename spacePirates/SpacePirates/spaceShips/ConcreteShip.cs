using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.spaceShips
{
    class ConcreteShip_Fighter : Unit, ISpaceShip
    {
        double maxTurnSpeed = 100; //the maximum turn speed the ship itself can generate (degrees per second)

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
        }

        public void Turn(double turnRate)
        {
            throw new NotImplementedException();
        }

        public void Thrust(double thrust)
        {
            throw new NotImplementedException();
        }

        public void Fire()
        {
            throw new NotImplementedException();
        }

        public void NextWeapon()
        {
            throw new NotImplementedException();
        }

        public void PreviousWeapon()
        {
            throw new NotImplementedException();
        }

        public void UseAbility()
        {
            throw new NotImplementedException();
        }

        public void NextAbility()
        {
            throw new NotImplementedException();
        }

        public void PreviousAbility()
        {
            throw new NotImplementedException();
        }
    }
}
