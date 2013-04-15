using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;

namespace SpacePirates.spaceShips
{
    class ConcreteShip_Fighter : Unit, ISpaceShip
    {
        double maxTurnSpeed = MathHelper.Pi; //the maximum turn speed the ship itself can generate (degrees per second)
        double maxThrust = 50000; //maximum force in Newtons output by the ship's engine(s)
        double currentThrust; //The current thrust, in percent, of the maximum thrust

        double animationTime; //The time, in milliseconds, since the last time the animation frame was changed
        
        IWeapon currentWeapon; //the currently selected weapon used by fire()
        IWeapon[] weapons; //the weapons installed on the ship
        
        IAbility currentAbility; //the ability used by useAbility()
        IAbility[] abilities; //the abilities installed on the ship

        Ownership registration; //vehicle registration. The player can be retrieved from this

       
        /// <summary>
        /// Instance a basic fighter. Supply the initial position and facing
        /// </summary>
        /// 
     

        public ConcreteShip_Fighter(Ownership registration, Vector2 position, double rotation) : base(position, rotation, velocity, new Vector2(0), 20000, 0, 10000, 10000, 50, 100, 30, 40);
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
           
            
            //start at a standstill
           
            currentThrust = 0;

            this.registration = registration;
            animationFrame = new Rectangle(0, 0, 128, 128);

            graphics = GameObject.GetContentManager().Load<Texture2D>("Graphics/Ships/NFighterSheeth");
        }

        public override void Update(GameTime gameTime)
        {
            animationTime += gameTime.ElapsedGameTime.Milliseconds;
            if(animationTime >= 200){
                if (currentThrust > 0)
                {
                    //This is where the animation cycles between the two last frames at the height of the thrust
                    if (animationFrame.X / 128 >= 2 && animationFrame.Y / 128 >= 1)
                    {
                        animationFrame.X -= 128;
                    }
                    //This is where the animation switches to a lower line of frames
                    else if (animationFrame.X / 128 >= 3)
                    {
                        animationFrame.X = 0;
                        animationFrame.Y += 128;
                    }
                    //Normal animation
                    else
                    {
                        animationFrame.X += 128;
                    }
                    animationTime = 0;
                }
                else
                {
                    animationFrame.X = 0;
                    animationFrame.Y = 0;
                }
                animationTime = 0;
            }
        }

        public void Turn(double turnRate)
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
            rotationSpeed = turnRate / 100;
        }

        public void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, 100);
            thrust = thrust * maxThrust;
            this.currentThrust = thrust;
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
