using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpacePirates.spaceShips;

namespace SpacePirates.Player
{
    class Human : IPlayer
    {
        Ownership ownerLink;
        KeyboardState oldState = Keyboard.GetState();

        String name;
        public Human(string name)
        {
            this.name = name;

        }

        public String GetName()
        {
            return name;
        }

        public SpaceShip GetShip()
        {
            return ownerLink.GetShip();
        }

        public void SetOwnerShip(Ownership ownerLink)
        {
            this.ownerLink = ownerLink;
        }

        /// <summary>
        /// Handle input logic and call Spaceship interface methods.
        /// </summary>
        public void HandleInput(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();
            bool forwardKeyDown = false;
            bool shiftPressed = newState.IsKeyDown(Keys.LeftShift);
            SpaceShip ship = ownerLink.GetShip();
            
            //70 to 100% thrust
            if (newState.IsKeyDown(Keys.W))
            {
                if (shiftPressed)
                {
                    ship.Thrust(100);
                }
                else
                {
                    ship.Thrust(75);
                }
                forwardKeyDown = true;
            }
            
            //25 to 50% thrust
            if (newState.IsKeyDown(Keys.S))
            {
                if (shiftPressed)
                {
                    ship.Thrust(50);
                } else {
                    ship.Thrust(25);
                }
                forwardKeyDown = true;
            }

            //0% thrust
            if (!forwardKeyDown) {
                ship.Thrust(0);
            }

            //Turn left, right or not at all
            if (newState.IsKeyDown(Keys.A))
            {
                if (shiftPressed)
                {
                    ship.Turn(-40.0f);
                }
                else
                {
                    ship.Turn(-100.0f);
                }
            } else if (newState.IsKeyDown(Keys.D))
            {
                if (shiftPressed)
                {
                    ship.Turn(40.0f);
                }
                else
                {
                    ship.Turn(100.0f);
                }
            } else {
                ship.Turn(0.0f);
            }

            // Browse features
            if (newState.IsKeyDown(Keys.I))
            {

            } else if (newState.IsKeyDown(Keys.J))
            {

            }

            // Browse weapons
            if (newState.IsKeyDown(Keys.O))
            {

            }
            else if (newState.IsKeyDown(Keys.K))
            {

            }

            // Execute feature
            if (newState.IsKeyDown(Keys.E))
            {

            }

            // Fire weapon
            if (newState.IsKeyDown(Keys.F))
            {
                ship.Fire(gameTime);
            }
            oldState = newState;
        }

        void HandleKeyboardInput()
        {


        }

        void HandleGamepadInput()
        {
        }



        public static IPlayer createController()
        {
            return new Human("nobody");
            //throw new NotImplementedException();
        }
    }
}
