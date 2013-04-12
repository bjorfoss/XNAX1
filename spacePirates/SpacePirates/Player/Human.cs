using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ISpaceShip GetShip()
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
        public void HandleInput()
        {
            KeyboardState newState = Keyboard.GetState();
            bool forwardKeyDown = false;
            
            //70 to 100% thrust
            if (newState.IsKeyDown(Keys.W))
            {
                if (newState.IsKeyDown(Keys.LeftShift))
                {
                    ownerLink.GetShip().Thrust(100);
                }
                else
                {
                    ownerLink.GetShip().Thrust(75);
                }
                forwardKeyDown = true;
            }
            
            //25 to 50% thrust
            if (newState.IsKeyDown(Keys.S))
            {
                if (newState.IsKeyDown(Keys.LeftShift)) {
                    ownerLink.GetShip().Thrust(50);
                } else {
                    ownerLink.GetShip().Thrust(25);
                }
                forwardKeyDown = true;
            }

            //0% thrust
            if (!forwardKeyDown) {
                ownerLink.GetShip().Thrust(0);
            }

            //Turn left, right or not at all
            if (newState.IsKeyDown(Keys.A))
            {
                ownerLink.GetShip().Turn(-100.0f);
            } else if (newState.IsKeyDown(Keys.D))
            {
                ownerLink.GetShip().Turn(100.0f);
            } else {
                ownerLink.GetShip().Turn(0.0f);
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
