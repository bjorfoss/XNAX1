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
        bool activateAbility = false;
        Hud hud;
        

        private bool destroyed = false;
        private double timeDied = 0;

        //Changes for weapons and abilities.
        private bool shipUpgraded = false;
        private bool shipNextAbility = false;
        private bool shipPrevAbility = false;
        private bool shipNextWep = false;
        private bool shipPreviousWep = false;

        private bool generatedAsteroid = false;
        IObstacle asteroid;

        private bool awardPointToOpposition = false;

        //Stats
        private int kills = 0;
        private int timesDied = 0;
        private int timesDiedToAsteroid = 0;

        private int currency = 1500;
        private int generateCurrencytimer;

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

        public bool GetAbilityActivated()
        {
            return activateAbility;
        }

        public void setAbilityActivated()
        {
            activateAbility = false;
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
            //If we're destroyed and we shouldn't be, add the unit to the game.
            if (destroyed && !destroy)
            {
                GameObject.Instance().addToGame(GetShip() as Unit);
            }

            //If we're to be destroyed and we're not, increase death count and add to times died.
            if (destroy && !destroyed)
            {
                timeDied = time;
                //timesDied++;
            }

            destroyed = destroy;
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
            Unit unit = (Unit)ship;
            unit.addCd(new Utilities.CollisionCd(unit));
            unit.setPosition(spawn);
            unit.RestoreHealth(unit.getMaxHealth());
            unit.SetArmorEffectiveness(unit.getMaxHealth());

            GameObject.Instance().addToGame(unit);
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

        public void SetHasGeneratedAsteroid()
        {
            generatedAsteroid = false;
        }

        public void SetAwardOpposition(bool award)
        {
            awardPointToOpposition = award;
            if (award)
            {
                timeDied++;
            }
        }

        public bool GetAwardOpposition()
        {
            return awardPointToOpposition;
        }

        public int GetTimesDied()
        {
            return timesDied;
        }

        public void SetTimesDied(int deaths)
        {
            timesDied = deaths;
        }

        public void IncreaseKills()
        {
            kills++;
        }

        public void IncreaseDeathsByAsteroid()
        {
            timesDiedToAsteroid++;
        }

        public string GetStats()
        {
            string teamN = "Red";
            if (team == 2)
                teamN = "Blue";
            string essentials = name + " of team " + teamN;

            string stats = "";

            for (int i = 0; i < essentials.Length; i++)
                stats += " ";

            stats += " -- Starship: " + shipSelection + " Kills: " + kills.ToString() + " Deaths: " + timesDied.ToString() + " Deaths by Asteroid: " + timesDiedToAsteroid.ToString();

            return essentials + "\n" + stats;
        }

        /// <summary>
        /// Handle input logic and call Spaceship interface methods.
        /// </summary>

        public void HandleInput(KeyboardState newState, GameTime gameTime)
        {
            // generate currency over time
            generateCurrencytimer += gameTime.ElapsedGameTime.Milliseconds;
            if (generateCurrencytimer >= 200)
            {
                generateCurrencytimer = 0;
                currency += 1;
            }
            //allow ship control only if ship still alive
            if ((ownerLink.GetShip() as Unit).getHealth() > 0)
            {
                HandleShipInput(newState, gameTime);
            }
            else
            {
                if (destroyed && ReadyToRespawn(gameTime.TotalGameTime.TotalSeconds))
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
            if (newState.IsKeyDown(Keys.I) && oldState.IsKeyUp(Keys.I) && !ship.GetCurrentAbility().getActive())
            {
                shipNextAbility = true;
                ship.NextAbility();
            }
            else if (newState.IsKeyDown(Keys.J) && oldState.IsKeyUp(Keys.J) && !ship.GetCurrentAbility().getActive())
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
                activateAbility = true;
                ship.UseAbility(gameTime);
            }

            // Fire weapon
            if (newState.IsKeyDown(Keys.F) || newState.IsKeyDown(Keys.Space))
            {
                shipFires = true;
                ship.Fire(gameTime);
            }

            if (ship is SpaceShip)
            {
                if ((ship as SpaceShip).GetIsDocked())
                {
                    HandleShopInput(newState, gameTime, (ship as SpaceShip));
                    
                }

            }

            

        }
        public void HandleShopInput(KeyboardState newState, GameTime gameTime, SpaceShip ship)
        {

            switch (ship.GetShopWindow())
            {
                case "main":

                    if (newState.IsKeyDown(Keys.Z)) // Repair
                    {
                        if (currency >= 1 && ship.Repair(gameTime)) currency -= 1; 
                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // Engine window
                    {
                        ship.SetShopWindow("engine");
                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // Abilities window
                    {
                        ship.SetShopWindow("abilities1");
                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // Weapons window
                    {
                        ship.SetShopWindow("weapons1");
                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // Armor window
                    {
                        ship.SetShopWindow("armor");
                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // 
                    {
                                
                    }
                    break;
                case "abilities1":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // Shield 500
                    {
                        int price = 500;
                        if (currency >= price && ship.BuyAbility("shield")) Purchase(price);
                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // Next abilities page
                    {
                        ship.SetShopWindow("abilities2");
                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
                case "abilities2":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // Next abilities window
                    {
                        ship.SetShopWindow("abilities1");
                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
                case "weapons1":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // Standard Gun 200
                    {
                        int price = 200;
                        if (currency >= price && ship.BuyWeapon("gun")) Purchase(price);
                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // Rapid gun 600
                    {
                        int price = 600;
                        if (currency >= price && ship.BuyWeapon("rapidgun")) Purchase(price);
                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // Laser 1000
                    {
                        int price = 1000;
                        if (currency >= price && ship.BuyWeapon("laser")) Purchase(price);
                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // Next weapons window
                    {
                        ship.SetShopWindow("weapons2");
                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
                case "weapons2":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // Next weapons window
                    {
                        ship.SetShopWindow("weapons1");
                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
                case "armor":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // Buy Armor 100
                    {
                        if (Purchase(100)) ship.BuyArmor();
                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
                case "engine":
                    if (newState.IsKeyDown(Keys.Z) && !oldState.IsKeyDown(Keys.Z)) // Thrust 150
                    {
                        if (Purchase(150)) ship.BuyThrust();
                    }
                    else if (newState.IsKeyDown(Keys.X) && !oldState.IsKeyDown(Keys.X)) // 
                    {
                        
                    }
                    else if (newState.IsKeyDown(Keys.C) && !oldState.IsKeyDown(Keys.C)) // Turn speed 150
                    {
                        if(Purchase(150)) ship.BuyTurnSpeed();
                    }
                    else if (newState.IsKeyDown(Keys.V) && !oldState.IsKeyDown(Keys.V)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.B) && !oldState.IsKeyDown(Keys.B)) // 
                    {

                    }
                    else if (newState.IsKeyDown(Keys.N) && !oldState.IsKeyDown(Keys.N)) // Exit
                    {
                        ship.SetShopWindow("main");
                    }
                    break;
  
            }

        }

        private bool Purchase(int price){

            if(price <= currency){

                currency -= price;
                return true;
            }


            return false;
        }

        void HandleGamepadInput()
        {
        }

        public int GetCurrency()
        {
            return currency;
        }
        public void RecieveAwardCurrency()
        {

            currency += 500;
        }
        public static IPlayer createController()
        {
            return new Human("nobody");
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Check if the ship has received an upgrade.
        /// If yes, it's automatically reset to false after this check.
        /// </summary>
        /// <returns></returns>
        public bool WasShipUpgraded()
        {
            if (shipUpgraded)
            {
                shipUpgraded = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Use to request ship upgrade synchronization over network
        /// </summary>
        public void ShipUpgraded()
        {
            shipUpgraded = true;
        }
    }
}
