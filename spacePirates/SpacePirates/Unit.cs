using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SpacePirates.Obstacles;
using SpacePirates.Utilities;
using SpacePirates.Player;
using SpacePirates.spaceShips;

namespace SpacePirates
{
    public class Unit
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
        protected Color unitColor = Color.White;

        protected double health;
        protected double maxHealth;

        protected double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
        protected double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it

        protected double blastRadius;
        protected double blastDamage;

        protected bool docked; //tells if ship(as unit) is docked
        protected double docktime; // tells when the ship docked last

        protected Dictionary<String, String> shopString;
        protected String shopWindow;

        private bool outOfBounds;

        List<CollisionCd> cooldowns;
        //Rectangle hitbox;

        //add getters and setters
             

         public Unit(Vector2 position, double rotation, Vector2 velocity, Vector2 acceleration, double mass, 
             double rotationSpeed, double health, double maxHealth, double armorThreshold, double armorEffectiveness, double blastRadius, double blastDamage, Texture2D graphics)
        {
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.mass = mass;
            Console.WriteLine("Created something at:"  + position.X + "," + position.Y);
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
            docked = false;
           

            cooldowns = new List<CollisionCd>();
        }
        

        /// <summary>
        /// Same as UpdatePosition()
        /// </summary>
        public void CalculateDirectionAndSpeed(GameTime gameTime)
        {
            double max = GameObject.Instance().getMaxSpeed(); 
            Vector2 newVelocity = new Vector2(
                velocity.X + (acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds), 
                velocity.Y + (acceleration.Y * (float)gameTime.ElapsedGameTime.TotalSeconds));

            if (this is IObstacle)
            {
                if (Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2) > Math.Pow(2*max, 2))
                {
                    float multiplier = (float)(2*max / Math.Sqrt(Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2)));
                    newVelocity.X *= multiplier;
                    newVelocity.Y *= multiplier;
                }
            }
            else if (Math.Pow(newVelocity.X, 2) + Math.Pow(newVelocity.Y, 2) > Math.Pow(max, 2))
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
            else if (newRotation >= MathHelper.TwoPi)
            {
                rotation = MathHelper.TwoPi - newRotation;
            }
            else
            {
                rotation = newRotation;
            }
        }

        public void SetAnimationFrame(Rectangle frame)
        {
            this.animationFrame = frame;
        }

        public Rectangle GetAnimationFrame()
        {
            return animationFrame;
        }

        public void SetRotation(double rotation)
        {
            this.rotation = rotation;
        }

        public void SetHealth(double health)
        {
            this.health = health;
        }

        /// <summary>
        /// Return rotation in radians
        /// </summary>
        /// <returns></returns>
        public double GetRotation()
        {
            return this.rotation;
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
        /// A method meant to be overritten, to allow some units to avoid collision
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public virtual bool readyToCollide(GameTime gameTime)
        {
            return true;
        }

        /// <summary>
        /// Check whether two units are ready to collide again
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool ableToCollide(Unit unit)
        {
            foreach (CollisionCd cd in cooldowns)
            {
                if (cd.getUnit() == unit) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Updates the collision cooldowns
        /// </summary>
        /// <param name="gameTime"></param>
        public void updateCooldowns(GameTime gameTime)
        {
            List<CollisionCd> toBeRemoved = new List<CollisionCd>();
            foreach (CollisionCd cd in cooldowns)
            {
                cd.update(gameTime);
                if (cd.cdOver())
                {
                    toBeRemoved.Add(cd);
                }
            }
            foreach (CollisionCd cd in toBeRemoved)
            {
                cooldowns.Remove(cd);
            }
        }

        /// <summary>
        /// Adds a new cooldown with a unit
        /// </summary>
        /// <param name="cd"></param>
        public void addCd(CollisionCd cd)
        {
            cooldowns.Add(cd);
        }

        /// <summary>
        /// The method that will called when this Unit collides with another.
        /// Usually, this method will only call HandleCollision.
        /// </summary>
        public void Collide(Unit unit, GameTime gameTime)
        {
            if (readyToCollide(gameTime) && unit.readyToCollide(gameTime))
            {
                if (ableToCollide(unit) && unit.ableToCollide(this))
                {
                    HandleCollision(unit, gameTime);
                    addCd(new CollisionCd(unit));
                    unit.addCd(new CollisionCd(this));
                }
            }
        }

        /// <summary>
        /// Handle collision with a obstacle
        /// if it will be needed
        /// </summary>
        /// <param name="Obstacle"></param>
        void HandleCollision(Object Obstacle)
        {

        }

        protected virtual double getDamage(double damage)
        {
            return damage;
        }

        /// <summary>
        /// Calculate armor and health damage
        /// </summary>
        /// <param name="damage"></param>
        public void damage(double damage)
        {
            damage = getDamage(damage);
            if (damage <= 0) { return; }
            double currentThreshold = ((armorEffectiveness / 100) * armorThreshold);
            double blockedDamage = damage - currentThreshold;
            Console.WriteLine("Unit.damage: Damage: " + damage + " -- Effectiveness: " + armorEffectiveness + " -- CurrentThreshold: " + currentThreshold);

            //cannot penetrate armor
            if (damage < currentThreshold)
            {
                //just reduce armor effectiveness a bit
                armorEffectiveness -= 1;
                Console.WriteLine("Unit.damage: new effectiveness: " + armorEffectiveness);
            }
            //some damage is blocked still
            else if (currentThreshold != 0)
            {
                health -= damage - currentThreshold;
                //erode armor faster when it's penetrated
                armorEffectiveness -= 10;
                if (armorEffectiveness < 0)
                {
                    armorEffectiveness = 0;
                }
                Console.WriteLine("Unit.damage: new effectiveness: " + armorEffectiveness);
            }
            //no armor left to block damage
            else
            {
                //damage bonus from no armor
                damage *= 1.02;
                health -= damage;
            }
        }

        /// <summary>
        /// Calculate own collision damage, check if colliding with an explosion
        /// Call OnDestroy/OnDeath and do blast damage (if applicable)
        /// </summary>
        /// <param name="unit"></param>
        protected void HandleCollision(Unit unit, GameTime gameTime)
        {

            // TODO: calulate ratio based on a fixed number and armor:
            double ratio = 1;
            double moveEnergy = 0.2; //The percentage of energy involved in movement

            damage(500);
            unit.damage(500);

            if (health <= 0)
            {
                OnDestroy(gameTime, true);
            }
            if (unit.getHealth() <= 0)
            {
                unit.OnDestroy(gameTime, true);
            }

            Vector2 velocityUnit = unit.getVelocity();
            Vector2 positionUnit = unit.GetPosition();
            Rectangle unitRec = unit.getUnitRectangle();
            double unitMass = unit.getMass();

            //From here starts calculations that will move the lightest unit out of the other unit's hitbox
            Vector2 difference = new Vector2(position.X - positionUnit.X, position.Y - positionUnit.Y);
            double distance = Math.Sqrt(Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2));

            //Finds the vector that multiplies difference, but only reaches to the edge of the hitbox
            double rad = Math.Sqrt(Math.Pow(animationFrame.Width / 2, 2) + Math.Pow(animationFrame.Height / 2, 2));
            double var = Math.Sqrt(Math.Pow(rad, 2) / (Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2)));
            Vector2 edge = new Vector2((float)var*difference.X, (float)var*difference.Y);
            edge = -edge;
            edge = downSize(edge, new Vector2(animationFrame.Width / 2, animationFrame.Height / 2));

            //Finds the vector that multiplies difference, but only reaches to the edge of the other ship's hitbox
            rad = Math.Sqrt(Math.Pow(unitRec.Width / 2, 2) + Math.Pow(unitRec.Height / 2, 2));
            var = Math.Sqrt(Math.Pow(rad, 2) / (Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2)));
            Vector2 edge2 = new Vector2((float)var * difference.X, (float)var * difference.Y);
            edge2 = downSize(edge2, new Vector2(unitRec.Width/2, unitRec.Height/2));

            var = Math.Sqrt(1 / (Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2)));
            Vector2 unity = new Vector2((float)(difference.X * var), (float)(difference.Y * var));

            //Moves the lightest ship
            Vector2 move;
            if (mass > unit.getMass())
            {
                move = edge - edge2 - unity + difference;
                unit.setPosition(positionUnit + move);
            }
            else
            {
                move = edge2 + unity - edge - difference;
                setPosition(position + move);
            }

            //End of movement calculations
            /*
            double vel1x = (moveEnergy * velocity.X * (mass - unitMass) + 2 * unitMass * velocityUnit.X) / (mass + unitMass);
            double vel2x = (moveEnergy * velocityUnit.X * (unitMass - mass) + 2 * mass * velocity.X) / (mass + unitMass);
            double vel1y = (moveEnergy * velocity.Y * (mass - unitMass) + 2 * unitMass * velocityUnit.Y) / (mass + unitMass);
            double vel2y = (moveEnergy * velocityUnit.Y * (unitMass - mass) + 2 * mass * velocity.Y) / (mass + unitMass);
            */

            double velx = ((mass * velocity.X + unitMass * velocityUnit.X) / (mass + unitMass));
            double vely = ((mass * velocity.Y + unitMass * velocityUnit.Y) / (mass + unitMass));

            setVelocity(new Vector2((float)velx, (float)vely));
            unit.setVelocity(new Vector2((float)velx, (float)vely));

            Log.getLog().addEvent("Unit at (" + position.X + ", " + position.Y + ") collided with unit at (" + positionUnit.X + ", " + positionUnit.Y + ")");
            bool test = getUnitRectangle().Intersects(unit.getUnitRectangle());
        }


        /// <summary>
        /// Scale a vector down in size. Used for collision detection with
        /// bounding boxes.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Vector2 downSize(Vector2 vector, Vector2 scale)
        {
            Vector2 edge = new Vector2(vector.X, vector.Y);
            double var;
            if (edge.X > scale.X)
            {
                var = scale.X / edge.X;
                edge.X = scale.X;
                edge.Y *= (float)var;
            }
            else if (edge.X < -scale.X)
            {
                var = -scale.X / edge.X;
                edge.X = -scale.X;
                edge.Y *= (float)var;
            }
            if (edge.Y > scale.Y)
            {
                var = scale.Y / edge.Y;
                edge.Y = scale.Y;
                edge.X *= (float)var;
            }
            else if (edge.Y < -scale.Y)
            {
                var = -scale.Y / edge.Y;
                edge.Y = -scale.Y;
                edge.X *= (float)var;
            }
            return edge;
        }

        /// <summary>
        /// Check if the unit is outside the level, and start damaging it
        /// </summary>
        /// <param name="gameTime"></param>
        private void checkIfOutsideLevel(GameTime gameTime)
        {
            if (GameObject.GetLevel().IsOutsideLevel(this))
            {
                double damage = maxHealth * 0.05 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (damage < 1) { damage = 1; }
                health -= damage; 
                outOfBounds = true;

                if (health <= 0)
                {
                    if ((this is SpaceShip) && (this as SpaceShip).GetOwner() != null)
                    {
                        IPlayer player = (this as SpaceShip).GetOwner();

                        if (!(player as Human).GetDestroyed())
                            OnDestroy(gameTime, false);
                    }
                    else
                        OnDestroy(gameTime, false);
                }
            } else {
               outOfBounds = false;
            }
        }
        /// <summary>
        /// TODO: create a blast if there should be one
        /// TODO: position the blast according to blastradius, shipsize and shipposition.
        /// </summary>
        void OnDestroy(GameTime gameTime, bool awardPoint=false) 
        {
            if (blastDamage > 0)
            {
                GameObject.Instance().addToGame(new Explosion(position, new Vector2((float)blastRadius, (float)blastRadius), blastDamage, Color.White));
                //something.CreateBlast(position, blastradius, blastdamage);
            }

            if ((this is SpaceShip) && (this as SpaceShip).GetOwner() != null)
            {
                    IPlayer died = (this as SpaceShip).GetOwner();

                    Human dead = died as Human;

                    velocity = Vector2.Zero;
                    acceleration = Vector2.Zero;

                    GameObject.Instance().removeFromObjects(this);

                    if (!dead.GetDestroyed())
                    {
                        dead.SetDestroyed(true, gameTime.TotalGameTime.TotalSeconds);
                        if (awardPoint)
                        {
                            (died as Human).SetAwardOpposition(true);
                        }
                    }
            }
            else
                GameObject.Instance().removeFromGame(this);
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

        public double getArmorThreshold()
        {
            return armorThreshold;
        }
        public double getArmorEffectiveness()
        {
            return armorEffectiveness;
        }


      

     
      

        /// <summary>
        /// Restore health up to the maximum amount
        /// </summary>
        /// <param name="heal"></param>
        public void RestoreHealth(double heal)
        {
            if (heal + health > maxHealth)
            {
                health = maxHealth;
                return;
            }

            health += heal;

        }

        /// <summary>
        /// Get the rectangle for a unit for collision detection
        /// </summary>
        /// <returns></returns>
        public Rectangle getUnitRectangle()
        {
            return new Rectangle((int)(position.X - (double)animationFrame.Width / 2), 
                (int)(position.Y - (double)animationFrame.Height / 2), animationFrame.Width, animationFrame.Height);
        }
        public Vector2 GetPosition()
        {
            return position;
        }
        public void setPosition(Vector2 pos)
        {
            position = pos;
        }
        public void setVelocity(Vector2 vel)
        {
            velocity = vel;
        }
        public void setColor(Color col)
        {
            unitColor = col;
        }
       


        public virtual void Update(GameTime gameTime)
        {
            if (this is IObstacle)
            {
                //destroy old obstacles
                if ((this as IObstacle).GetLifetimeExpired(gameTime.ElapsedGameTime.Milliseconds))
                {
                    this.SetHealth(0);
                    this.OnDestroy(gameTime, false);
                }
            }
        }

        /// <summary>
        /// Handle physics and other unit specific checks
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateUnit(GameTime gameTime)
        {
            updateCooldowns(gameTime);
            CalculateDirectionAndSpeed(gameTime);
            UpdatePosition(gameTime);
            UpdateFacing(gameTime);
            checkIfOutsideLevel(gameTime);
            if (docked)
            {
                docktime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (docktime >= 20)
                {
                    docked = false;
                    shopWindow = "main";
                }
            }
        }
<<<<<<< HEAD
        
=======

        /// <summary>
        /// Dock at a space station
        /// </summary>
        /// <param name="gameTime"></param>
        public void docking(GameTime gameTime)
        {
            docked = true;
            docktime = 0;
        }

        /// <summary>
        /// Turn a world position into screen space coordinates
        /// </summary>
        /// <param name="position">The position to transform</param>
        /// <returns>A new Vector with the screen coordinates</returns>
>>>>>>> 7ce7a18626b68de95317e4fb7619845c179282d8
        public static Vector2 WorldPosToScreenPos(Vector2 position)
        {
            Rectangle screen = GameObject.GetScreenArea();
            Vector2 cameraPos = (GameObject.GetCameraTarget() as Unit).GetPosition();
            Vector2 screenPos = new Vector2(position.X, position.Y);
            screenPos.X -= cameraPos.X;
            screenPos.X += (float)screen.Width / 2;
            
            screenPos.Y -= cameraPos.Y;
            screenPos.Y += (float)screen.Height / 2;
            screenPos.Y = screen.Height - screenPos.Y;

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
            SpriteFont font = GraphicBank.getInstance().GetFont("Menutext");
            //Draw debug position only for player ship
            if (GameObject.GetCameraTarget() == this)
            {
                ContentManager content = GameObject.GetContentManager();
                String text = "X: " + Math.Round(position.X) + " -- Y: " +
                        Math.Round(position.Y);
                batch.DrawString(font, text, screenPos + new Vector2(0, 200), Color.Wheat);
                batch.DrawString(font, GameObject.Instance().getVictoryText(), screenPos + new Vector2(0, 180), Color.Gold);

                if (outOfBounds)
                {
                    String warning = "Deserters will die, return to the combat area! -- Health: " +
                        Math.Round(this.getHealth());
                    batch.DrawString(font, warning, screenPos + new Vector2(-200, -200), Color.Red);
                }
                if (docked)
                {
                    
                    Rectangle screen = GameObject.GetScreenArea();
                    Vector2 pos = new Vector2(0, (float)(screen.Bottom - (font.MeasureString(shopString[shopWindow]).Y)));

                    
                    batch.DrawString(font, shopString[shopWindow], pos, Color.Green);
                }
            }

            //only draw the unit if it is alive
            if (health > 0)
            {
                batch.Draw(graphics, screenPos, animationFrame, unitColor, (float)rotation,
                        new Vector2(animationFrame.Width / 2, animationFrame.Height / 2),
                        1.0f, SpriteEffects.None, 0f);
            }
        }

    }
}
