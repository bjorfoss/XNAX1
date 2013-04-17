using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;
using SpacePirates.Utilities;

namespace SpacePirates.spaceShips
{
    class ConcreteShip_Fighter : SpaceShip, ISpaceShip
    {

        /// <summary>
        /// Instance a basic fighter. Supply the initial position and facing
        /// </summary>
        /// 
        public ConcreteShip_Fighter(Ownership registration, Vector2 position, double rotation) 
            : base(20000, 10000, 50, 30, 40, registration, position, rotation, GraphicBank.getInstance().getGraphic("fighter"))
        {
            animationFrame = new Rectangle(0, 0, 128, 128);
        }

        /// <summary>
        /// Animates the fighter.
        /// Overridden from the empty method in Unit
        /// </summary>
        /// <param name="gameTime">GameTime used for correct animation</param>
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

        public Vector2 GetShipPosition()
        {
            return position;
        }

        public void SetShipPosition(Vector2 pos)
        {
            position = pos;
        }
    }
}
