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
        private int team = 0;//No team.
        bool pickedTeam = false;
        private string shipSelection = "fighter";
        bool shipFires = false;

        private bool destroyed = false;
        private double timeDied = 0;

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

        public bool GetPickedTeam()
        {
            return pickedTeam;
        }

        public void SetTeam(int choice)
        {
            if (choice == 1)
                team = 1;//Red.
            else
                team = 2;//Blue.

            pickedTeam = true;
        }

        public int GetTeam()
        {
            return team;
        }

        public void SetSelectedShip(string selection)
        {
            if (selection == null || selection == "")
            {
                selection = "fighter";
                Console.WriteLine("Human.SetSelectedShip: Received invalid ship selection value, fall back to default");
            }
            shipSelection = selection;
        }

        public string GetShipSelection()
        {
            return shipSelection;
        }

        public bool GetFiring()
        {
            //only non-destroyed ships can fire
            return (shipFires && ((ownerLink.GetShip() as Unit).getHealth() > 0));
        }

        public void ShipFired()
        {
            shipFires = false;
        }

        public bool GetDestroyed()
        {
            return destroyed;
        }

        public void SetDestroyed(bool destroy, double time)
        {
            destroyed = destroy;
            timeDied = time;
        }

        public bool ReadyToRespawn(double time)
        {
            bool answer = false;

            double elapsed = time - timeDied;

            if (elapsed >= GameObject.Instance().GetRespawnCooldown())
                answer = true;

            return answer;
        }

        public void Respawn()
        {
            destroyed = false;
            ISpaceShip ship = GetShip();

            Vector2 spawn;

            if (team == 1)
                spawn = GameObject.Instance().getRedSpawn();
            else
                spawn = GameObject.Instance().getBlueSpawn();

            (ship as SpaceShip).setPosition(spawn);
            (ship as Unit).RestoreHealth((ship as Unit).getMaxHealth());
        }

        /// <summary>
        /// Handle input logic and call Spaceship interface methods.
        /// </summary>

        public void HandleInput(KeyboardState newState, GameTime gameTime)
        {
            //allow ship control only if ship still alive
            if ((ownerLink.GetShip() as Unit).getHealth() > 0)
            {
                HandleShipInput(newState, gameTime);
            }
            else
            {
                if (ReadyToRespawn(gameTime.TotalGameTime.TotalSeconds))
                    Respawn();
            }

            oldState = newState;
        }

        void HandleShipInput(KeyboardState newState, GameTime gameTime)
        {
            HandleShipKeyboardInput(newState, gameTime);
        }

        void HandleShipKeyboardInput(KeyboardState newState, GameTime gameTime)
        {
            bool forwardKeyDown = false;
            bool shiftPressed = newState.IsKeyDown(Keys.LeftShift);
            ISpaceShip ship = ownerLink.GetShip();

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
                }
                else
                {
                    ship.Thrust(25);
                }
                forwardKeyDown = true;
            }

            //0% thrust
            if (!forwardKeyDown)
            {
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
            }
            else if (newState.IsKeyDown(Keys.D))
            {
                if (shiftPressed)
                {
                    ship.Turn(40.0f);
                }
                else
                {
                    ship.Turn(100.0f);
                }
            }
            else
            {
                ship.Turn(0.0f);
            }

            // Browse features
            if (newState.IsKeyDown(Keys.I))
            {

            }
            else if (newState.IsKeyDown(Keys.J))
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
                shipFires = true;
                ship.Fire(gameTime);
            }

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
