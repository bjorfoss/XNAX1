using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;

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
        Hud hud;
        

        private bool destroyed = false;
        private double timeDied = 0;

        //Changes for weapons and abilities.
        private bool shipNextAbility = false;
        private bool shipPrevAbility = false;
        private bool shipNextWep = false;
        private bool shipPreviousWep = false;

        private bool generatedAsteroid = false;
        IObstacle asteroid;

        private bool awardPointToOpposition = false;

        public Human(string name)
        {
            this.hud = new Hud(this);
            this.name = name;

        }

        public Hud getHud() {
            return hud;
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

        //Getter for network. Just set the bool to false at the end anyway.
        public bool GetNextWeaponChange()
        {
            bool ret = shipNextWep;

            shipNextWep = false;

            return ret;
        }

        //Getter for network. Just set the bool to false at the end anyway.
        public bool GetPrevWeaponChange()
        {
            bool ret = shipPreviousWep;

            shipPreviousWep = false;

            return ret;
        }

        //Getter for network. Just set the bool to false at the end anyway.
        public bool GetNextAbilityChange()
        {
            bool ret = shipNextAbility;

            shipNextAbility = false;

            return ret;
        }

        //Getter for network. Just set the bool to false at the end anyway.
        public bool GetPrevAbilityChange()
        {
            bool ret = shipPrevAbility;

            shipPrevAbility = false;

            return ret;
        }

        public bool GetDestroyed()
        {
            return destroyed;
        }

        public void SetDestroyed(bool destroy, double time)
        {
            if (destroyed && !destroy)
            {
                GameObject.Instance().addToGame(GetShip() as Unit);
            }
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
            GameObject.Instance().addToGame(ship as Unit);
        }

        public void HostAsteroidGeneration(IObstacle astro)
        {
            generatedAsteroid = true;
            asteroid = astro;
        }

        public bool HasHostGeneratedAsteroid()
        {
            return generatedAsteroid;
        }

        public IObstacle GetGeneratedAsteroid()
        {
            return asteroid;         
        }

        public void SetAwardOpposition(bool award)
        {
            awardPointToOpposition = award;
        }

        public bool GetAwardOppposition()
        {
            return awardPointToOpposition;
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
            if (newState.IsKeyDown(Keys.I) && oldState.IsKeyUp(Keys.I))
            {
                shipNextAbility = true;
                ship.NextAbility();
            }
            else if (newState.IsKeyDown(Keys.J) && oldState.IsKeyUp(Keys.J))
            {
                shipPrevAbility = true;
                ship.PreviousAbility();
            }

            // Browse weapons
            if (newState.IsKeyDown(Keys.O) && oldState.IsKeyUp(Keys.O))
            {
                shipNextWep = true;
                ship.NextWeapon();
            }
            else if (newState.IsKeyDown(Keys.K) && oldState.IsKeyUp(Keys.K))
            {
                shipPreviousWep = true;
                ship.PreviousWeapon();
            }

            // Execute feature
            if (newState.IsKeyDown(Keys.E))
            {
                ship.UseAbility();
            }

            // Fire weapon
            if (newState.IsKeyDown(Keys.F) || newState.IsKeyDown(Keys.Space))
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
