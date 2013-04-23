using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;
using SpacePirates.Utilities;
using SpacePirates.spaceShips.Weapons;
using SpacePirates.spaceShips.Abilities;

namespace SpacePirates.spaceShips
{
    public class SpaceShip : Unit, ISpaceShip
    {
        protected double maxTurnSpeed = MathHelper.Pi; //the maximum turn speed the ship itself can generate (degrees per second)
        protected double maxThrust = 30000; //maximum force in Newtons output by the ship's engine(s)
        protected double currentThrust; //The current thrust, in percent, of the maximum thrust

        protected double animationTime; //The time, in milliseconds, since the last time the animation frame was changed

        protected IWeapon currentWeapon; //the currently selected weapon used by fire()
        protected List<IWeapon> weapons; //the weapons installed on the ship

        protected IAbility currentAbility; //the ability used by useAbility()
        protected List<IAbility> abilities; //the abilities installed on the ship

        protected Ownership registration; //vehicle registration. The player can be retrieved from this


        /// <summary>
        /// Instance a spaceship.
        /// When inheriting from this class, it is important to set the animationFrame.
        /// To animate, make the method "public override void Update(GameTime gameTime){//Your animation code here}"
        /// </summary>
        /// <param name="mass">The mass of the ship in kilograms</param>
        /// <param name="health">The maximum health of the ship, current starts at maximum</param>
        /// <param name="armor">The armor of the ship, starts at 100% effectiveness</param>
        /// <param name="blastRadius">The blast radius of the shp, the higher number, the bigger explosion at death</param>
        /// <param name="blastDamage">The damage of the explosion</param>
        /// <param name="registration">The connection of ownership between a ship and a player</param>
        /// <param name="position">The starting position of the ship</param>
        /// <param name="rotation">The starting rotation of the ship, in radians</param>
        /// <param name="graphic">The sprite or spritesheet that represents the ship</param>
        public SpaceShip(double mass, double health, double armor, double blastRadius, double blastDamage, Ownership registration, Vector2 position, double rotation, Texture2D graphic)
            : base(position, rotation, Vector2.Zero, Vector2.Zero, mass, 0, health, health, armor, 100, blastRadius, blastDamage, graphic)
        {
            currentThrust = 0;

            this.registration = registration;
            weapons = new List<IWeapon>();
            weapons.Add(ConcreteWeaponFactory.CreateWeapon("gun"));
            
            currentWeapon = weapons[0];

            abilities = new List<IAbility>();
            abilities.Add(ConcreteAbilityFactory.CreateAbility("shield"));
            currentAbility = abilities[0];


            
            shopString = new Dictionary<String, String>();
            shopWindow = "main";
            CreateShop();
        }

        private void CreateShop()
        {
            shopString.Add("main", "Spaceshop\nZ - Repair\nX - Engine\nC - Abilities\nV - Weapons\nB - Armor\n");
            shopString.Add("abilities1", "Abilities 1\nZ - Shield " + (BoughtAbility("shield") ? " - Bought" : "$500") + "\nX - \nC - \nV - \nB - Next Page\nN - Exit");
            shopString.Add("abilities2", "Abilities 2\nZ - \nX - \nC - \nV - \nB - Next Page\nN - Exit");
            shopString.Add("weapons1", "Weapons 1\nZ - Standard Gun " + (BoughtWeapon("gun") ? "- Bought" : "$200") + "\nX - Rapidgun " + (BoughtWeapon("rapidgun") ? "- Bought" : "$600") + "\nC - Laser " + (BoughtWeapon("laser") ? "- Bought" : "$1000") + "\nV - \nB - Next Page\nN - Exit");
            shopString.Add("weapons2", "Weapons 2\nZ - \nX - \nC - \nV - \nB - Next Page\nN - Exit");
            shopString.Add("armor", "Armor\nZ - Buy Armor " + "$100 " + (int)armorThreshold + "\nX - \nC - \nV - \nB - \nN - Exit");
            shopString.Add("engine", "Engine\nZ - Thrust $150 " + (int)maxThrust + "\nX - \nC - Turn Speed $150 " + Math.Round(maxTurnSpeed, 2) + "\nV - \nB - \nN - Exit");
            
        }

        /// <summary>
        /// Turn the spaceship
        /// </summary>
        /// <param name="turnRate">The percentage of maximum turn power to be used</param>
        public virtual void Turn(double turnRate)
        {
            //Check direction of turning
            if (turnRate > 0)
            {
                //Don't exceed turn speed capability
                turnRate = Math.Min(turnRate, 100);
            }
            else if (turnRate < 0)
            {
                //Don't exceed turn speed capability
                turnRate = Math.Max(turnRate, -100);
            }
            turnRate *= maxTurnSpeed;
            rotationSpeed = turnRate / 100;
        }

        /// <summary>
        /// Gives the ship thrust, accelerating it
        /// </summary>
        /// <param name="thrust">The percentage of maximum thrust power to be used</param>
        public virtual void Thrust(double thrust)
        {
            //ensure the ship doesn't thrust more than its capabilities
            thrust = Math.Min(thrust, 100);
            thrust = thrust * maxThrust;
            this.currentThrust = thrust;
            //we regard thrust as Force when passed, divide by mass to get acceleration
            double acceleration = thrust / mass;

            //decompose acceleration into vectors:
            this.acceleration.X = (float)(Math.Sin(rotation) * acceleration);
            this.acceleration.Y = (float)(Math.Cos(rotation) * acceleration);
        }

        public virtual void Fire(GameTime gameTime)
        {
            currentWeapon.Fire(gameTime, this);
        }

        public virtual void NextWeapon()
        {
            int index = weapons.IndexOf(currentWeapon);
            //increment weapon or go to start of weapons list
            index = index + 1;
            while (index <= weapons.Count)
            {
                if (index >= weapons.Count)
                {
                    index = 0;
                    break;
                }
                if (weapons[index] != null)
                {
                    break;
                }
                index++;
                    
            }
            currentWeapon = weapons[index];
        }

        public virtual void PreviousWeapon()
        {
            int index = weapons.IndexOf(currentWeapon);
            //decrement weapon or go to end of weapons array
            if (index > 0)
            {
                currentWeapon = weapons[index - 1];
            }
            else
            {
                index = weapons.Count - 1;
                //don't select null weapons
                while (weapons[index] == null)
                {
                    index--;
                }
                currentWeapon = weapons[index];
            }
        }

        public virtual void UseAbility(GameTime gameTime)
        {
            if (currentAbility != null)
            {
                currentAbility.Activate(gameTime);
            }
        }

        protected override double getDamage(double damage)
        {
            if (currentAbility is AbilityState_Shield)
            {
                return (currentAbility as AbilityState_Shield).Damage(damage);
            }
            else
            {
                return damage;
            }
        }

        public virtual void updateAbilities(GameTime gameTime)
        {
            if (abilities.Count > 0)
            {
                foreach (IAbility ability in abilities)
                {
                    ability.Update(gameTime);
                }
            }
        }

        public virtual void drawAbilities(SpriteBatch batch)
        {
            if (currentAbility != null)
            {
                currentAbility.Draw(batch, this);
            }
        }

        public virtual void NextAbility()
        {
            int index = abilities.IndexOf(currentAbility);
            //increment ability or go to start of abilities array
            if (index + 1 < abilities.Count)
            {
                currentAbility = abilities[index + 1];
            }
            else
            {
                currentAbility = abilities[0];
            }
        }

        public virtual void PreviousAbility()
        {
            int index = abilities.IndexOf(currentAbility);
            //decrement ability or go to end of abilities array
            if (index > 0)
            {
                currentAbility = abilities[index - 1];
            }
            else
            {
                currentAbility = abilities[abilities.Count - 1];
            }
        }


        public string GetSelWeaponName()
        {
            return currentWeapon.GetName();
        }

        public string GetSelAbilityName()
        {
            return currentAbility.GetName();
        }

        public bool getAbilityActive()
        {
            return currentAbility.getActive();
        }

        public double getAbilityTimer()
        {
            return currentAbility.getTimer();
        }

        public IPlayer GetOwner()
        {
            return registration.GetOwner();
        }

        public IAbility GetCurrentAbility()
        {
            return currentAbility;
        }

        public double GetShieldHealth()
        {
            if (currentAbility is AbilityState_Shield)
            {
                return (currentAbility as AbilityState_Shield).getHealth();
            }
            else
            {
                return -1;
            }
        }

        [Obsolete("Use GetPosition in Unit instead")]
        public Vector2 GetShipPosition()
        {
            return position;
        }

        [Obsolete("Use SetPosition in Unit instead")]
        public void SetShipPosition(Vector2 pos)
        {
            position = pos;
        }

        [Obsolete("Use GetRotation in Unit instead")]
        public double GetRotation()
        {
            return rotation;
        }

        [Obsolete("Use SetRotation in Unit instead")]
        public void SetRotation(double rot)
        {
            rotation = rot;
        }

        [Obsolete("Use GetAnimationFrame in Unit instead")]
        public Rectangle GetAnimationFrame()
        {
            return animationFrame;
        }

        [Obsolete("Use SetAnimationFrame in Unit instead")]
        public void SetAnimationFrame(Rectangle anim)
        {
            animationFrame = anim;
        }
        public String GetShopWindow()
        {
            return shopWindow;
        }
        public void SetShopWindow(String nextWindow)
        {

            shopWindow = nextWindow;
        }


        /// <summary>
        /// If the ship is docked at a space station
        /// </summary>
        /// <returns></returns>
        public bool GetIsDocked()
        {
            return docked;
        }
        public void docking(GameTime gameTime)
        {
            docked = true;
            docktime = 0;
        }
        private bool BoughtWeapon(String weapon)
        {
            foreach (IWeapon w in weapons)
            {
                if (w.GetType() == weapon)
                {
                    return true;
                }
            }
            return false;

        }
        private bool BoughtAbility(String ability)
        {
            foreach (IAbility a in abilities)
            {
                if (a.GetType() == ability)
                {
                    return true;
                }
            }


            return false;
        }
        public void BuyArmor()
        {
            armorThreshold *= 1.1;


            shopString["armor"] = "Armor\nZ - Buy Armor " + "$100 " + (int)armorThreshold + "\nX - \nC - \nV - \nB - \nN - Exit";
            GetOwner().ShipUpgraded();
        }
        public void BuyTurnSpeed()
        {
            maxTurnSpeed += 0.1;
            shopString["engine"] = "Engine\nZ - Thrust $150 " + (int)maxThrust + "\nX - \nC - Turn Speed $150 " + Math.Round(maxTurnSpeed, 2) + "\nV - \nB - \nN - Exit";
            GetOwner().ShipUpgraded();
        }
        public void BuyThrust()
        {
            maxThrust += 1000;
            shopString["engine"] = "Engine\nZ - Thrust $150 " + (int)maxThrust + "\nX - \nC - Turn Speed $150 " + Math.Round(maxTurnSpeed, 2) + "\nV - \nB - \nN - Exit";
            GetOwner().ShipUpgraded();
        }
        public bool BuyWeapon(String weapon)
        {
            if (!BoughtWeapon(weapon))
            {

                weapons.Add(ConcreteWeaponFactory.CreateWeapon(weapon));
                shopString["weapons1"] = "Weapons 1\nZ - Standard Gun " + (BoughtWeapon("gun") ? "- Bought" : "$200") + "\nX - Rapidgun " + (BoughtWeapon("rapidgun") ? "- Bought" : "$600") + "\nC - Laser " + (BoughtWeapon("laser") ? "- Bought" : "$1000") + "\nV - \nB - Next Page\nN - Exit";
                shopString["weapons2"] = "Weapons 2\nZ - \nX - \nC - \nV - \nB - Next Page\nN - Exit";
                GetOwner().ShipUpgraded();
                return true;
            }
            return false;
        }
        public bool BuyAbility(String ability)
        {
            if (!BoughtAbility(ability))
            {
                abilities.Add(ConcreteAbilityFactory.CreateAbility(ability));

                shopString["abilities1"] = "Abilities 1\nZ - Shield " + (BoughtAbility("shield") ? " - Bought" : "$500") + "\nX - \nC - \nV - \nB - Next Page\nN - Exit";
                shopString["abilities2"] = "Abilities 2\nZ - \nX - \nC - \nV - \nB - Next Page\nN - Exit";
                GetOwner().ShipUpgraded();
                return true;
            }
            return false;
        }
     
        /// <summary>
        /// Repair health first, then armor
        /// </summary>
        /// <param name="gameTime"></param>
        public bool Repair(GameTime gameTime)
        {
            if (health != maxHealth)
            {

                if (30 + health > maxHealth)
                {
                    health = maxHealth;
                    return true;
                }

                health += 30;
                return true;
            }

            if (armorEffectiveness < 100)
            {
                double inc = 20 * gameTime.ElapsedGameTime.TotalSeconds;

                if (armorEffectiveness + inc > 100)
                {
                    armorEffectiveness = 100;
                    return true;
                }
                armorEffectiveness += inc;

            }
            return false;
        }


        public virtual int GetNumWeaponSlots() {
            return 4;
        }

        public virtual int GetNumAbilitySlots()
        {
            return 2;
        }

        public virtual double GetArmorThreshold()
        {
            return armorThreshold;
        }

        public virtual void SetArmorThreshold(double threshold)
        {
            armorThreshold = threshold;
        }

        public virtual double GetMaxThrust()
        {
            return maxThrust;
        }

        public virtual void SetMaxThrust(double thrust)
        {
            maxThrust = thrust;
        }

        public virtual string GetWeapons()
        {
            string weapons = "";
            foreach (IWeapon weapon in this.weapons)
            {
                
                weapons += "|" + weapon.GetType();
            }
            return weapons;
        }

        public virtual void SetWeapons(string weaponTypes)
        {
            string[] weaponTypeArr = weaponTypes.Split('|');

            weapons = new List<IWeapon>();
            foreach (string type in weaponTypeArr)
            {
                if (!String.IsNullOrEmpty(type))
                {
                    weapons.Add(ConcreteWeaponFactory.CreateWeapon(type));
                }
            }
        }

        public virtual string GetAbilities()
        {
            string abilities = "";
            foreach (IAbility ability in this.abilities)
            {

                abilities += "|" + ability.GetType();
            }
            return abilities;
        }

        public virtual void SetAbilities(string abilityTypes)
        {
            string[] abilityTypeArr = abilityTypes.Split('|');

            abilities = new List<IAbility>();
            foreach (string type in abilityTypeArr)
            {
                if (!String.IsNullOrEmpty(type))
                {
                    abilities.Add(ConcreteAbilityFactory.CreateAbility(type));
                }
            }
        }

        public virtual double GetMaxTurnSpeed()
        {
            return maxTurnSpeed;
        }

        public virtual void SetMaxTurnSpeed(double speed)
        {
            this.maxTurnSpeed = speed;
        }
    }
}
