using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Utilities;

namespace SpacePirates.spaceShips
{
    class AbilityState_Shield : IAbility
    {
        double cooldown; //The time, in milliseconds, from the shield expires until it can be used again.
        double duration; //The time, in milliseconds, from the shield is activated until it expires.
        double maxHealth; //The shield's health at activation.
        double health; //The shield's health until it breaks.

        string name; //Name of the ability.
        Texture2D graphic;

        bool active; //Holds whether the shield is currently active.
        double time; //Holds when the shield last either activated or expired.

        /// <summary>
        /// Creates a new shield
        /// </summary>
        /// <param name="cooldown">The time, in milliseconds, from the shield expires until it can be used again.</param>
        /// <param name="duration">The time, in milliseconds, from the shield is activated until it expires.</param>
        /// <param name="health">The shield's health at activation.</param>
        public AbilityState_Shield(double cooldown, double duration, double health)
        {
            this.cooldown = cooldown;
            this.duration = duration;
            maxHealth = health;
            this.health = 0;

            name = "Shield";
            graphic = GraphicBank.getInstance().GetGraphic("barrier");

            active = false;
            time = 0; //Set the time to negative cooldown, so the shield can be activated immediately
        }

        public string GetType()
        {

            return "shield";
        }
        /// <summary>
        /// Activates the shield, given that the cooldown has expired.
        /// This gives the shield health, and sets active to true.
        /// </summary>
        public void Activate(GameTime gameTime)
        {
            if (time <= 0)
            {
                health = maxHealth;
                active = true;
                time = duration;
            }
        }

        /// <summary>
        /// Checks whether the shield should expire.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (time > 0)
            {
                time -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                if (time < 0)
                {
                    time = 0;
                }
                if (active)
                {
                    active = false;
                    time = cooldown;
                }
            }
        }

        /// <summary>
        /// Deals damage to the shield
        /// </summary>
        /// <param name="damage"></param>
        public double Damage(double damage)
        {
            if (active)
            {
                double remainder = damage - health;
                if (remainder < 0)
                {
                    remainder = 0;
                    health -= damage;
                }
                else
                {
                    health = 0;
                }

                if (health <= 0 && active)
                {
                    active = false;
                    time = cooldown;
                }

                return remainder;
            }
            else
            {
                return damage;
            }
        }


        public void Draw(SpriteBatch batch, SpaceShip ship)
        {
            if (active)
            {
                Rectangle rectangle = ship.getUnitRectangle();
                Vector2 location = Unit.WorldPosToScreenPos(ship.GetPosition());
                Rectangle animationFrame = new Rectangle(0, 0, graphic.Width, graphic.Height);
                batch.Draw(graphic, location, animationFrame, Color.White, 0f, 
                    new Vector2(animationFrame.Width/2, animationFrame.Height/2), 
                    (float)rectangle.Width/animationFrame.Width, SpriteEffects.None, 1f);
            }
        }

        /// <summary>
        /// Checks whether the shield is currently active
        /// </summary>
        /// <returns></returns>
        public bool getActive()
        {
            return active;
        }

        public double getTimer()
        {
            return time;
        }

        public string GetName()
        {
            return name;
        }
    }
}
