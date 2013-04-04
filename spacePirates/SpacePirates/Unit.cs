using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates
{
    class Unit
    {
        //todo make protected
        Vector2 velocity;
        Vector2 acceleration;
        double mass;
        Vector2 position;
        double rotation;
        double rotationSpeed;

        Texture2D graphics;
        
        double health;
        double maxHealth;

        double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
        double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it
        
        double blastRadius;
        double blastDamage;
        //Rectangle hitbox;

        //add getters and setters

        /// <summary>
        /// 
        /// </summary>
        void CalculateDirectionAndSpeed() {}

        /// <summary>
        /// calculate the next position of the unit
        /// </summary>
        void UpdatePosition() {
            position = position + velocity;
        }

        /// <summary>
        /// Calculate own collision damage, check if colliding with an explosion
        /// Call OnDestroy/OnDeath and do blast damage (if applicable)
        /// </summary>
        /// <param name="unit"></param>
        void HandleCollision(Unit unit) {




            // TODO: calulate ratio based on a fixed number and armor:
            double ratio = 1;



            Vector2 velocityUnit = unit.getVelocity();

            double xforce = ( (velocity.X * mass) - (velocityUnit.X * unit.getMass()) ) / (mass / 2 * unit.getMass() / 2);
            double yforce = ( (velocity.Y * mass) - (velocityUnit.Y * unit.getMass()) ) / (mass / 2 * unit.getMass() / 2);

            acceleration.X += (float) xforce;
            acceleration.Y += (float) yforce;


            double force = Math.Sqrt(xforce * xforce + yforce * yforce) * ratio;

            health = health - force;


            if (health < 0)
            {
                OnDestroy();
            }

        
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDestroy() { }




        public Vector2 getVelocity()
        {
            return velocity;
        }
        public double getMass()
        {
            return mass;
        }

    }
}
