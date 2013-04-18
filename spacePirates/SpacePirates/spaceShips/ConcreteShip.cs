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
    }

    class ConcreteShip_Eightwing : SpaceShip, ISpaceShip
    {

        /// <summary>
        /// Instance a basic fighter. Supply the initial position and facing
        /// </summary>
        /// 
        public ConcreteShip_Eightwing(Ownership registration, Vector2 position, double rotation)
            : base(20000, 10000, 50, 30, 40, registration, position, rotation, GraphicBank.getInstance().getGraphic("eightwing"))
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
