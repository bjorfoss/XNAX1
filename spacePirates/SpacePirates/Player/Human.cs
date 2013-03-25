using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SpacePirates.Player
{
    class Human : Player
    {
        Ownership ownerLink;
        Keyboard keyboard;
        GamePad gamepad;

        /// <summary>
        /// Handle input logic and call Spaceship interface methods.
        /// </summary>
        public void HandleInput();

        void HandleKeyboardInput();
        void HandleGamepadInput();
    }
}
