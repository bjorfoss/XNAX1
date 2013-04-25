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
            : base(20000, 10000, 700, 64, 100, registration, position, rotation, GraphicBank.getInstance().GetGraphic("fighter"))
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
    }

    class ConcreteShip_LolFighter : SpaceShip, ISpaceShip
    {
        public ConcreteShip_LolFighter(Ownership registration, Vector2 position, double rotation)
            : base(20, 100000, 100000, 5000, 15000, registration, position, rotation, GraphicBank.getInstance().GetGraphic("lol"))
        {
            animationFrame = new Rectangle(0, 0, 128, 52);
            unitColor = Color.DimGray;
            maxThrust = 60;
            weapons.Add(Weapons.ConcreteWeaponFactory.CreateWeapon("lolcannon"));
        }

        public override void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, 100);
            thrust = thrust * maxThrust;
            this.currentThrust = thrust;
            //we regard thrust as Force when passed, divide by mass to get acceleration
            double acceleration = thrust / mass;

            //decompose acceleration into vectors:
            this.acceleration.X = (float)(Math.Sin(rotation+MathHelper.PiOver2) * acceleration);
            this.acceleration.Y = (float)(Math.Cos(rotation+MathHelper.PiOver2) * acceleration);
        }
    }

    class ConcreteShip_Eightwing : SpaceShip, ISpaceShip
    {

        /// <summary>
        /// Instance a basic fighter. Supply the initial position and facing
        /// </summary>
        /// 
        public ConcreteShip_Eightwing(Ownership registration, Vector2 position, double rotation)
            : base(20000, 10000, 500, 30, 40, registration, position, rotation, GraphicBank.getInstance().GetGraphic("eightwing"))
        {
            animationFrame = new Rectangle(0, 0, 128, 128);
            unitColor = Color.DimGray;
        }

        /// <summary>
        /// Animates the fighter.
        /// Overridden from the empty method in Unit
        /// </summary>
        /// <param name="gameTime">GameTime used for correct animation</param>
        public override void Update(GameTime gameTime)
        {
            animationTime += gameTime.ElapsedGameTime.Milliseconds;
            if (animationTime >= 200)
            {
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
    }

}
