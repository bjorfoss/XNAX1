using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;
using SpacePirates.Utilities;
using SpacePirates.spaceShips.Weapons;
using SpacePirates.spaceShips.Abilities;

namespace SpacePirates.spaceShips
{
    public class SpaceShip : Unit, ISpaceShip
    {
        protected double maxTurnSpeed = MathHelper.Pi; //the maximum turn speed the ship itself can generate (degrees per second)
        protected double maxThrust = 50000; //maximum force in Newtons output by the ship's engine(s)
        protected double currentThrust; //The current thrust, in percent, of the maximum thrust

        protected double animationTime; //The time, in milliseconds, since the last time the animation frame was changed

        protected IWeapon currentWeapon; //the currently selected weapon used by fire()
        protected IWeapon[] weapons; //the weapons installed on the ship

        protected IAbility currentAbility; //the ability used by useAbility()
        protected IAbility[] abilities; //the abilities installed on the ship

        protected Ownership registration; //vehicle registration. The player can be retrieved from this


        /// <summary>
        /// Instance a spaceship.
        /// When inheriting from this class, it is important to set the animationFrame.
        /// To animate, make the method "public override void Update(GameTime gameTime){//Your animation code here}"
        /// </summary>
        /// <param name="mass">The mass of the ship in kilograms</param>
        /// <param name="health">The maximum health of the ship, current starts at maximum</param>
        /// <param name="armor">The armor of the ship, starts at 100% effectiveness</param>
        /// <param name="blastRadius">The blast radius of the shp, the higher number, the bigger explosion at death</param>
        /// <param name="blastDamage">The damage of the explosion</param>
        /// <param name="registration">The connection of ownership between a ship and a player</param>
        /// <param name="position">The starting position of the ship</param>
        /// <param name="rotation">The starting rotation of the ship, in radians</param>
        /// <param name="graphic">The sprite or spritesheet that represents the ship</param>
        public SpaceShip(double mass, double health, double armor, double blastRadius, double blastDamage, Ownership registration, Vector2 position, double rotation, Texture2D graphic)
            : base(position, rotation, Vector2.Zero, Vector2.Zero, mass, 0, health, health, armor, 100, blastRadius, blastDamage, graphic)
        {
            currentThrust = 0;

            this.registration = registration;
            weapons = new IWeapon[2];
            weapons[0] = ConcreteWeaponFactory.CreateWeapon("gun");
            weapons[1] = ConcreteWeaponFactory.CreateWeapon("rapidgun");
            currentWeapon = weapons[0];

            abilities = new IAbility[1];
            abilities[0] = ConcreteAbilityFactory.CreateAbility("shield");
            currentAbility = abilities[0];
        }

        /// <summary>
        /// Turn the spaceship
        /// </summary>
        /// <param name="turnRate">The percentage of maximum turn power to be used</param>
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
            rotationSpeed = turnRate / 100;
        }

        /// <summary>
        /// Gives the ship thrust, accelerating it
        /// </summary>
        /// <param name="thrust">The percentage of maximum thrust power to be used</param>
        public virtual void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, 100);
            thrust = thrust * maxThrust;
            this.currentThrust = thrust;
            //we regard thrust as Force when passed, divide by mass to get acceleration
            double acceleration = thrust / mass;

            //decompose acceleration into vectors:
            this.acceleration.X = (float)(Math.Sin(rotation) * acceleration);
            this.acceleration.Y = (float)(Math.Cos(rotation) * acceleration);
        }

        public virtual void Fire(GameTime gameTime)
        {
            currentWeapon.Fire(gameTime, this);
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

        public IPlayer GetOwner()
        {
            return registration.GetOwner();
        }

        [Obsolete("Use GetPosition in Unit instead")]
        public Vector2 GetShipPosition()
        {
            return position;
        }

        [Obsolete("Use SetPosition in Unit instead")]
        public void SetShipPosition(Vector2 pos)
        {
            position = pos;
        }

        [Obsolete("Use GetRotation in Unit instead")]
        public double GetRotation()
        {
            return rotation;
        }

        [Obsolete("Use SetRotation in Unit instead")]
        public void SetRotation(double rot)
        {
            rotation = rot;
        }

        [Obsolete("Use GetAnimationFrame in Unit instead")]
        public Rectangle GetAnimationFrame()
        {
            return animationFrame;
        }

        [Obsolete("Use SetAnimationFrame in Unit instead")]
        public void SetAnimationFrame(Rectangle anim)
        {
            animationFrame = anim;
        }
    }
}
