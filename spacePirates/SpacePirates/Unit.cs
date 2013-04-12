﻿using System;
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
        public double rotation;
        protected double rotationSpeed;

        protected Texture2D graphics;
        protected Rectangle animationFrame;

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
        public void CalculateDirectionAndSpeed()
        {
            double max = GameObject.Instance().getMaxSpeed();
            Vector2 newVelocity = new Vector2(velocity.X + acceleration.X, velocity.Y + acceleration.Y);
            if (Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2) > Math.Pow(max, 2))
            {
                float multiplier = (float)(max / Math.Sqrt(Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2)));
                newVelocity.X *= multiplier;
                newVelocity.Y *= multiplier;
            }
            velocity = newVelocity;

            
        }

        /// <summary>
        /// calculate the next position of the unit
        /// </summary>
        public Vector2 UpdatePosition(Vector2 relativePosition) {
            position = position + velocity - relativePosition;
            return position;
        }

        public void UpdateFacing(GameTime gameTime)
        {
            //handle rotation
            double newRotation = rotation; //+ (rotationSpeed * gametime.ElapsedGameTime.TotalMilliseconds);
            newRotation = rotation + (rotationSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (newRotation < 0)
            {
                rotation = 360 + newRotation;
            }
            else if (newRotation > 360)
            {
                rotation = 360 - newRotation;
            }
            else
            {
                rotation = newRotation;
            }
        }

        public void UpdatePosition(GameTime gameTime)
        {
            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

        public virtual void Update(GameTime gameTime)
        {
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public static Vector2 WorldPosToScreenPos(Vector2 position)
        {
            Rectangle screen = GameObject.GetScreenArea();
            Vector2 cameraPos = (GameObject.GetCameraTarget() as Unit).GetPosition();
            Vector2 screenPos = new Vector2(position.X, position.Y);
            screenPos = screenPos - cameraPos;
            screenPos.X += (float)screen.Width / 2;
            //screenPos.X += 400;
            screenPos.Y += (float)screen.Height / 2;
            //screenPos.Y += 230;
            return screenPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        public virtual void Draw(SpriteBatch batch)
        {

            Vector2 screenPos = WorldPosToScreenPos(position);
            batch.DrawString(GameObject.GetContentManager().Load<SpriteFont>("Graphics/SpriteFonts/Menutext"), position.ToString(), screenPos + new Vector2(0, 200), Color.Wheat);
            batch.Draw(graphics, screenPos, animationFrame, Color.White, (float)rotation,
                    new Vector2(animationFrame.Width / 2, animationFrame.Height / 2),
                    1.0f, SpriteEffects.None, 0f);
          //  bach.Draw(texture, position, Color.White);
            
          
            
        }
        public void draw(SpriteBatch bach, Texture2D texture, Rectangle rectangle)
        {
            rectangle.Width = 128;
            rectangle.Height = 128;

            bach.Draw(texture, position, new Rectangle(0,0,128,128), Color.White, (float)rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.1f);

        }

    }
}
