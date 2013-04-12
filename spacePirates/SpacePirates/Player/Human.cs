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

            // Accelerate

            if (newState.IsKeyDown(Keys.W))
            {
                ownerLink.GetShip().Thrust(75);
            }
            else
            {
                ownerLink.GetShip().Thrust(0);
            }

            // Deaccelerate

            if (newState.IsKeyDown(Keys.S))
            {

            }

            // Turn

            if (newState.IsKeyDown(Keys.D))
            {

            } else if (newState.IsKeyDown(Keys.A))
            {

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
