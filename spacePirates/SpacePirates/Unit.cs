using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
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

        private bool outOfBounds;
        //Rectangle hitbox;

        //add getters and setters
             

         public Unit(Vector2 position, double rotation, Vector2 velocity, Vector2 acceleration, double mass, 
             double rotationSpeed, double health, double maxHealth, double armorThreshold, double armorEffectiveness, double blastRadius, double blastDamage, Texture2D graphics)
        {
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.mass = mass;
            this.position = position;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            this.health = health;
            this.maxHealth = maxHealth;
            this.armorEffectiveness = armorEffectiveness;
            this.armorThreshold = armorThreshold;
            this.blastDamage = blastDamage;
            this.blastRadius = blastRadius;

            this.graphics = graphics;
        }
        

        /// <summary>
        /// Same as UpdatePosition()
        /// </summary>
        public void CalculateDirectionAndSpeed(GameTime gameTime)
        {
            double max = GameObject.Instance().getMaxSpeed();
            Vector2 newVelocity = new Vector2(
                velocity.X + (acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds), 
                velocity.Y + acceleration.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
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

        /// <summary>
        /// Update the unit facing (rotation), scaled by time since last update.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateFacing(GameTime gameTime)
        {
            //handle rotation
            double newRotation = rotation;
            newRotation += (rotationSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (newRotation < 0)
            {
                rotation = MathHelper.TwoPi + newRotation;
            }
            else if (newRotation > MathHelper.TwoPi)
            {
                rotation = MathHelper.TwoPi - newRotation;
            }
            else
            {
                rotation = newRotation;
            }
        }

        /// <summary>
        /// Update movement from velocity, scale by time since last update.
        /// </summary>
        /// <param name="gameTime"></param>
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

        private void checkIfOutsideLevel(GameTime gameTime)
        {
            if (GameObject.GetLevel().IsOutsideLevel(this))
            {
               health -= maxHealth * 0.05 * (float)gameTime.ElapsedGameTime.TotalSeconds;
               outOfBounds = true;
            } else {
               outOfBounds = false;
            }

            if (health <= 0)
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

        public void UpdateUnit(GameTime gameTime)
        {
            CalculateDirectionAndSpeed(gameTime);
            UpdatePosition(gameTime);
            UpdateFacing(gameTime);
            checkIfOutsideLevel(gameTime);
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
        /// Draws the unit. This method can be replaced by an implementation specific to
        /// the runtime instance.
        /// </summary>
        /// <param name="batch"></param>
        public virtual void Draw(SpriteBatch batch)
        {

            Vector2 screenPos = WorldPosToScreenPos(position);
            //Draw debug position only for player ship
            if (GameObject.GetCameraTarget() == this)
            {
                ContentManager content = GameObject.GetContentManager();
                String text = "X: " + Math.Round(position.X) + " -- Y: " +
                        Math.Round(position.Y);
                batch.DrawString(content.Load<SpriteFont>("Graphics/SpriteFonts/Menutext"),
                        text, screenPos + new Vector2(0, 200), Color.Wheat);

                if (outOfBounds)
                {
                    String warning = "Deserters will die, return to the combat area! -- Health: " +
                        Math.Round(this.getHealth());
                    batch.DrawString(content.Load<SpriteFont>("Graphics/SpriteFonts/Menutext"),
                        warning, screenPos + new Vector2(-150, -200), Color.Red);
                }
            }

            batch.Draw(graphics, screenPos, animationFrame, Color.White, (float)rotation,
                    new Vector2(animationFrame.Width / 2, animationFrame.Height / 2),
                    1.0f, SpriteEffects.None, 0f);
            
          
            
        }

    }
}
