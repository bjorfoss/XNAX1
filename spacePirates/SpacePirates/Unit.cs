using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Obstacles;

namespace SpacePirates
{
    class Unit
    {
        //todo make protected
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected double mass;
        protected Vector2 position;
        protected double rotation;
        protected double rotationSpeed;

        protected Texture2D graphics;

        protected double health;
        protected double maxHealth;

        protected double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
        protected double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it

        protected double blastRadius;
        protected double blastDamage;
        //Rectangle hitbox;

        //add getters and setters

        /// <summary>
        /// Same as UpdatePosition()
        /// </summary>
        void CalculateDirectionAndSpeed()
        {
            double max = GameObject.Instance().getMaxSpeed();
            Vector2 newVelocity = new Vector2(velocity.X + acceleration.X, velocity.Y + acceleration.Y);
            if (Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2) > Math.Pow(max, 2))
            {
                float multiplier = (float)(max / Math.Sqrt(Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2)));
                newVelocity.X *= multiplier;
                newVelocity.Y *= multiplier;
            }
        }

        /// <summary>
        /// calculate the next position of the unit
        /// </summary>
        public Vector2 UpdatePosition(Vector2 relativePosition) {
            position = position + velocity - relativePosition;
            return position;
        }
        /// <summary>
        /// Handle collision with a obstacle
        /// if it will be needed
        /// </summary>
        /// <param name="Obstacle"></param>
        void HandleCollision(Object Obstacle)
        {

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
        /// TODO: create a blast if there should be one
        /// TODO: position the blast according to blastradius, shipsize and shipposition.
        /// </summary>
        void OnDestroy() 
        {

            if (blastDamage > 0)
            {
                //something.CreateBlast(position, blastradius, blastdamage);
            }
        
        }




        public Vector2 getVelocity()
        {
            return velocity;
        }
        public double getMass()
        {
            return mass;
        }
        public double getMaxHealth()
        {
            return maxHealth;
        }
        public double getHealth()
        {
            return health;
        }


        /// <summary>
        /// Overwrite this in underclasses or make it here?
        /// </summary>
        /// <param name="bach"></param>
        public void draw(SpriteBatch bach)
        {

        }

    }
}
