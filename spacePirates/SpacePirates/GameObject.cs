using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;
using SpacePirates.Player;
using SpacePirates.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SpacePirates
{
    

    class GameObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static GameObject instance;
        static readonly object padlock = new Object();
        // private ContentManager Content; //Inherited

        public static int numberOfShips = 10;
        public GraphicsDevice graphicsDevice;

        ISpaceShip[] spaceShips = new ISpaceShip[10];

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;
        private Rectangle screenArea;

        // Holds the level object
        private Level level;

        //percent chance of astroid spawning per second
        private float chanceOfAstroidPerSecond = 0;

        // Holds the player unit : spaceship
        private ISpaceShip cameraTarget;

        private bool gameSetup;

        public bool active = false; // Is true if this is the currentObject in Game1.cs

        // Holds the spaceShips belonging to each team
        private List<ISpaceShip> redTeam;
        private List<ISpaceShip> blueTeam;

        // Holds a collection of obstacles: asteroids, fired obstacles ...
        private List<IObstacle> obstacles;

        private List<Unit> objectsInGame;

        private List<SpaceStation> spaceStations;

        private List<Explosion> explosions;

        //Holds the global maximum speed of any normal object
        private double maxSpeed;

        //Game mode variables.
        private int goalLimit;
        private int redScore;
        private int blueScore;
        private string victoryText = "";
        private double respawnCooldown = 15.0;
        private Vector2 redSpaceStationPos = new Vector2(2500, 2500);
        private Vector2 blueSpaceStationPos = new Vector2(7500, 7500);       

        SpriteFont spritefont;

        private PacketReader packetReader = new PacketReader();
        private PacketWriter packetWriter = new PacketWriter();

        private GameObject(int w, int h, ContentManager Content, int goalsToWin, GraphicsDevice graphicsDevice)
        {
            GameObject self = this;
            this.Content = Content;
            self.windowWidth = w;
            self.windowHeight = h;
            screenArea = new Rectangle(0, 0, w, h);
            self.graphicsDevice = graphicsDevice;

            self.gameSetup = true;

            self.level = new Level(Content);

            // Holds the spaceShips belonging to each team
            self.redTeam = new List<ISpaceShip>();
            self.blueTeam = new List<ISpaceShip>();

            // Holds a collection of obstacles: asteroids, fired obstacles ...
            self.obstacles = new List<IObstacle>();

            // Holds everything classified as a unit
            self.objectsInGame = new List<Unit>();

            // Holds the spacestation in the game
            self.spaceStations = new List<SpaceStation>();

            // Holds the explosions in the game
            self.explosions = new List<Explosion>();

            maxSpeed = 300;           

            self.goalLimit = goalsToWin;
            self.redScore = 0;
            self.blueScore = 0;

            self.spritefont = Content.Load<SpriteFont>("Graphics/Spritefonts/Menutext");

        }

        public static Level GetLevel()
        {
            return Instance().level;
        }

        public static Rectangle GetScreenArea()
        {
            return Instance().screenArea;
        }

        public static ContentManager GetContentManager()
        {
            return GameObject.instance.Content;
        }

        public bool isActive()
        {
            return active;
        }

        /// <summary>
        /// Call this for first time setup of the GameObject
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="Content"></param>
        /// <param name="defaultGoalLimit"></param>
        /// <returns></returns>
        public static GameObject Instance(int w, int h, ContentManager Content,GraphicsDevice graphicsDevice, int defaultGoalLimit=2)
        {
            lock (padlock) {
                if (instance == null)
                {
                    instance = new GameObject(w, h, Content, defaultGoalLimit, graphicsDevice);
                }
                return instance;
            }
        }

        /// <summary>
        /// Call this to get the GameObject
        /// </summary>
        /// <returns></returns>
        public static GameObject Instance()
        {
            lock (padlock)
            {              
                return instance;
            }
        }

        [Obsolete("Use method in ConcreteShipFactory instead.")]
        public Dictionary<String, IShipFactory> GetShipCollection()
        {
            return ConcreteShipFactory.GetFactories();
        }

        public double GetRespawnCooldown()
        {
            return respawnCooldown;
        }


        // TODO: make ship selection random

        // Sets up AI ship
        private ISpaceShip setUpShip()
        {
            String shipType = "fighter";
            return setUpShip(new Ai(), shipType, Vector2.Zero);
        }

        // Sets up ship 
        private ISpaceShip setUpShip(IPlayer controller, String shipType, Vector2 position)
        {
            Ownership registration = new Ownership();
            registration.SetOwner(controller);
            controller.SetOwnerShip(registration);

            ISpaceShip ship = ConcreteShipFactory.BuildSpaceship(shipType, registration, position, 0);

            registration.SetShip(ship);
            return ship;
        }

        public static ISpaceShip GetCameraTarget()
        {
            return GameObject.Instance().cameraTarget;
        }

        // Adds obstacles to the game
        private void addToGame(List<IObstacle> list, IObstacle iObstacle)
        {
            list.Add(iObstacle);
            addToGame((Unit)iObstacle);
        }

        public void addToGame(IObstacle obstacle)
        {
            obstacles.Add(obstacle);
            addToGame((Unit)obstacle);
        }

        // Adds spaceships to the game
        private void addToGame(List<ISpaceShip> list, ISpaceShip spaceShip)
        {
            list.Add(spaceShip);
            addToGame((spaceShip as Unit));
        }

        // Adds spacestations to the game
        private void addToGame(List<SpaceStation> list, SpaceStation Station)
        {
            list.Add(Station);
            //addToGame((Unit)Station);

        }

        // Adds to unit collection in game
        public void addToGame(Unit unit)
        {
            objectsInGame.Add(unit);
        }

        public void addToGame(Explosion explosion)
        {
            explosions.Add(explosion);
        }

        public void removeFromGame(Unit unit)
        {
            objectsInGame.Remove(unit);
            if (unit is SpaceShip)
            {
                redTeam.Remove(unit as SpaceShip);
                blueTeam.Remove(unit as SpaceShip);
            }
            else if (unit is IObstacle)
            {
                obstacles.Remove(unit as IObstacle);
            }
        }

        public void removeFromGame(Explosion explosion)
        {
            explosions.Remove(explosion);
        }

        public void removeFromObjects(Unit unit)
        {
            objectsInGame.Remove(unit);
        }

        public Vector2 getRedSpawn()
        {
            return redSpaceStationPos;
        }

        public Vector2 getBlueSpawn()
        {
            return blueSpaceStationPos;
        }

        public string getVictoryText()
        {
            return victoryText;
        }

        public void setUpGame()
        {
            gameSetup = false;

            if (NetworkObject.Instance().getNetworked())
            {
                foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
                {
                    int nTeam = (player.Tag as Human).GetTeam();
                    ISpaceShip ship;
                    string shipType = (player.Tag as Human).GetShipSelection();

                    if (nTeam == 1)
                    {
                        ship = setUpShip((player.Tag as Human), shipType, redSpaceStationPos);
                        addToGame(redTeam, ship);
                    }
                    else
                    {
                        ship = setUpShip((player.Tag as Human), shipType, blueSpaceStationPos);
                        addToGame(blueTeam, ship);
                    }

                    (ship as SpaceShip).setUnitID(player.Gamertag);

                    if (player.IsLocal)
                        cameraTarget = ship;
                }
                addToGame(spaceStations, new SpaceStation(redSpaceStationPos, Color.Red));



                addToGame(spaceStations, new SpaceStation(blueSpaceStationPos, Color.Aqua));

                victoryText = "";

	   }

           
        }

        private void generateAstroids(GameTime gameTime, Human host)
        {
            if (obstacles.Count > 120)
                return;

            String obstacleType = "asteroid";

            // chance of Asteroid being created
            Random random = new Random();
            
            chanceOfAstroidPerSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (chanceOfAstroidPerSecond > 2)
            {
                chanceOfAstroidPerSecond = 0;
                // Generate outside level limit with a directional velocity that will make it go across the level

                int astroidMaxSpeed = 240;
                int astroidMinSpeed = 160;

                Vector2 astroidStart;
                Vector2 astroidVelocity;

                Rectangle levelBounds = level.GetLevelBounds();

                bool sideOrTop = random.Next(1, 3) % 2 == 0 ? true : false;
                bool leftOrRight = random.Next(1, 3) % 2 == 0 ? true : false;

                if (sideOrTop)
                {
                    // Astroid is placed on the sides of the rectangle
                    astroidStart.X = levelBounds.Width / 2;
                    astroidStart.Y = random.Next(0, levelBounds.Height);

                    if (leftOrRight)
                    {
                        // Astroid is placed on the left side
                        astroidVelocity.X = random.Next(astroidMinSpeed, astroidMaxSpeed);
                        astroidStart.X = levelBounds.Center.X - astroidStart.X;
                    }
                    else
                    {
                        // Astroid is placed on the right side
                        astroidVelocity.X = random.Next(astroidMinSpeed, astroidMaxSpeed) * -1;
                        astroidStart.X = levelBounds.Center.X + astroidStart.X;
                    }

                    astroidVelocity.Y = random.Next(astroidMinSpeed / 5, astroidMaxSpeed / 5);
                }
                else
                {
                    // Asroid is place over or under rectangle
                    astroidStart.X = random.Next(0, levelBounds.Height);
                    astroidStart.Y = levelBounds.Height  / 2;

                    if (leftOrRight)
                    {
                        // Astroid is placed under rectangle
                        astroidVelocity.Y = random.Next(astroidMinSpeed, astroidMaxSpeed);
                        astroidStart.Y = levelBounds.Center.Y - astroidStart.Y;
                    }
                    else
                    {
                        // Astroid is placed over rectangle
                        astroidVelocity.Y = random.Next(astroidMinSpeed, astroidMaxSpeed) * -1;
                        astroidStart.Y = levelBounds.Center.Y + astroidStart.Y;
                    }

                    astroidVelocity.X = random.Next(astroidMinSpeed / 5, astroidMaxSpeed / 5);
                }

                IObstacle asteroid = ConcreteObstacleFactory.CreateObstacle(obstacleType, astroidStart, astroidVelocity);
                addToGame(obstacles, asteroid);

                host.HostAsteroidGeneration(asteroid);

                Console.WriteLine("Created astroid with position " + astroidStart.X + "," + astroidStart.Y + " and velocity " + astroidVelocity.X + "," + astroidVelocity.Y);
            }


        }

        private void EndgameCleanup()
        {
            redScore = 0;
            blueScore = 0;
            //victoryText = "";

            obstacles = new List<IObstacle>();

            objectsInGame = new List<Unit>();

            spaceStations = new List<SpaceStation>();

            explosions = new List<Explosion>();

            redTeam = new List<ISpaceShip>();
            blueTeam = new List<ISpaceShip>();

            spaceStations = new List<SpaceStation>();

            gameSetup = true;
        }

        

        public void executeGameLogic(GameTime gameTime)
        {
            if (gameSetup)
            {
                setUpGame();
            }

            ReceiveNetworkData(gameTime);

            //Has any side won already?
            if (redScore >= goalLimit)
            {
                //Show victory red team.
                //Should perhaps show score screen of some manner before setting gameobject as false and going back to the menu?
                victoryText = "Red Team Victorious! (" + redScore.ToString() + " -- " + blueScore.ToString() + ")";
                active = false;
                MenuObject.Instance().SetVictoryLobby();
                EndgameCleanup();

            } else if (blueScore >= goalLimit)
            {
                //Show victory blue team.
                victoryText = "Blue Team Victorious! (" + redScore.ToString() + " -- " + blueScore.ToString() + ")";
                active = false;
                MenuObject.Instance().SetVictoryLobby();
                EndgameCleanup();
            }

            if (NetworkObject.Instance().getNetworked())
            {
                foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
                {
                    if (player.IsLocal)
                    {
                        Human test = (player.Tag as Human);
                        test.HandleInput(Keyboard.GetState(), (gameTime));

                        if(player.IsHost)
                            generateAstroids(gameTime, test);
                    }           
                }
            }
            else
            {
                //Handle input from players.
                foreach (ISpaceShip ship in redTeam)
                {
                    IPlayer owner = ship.GetOwner();

                    if (owner is Human)
                        (owner as Human).HandleInput(Keyboard.GetState(), (gameTime));
                    //else
                    //(ship.GetOwner() as Ai)
                }
                foreach (ISpaceShip ship in blueTeam)
                {
                    IPlayer owner = ship.GetOwner();

                    if (owner is Human)
                        (owner as Human).HandleInput(Keyboard.GetState(), (gameTime));
                    //else
                    //(ship.GetOwner() as Ai)
                }
            }
            
            //Update all explosions
            List<Explosion> exClone = Extensions.CloneExplosions(explosions);
            foreach (Explosion ex in exClone)
            {
                if (ex.update(gameTime))
                {
                    ex.playSound();
                    List<Unit> objectClone = Extensions.CloneUnits(objectsInGame); 
                    foreach (Unit unit in objectClone)
                    {
                            if (ex.getExplosionRectangle().Intersects(unit.getUnitRectangle()))
                            {
                                if(unit.getHealth() > 0)
                                    unit.damage(ex.getDamage(), gameTime);
                            }
                        }
                    }
            }

            //Updates all the units in the game
            for (int i = 0; i < objectsInGame.Count; i++)
            {
                Unit unit = objectsInGame.ElementAt(i);
                unit.Update(gameTime);
                unit.UpdateUnit(gameTime);
                if (unit is ISpaceShip)
                {
                    (unit as ISpaceShip).updateAbilities(gameTime);
                }
                
            }
            foreach (SpaceStation station in spaceStations)
            {
                station.Update(gameTime);
            }
            foreach (SpaceShip ship in redTeam){

                

                if((ship as Unit).getUnitRectangle().Intersects(spaceStations.ElementAt(0).getRectangle()))
                {
                    ship.docking(gameTime);
                }
                if ((ship as Unit).getUnitRectangle().Intersects(new Rectangle(spaceStations.ElementAt(1).getRectangle().X - 300, spaceStations.ElementAt(1).getRectangle().Y - 300, spaceStations.ElementAt(1).getRectangle().Width + 600, spaceStations.ElementAt(1).getRectangle().Height + 600)) && (int)gameTime.TotalGameTime.TotalMilliseconds % 1000 == 0)
                {
                    //Enemy ship! Get it! If it's not dead.
                    if (!(ship.GetOwner() as Human).GetDestroyed())
                    {
                        (ship as Unit).damage(1500, gameTime);
                        addToGame(new Explosion((ship as Unit).GetPosition(), new Vector2((float)60, (float)60), 0, Color.Blue));
                    }
                }
            }
            foreach (SpaceShip ship in blueTeam)
            {

                if ((ship as Unit).getUnitRectangle().Intersects(spaceStations.ElementAt(1).getRectangle()))
                {
                    ship.docking(gameTime);
                }
                if ((ship as Unit).getUnitRectangle().Intersects(new Rectangle(spaceStations.ElementAt(0).getRectangle().X - 300, spaceStations.ElementAt(0).getRectangle().Y - 300, spaceStations.ElementAt(0).getRectangle().Width + 600, spaceStations.ElementAt(0).getRectangle().Height + 600)) && (int)gameTime.TotalGameTime.TotalMilliseconds % 1000 == 0)
                {
                    //Enemy ship! Get it! If it's not dead.
                    if (!(ship.GetOwner() as Human).GetDestroyed())
                    {
                        (ship as Unit).damage(1500, gameTime);
                        addToGame(new Explosion((ship as Unit).GetPosition(), new Vector2((float)60, (float)60), 0, Color.Red));
                    }
                }
            }

            if (NetworkObject.Instance().getNetworked())
            {
                NetworkObject.Instance().getNetworksession().Update();
            }            

            //Collision check
            for (int i = 0; i < objectsInGame.Count; i++)
            {
                for (int j = i + 1; j < objectsInGame.Count; j++)
                {
                    if (objectsInGame.ElementAt(i).getUnitRectangle().Intersects(objectsInGame.ElementAt(j).getUnitRectangle()))
                    {
                        objectsInGame.ElementAt(i).Collide(objectsInGame.ElementAt(j), gameTime);
                    }
                }
            }

            SendNetworkData();
        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            level.executeDraw(spriteBatch);

            foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
            {
                
                if (player.IsLocal)
                {
                    Human human = player.Tag as Human;

                    ISpaceShip ship = human.GetShip();

                    Vector2 pos = new Vector2((ship as Unit).GetPosition().X, (ship as Unit).GetPosition().Y - (float)((ship as Unit).getUnitRectangle().Height/(1.5)));
                    Color col = Color.OrangeRed;

                    if (blueTeam.Contains(ship))
                    {
                        col = Color.LightBlue;
                    }

                    Unit unit = (ship as Unit);

                    if (unit.getHealth() > 0 && !(player.Tag as Human).GetDestroyed())
                    {
                        human.getHud().executeDraw(spriteBatch, screenArea);
                        unit.Draw(spriteBatch);
                        ship.drawAbilities(spriteBatch);
                        Vector2 screenPos = Unit.WorldPosToScreenPos(pos);
                        spriteBatch.DrawString(spritefont, player.Gamertag, screenPos, col, 0, spritefont.MeasureString(player.Gamertag) / 2, 1f, SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    Human human = player.Tag as Human;
                    if (human != null && !player.HasLeftSession)
                    {
                        ISpaceShip ship = human.GetShip();

                        Vector2 pos = new Vector2((ship as Unit).GetPosition().X, (ship as Unit).GetPosition().Y - (float)((ship as Unit).getUnitRectangle().Height / (1.5)));

                        
                        Color col = Color.OrangeRed;

                        if (blueTeam.Contains(ship))
                        {
                            
                            col = Color.LightBlue;
                        }

                        Unit unit = (ship as Unit);
                        //if (unit.getHealth() > 0)
                        if(!human.GetDestroyed())
                        {
                            unit.Draw(spriteBatch);
                            ship.drawAbilities(spriteBatch);
                            Vector2 screenPos = Unit.WorldPosToScreenPos(pos);
                            spriteBatch.DrawString(spritefont, player.Gamertag, screenPos, col, 0, spritefont.MeasureString(player.Gamertag)/2, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
            
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            foreach (Unit unit in objectsInGame)
            {
                if (unit is ISpaceShip)
                {
                    if (!((unit as ISpaceShip).GetOwner() is Human))
                    {
                        unit.Draw(spriteBatch);
                        (unit as ISpaceShip).drawAbilities(spriteBatch);
                    }
                }
                else
                {
                    unit.Draw(spriteBatch);
                }
            }
            foreach (SpaceStation station in spaceStations)
            {
                station.Draw(spriteBatch);
            }          

        }

        public double getMaxSpeed()
        {
            return maxSpeed;
        }

        //Get the GameObject instance and call this when red team destroys an enemy ship.
        public void redScored()
        {
            foreach (ISpaceShip ship in redTeam)
            {
                (ship.GetOwner() as Human).RecieveAwardCurrency();
            }
            redScore++;
        }

        public int getRedScore()
        {
            return redScore;
        }

        //Get the GameObject instance and call this when blue team destroys an enemy ship.
        public void blueScored()
        {
            foreach (ISpaceShip ship in blueTeam)
            {
                (ship.GetOwner() as Human).RecieveAwardCurrency();
            }
            blueScore++;
        }

        public int getBlueScore()
        {
            return blueScore;
        }

        private void ReceiveNetworkData(GameTime gameTime)
        {
            foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
            {
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;
                    gamer.ReceiveData(packetReader, out sender);

                    if (!sender.IsLocal)
                    {
                        Human senderHuman = sender.Tag as Human;
                        ISpaceShip ship = senderHuman.GetShip();

                        Vector2 pos;
                        double rot;
                        Vector2 vel;
                        double hp;
                        double armorEffectiveness;
                        Vector2 xy;
                        Vector2 wh;
                        bool firing;
                        bool abilityActivated;
                        double shieldHealth = 0.0;

                        bool destroyed;
                        bool rewardOpposition;

                        bool shipUpgraded;

                       // bool shipNextWep;
                       // bool shipPreviousWep;
                        bool shipNextAbility;
                        bool shipPrevAbility;

                        bool generatedAsteroid;
                        string asteroidName;
                        Vector2 startAsteroidPos;
                        Vector2 asteroidVelocity;

                        try
                        {
                            //This should be the same as was is sent in the send function.
                            
                            
                            pos = packetReader.ReadVector2();
                            rot = packetReader.ReadDouble();
                            vel = packetReader.ReadVector2();
                            hp = packetReader.ReadDouble();
                            armorEffectiveness = packetReader.ReadDouble();

                            xy = packetReader.ReadVector2();
                            wh = packetReader.ReadVector2();

                            //weapon changed
                            if (packetReader.ReadBoolean())
                            {
                                string weapType = packetReader.ReadString();
                                ship.SetWeapon(weapType);
                            }

                            firing = packetReader.ReadBoolean();
                            abilityActivated = packetReader.ReadBoolean();
                            shieldHealth = packetReader.ReadDouble();

                            destroyed = packetReader.ReadBoolean();
                            rewardOpposition = packetReader.ReadBoolean();

                            //ship upgrades
                            shipUpgraded = packetReader.ReadBoolean();
                            if (shipUpgraded)
                            {
                                ship.SetArmorThreshold(packetReader.ReadDouble());
                                ship.SetMaxThrust(packetReader.ReadDouble());
                                ship.SetMaxTurnSpeed(packetReader.ReadDouble());

                                ship.SetWeapons(packetReader.ReadString());
                                ship.SetAbilities(packetReader.ReadString());
                            }

                            //shipNextWep = packetReader.ReadBoolean();
                            //shipPreviousWep = packetReader.ReadBoolean();
                            shipNextAbility = packetReader.ReadBoolean();
                            shipPrevAbility = packetReader.ReadBoolean();

                            double totPos1 = Math.Sqrt(Math.Pow(pos.X, 2) + Math.Pow(pos.Y, 2));
                            double totPos2 = Math.Sqrt(Math.Pow((ship as Unit).getVelocity().X, 2) 
                                + Math.Pow((ship as Unit).getVelocity().Y, 2));
                            //If the difference between the local ship's position and the position of the sending ship
                            //is larger than 10, update the position
                            if (totPos1 - totPos2 > 5 || totPos1 - totPos2 < -5)
                            {
                                (ship as Unit).setPosition(pos);
                            }
                            (ship as Unit).SetRotation(rot);
                            (ship as Unit).setVelocity(vel);
                            if ((ship as Unit).getHealth() > 0 && hp <= 0)
                            {
                                (ship as Unit).SetHealth(hp);
                                (ship as Unit).OnDestroy(gameTime);
                            }
                            else { (ship as Unit).SetHealth(hp); }
                            (ship as Unit).SetArmorEffectiveness(armorEffectiveness);

                            (ship as Unit).SetAnimationFrame(new Rectangle((int)xy.X, (int)xy.Y, (int)wh.X, (int)wh.Y));


                            if (firing)
                                ship.Fire(gameTime);

                            if (abilityActivated)
                                ship.UseAbility(gameTime);

                            if (ship.GetCurrentAbility().getActive() && ship.GetCurrentAbility() is AbilityState_Shield)
                            {
                                (ship.GetCurrentAbility() as AbilityState_Shield).setHealth(shieldHealth);
                            }

                            
                            //senderHuman.SetDestroyed(destroyed, gameTime.TotalGameTime.TotalSeconds);

                            if (rewardOpposition)
                            {
                                int team = senderHuman.GetTeam();

                                if (team == 1)
                                    blueScored();
                                else
                                    redScored();
                            }


                            /*if (shipNextWep)
                                ship.NextWeapon();
                            if (shipPreviousWep)
                                ship.PreviousWeapon();*/
                            if (shipNextAbility)
                                ship.NextAbility();
                            if (shipPrevAbility)
                                ship.PreviousAbility();

                            if (sender.IsHost)
                            {
                                //Has an asteroid been generated?
                                generatedAsteroid = packetReader.ReadBoolean();

                                if (generatedAsteroid)
                                {
                                    asteroidName = packetReader.ReadString();
                                    startAsteroidPos = packetReader.ReadVector2();
                                    asteroidVelocity = packetReader.ReadVector2();

                                    IObstacle asteroid = ConcreteObstacleFactory.CreateObstacle(asteroidName, startAsteroidPos, asteroidVelocity);
                                    addToGame(obstacles, asteroid);
                                }
                            }

                        }
                        catch (EndOfStreamException)
                        {
                            //Debug!
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send data of local player ships to others
        /// </summary>
        private void SendNetworkData()
        {
            foreach (LocalNetworkGamer gamer in NetworkObject.Instance().getNetworksession().LocalGamers)
            {
                Human me = gamer.Tag as Human;
                Unit unit = (me.GetShip() as Unit);
                //This should be the same as is read in the read function.
                packetWriter.Write(unit.GetPosition());
                packetWriter.Write(unit.GetRotation());
                packetWriter.Write(unit.getVelocity());
                packetWriter.Write(unit.getHealth());
                packetWriter.Write(unit.getArmorEffectiveness());

                Rectangle anim = unit.GetAnimationFrame();
                packetWriter.Write(new Vector2(anim.X, anim.Y));
                packetWriter.Write(new Vector2(anim.Width, anim.Height));

                //weapon changed?
                bool weapChanged = me.HasWeaponChanged();
                packetWriter.Write(weapChanged);
                if (weapChanged)
                {
                    packetWriter.Write((unit as ISpaceShip).GetSelectedWeapon().GetTypeOf());
                }

                packetWriter.Write(me.GetFiring());
                //Regardless of whether we fired or not, set the bool to false.
                me.ShipFired();

                packetWriter.Write(me.GetAbilityActivated());
                double shieldHp = -1;
                if (me.GetAbilityActivated() && me.GetShip().GetCurrentAbility() is AbilityState_Shield)
                {
                    shieldHp = (me.GetShip().GetCurrentAbility() as AbilityState_Shield).getHealth();
                }
                packetWriter.Write(shieldHp);

                //Regardless of whether we activated or not, set the bool to false.
                me.setAbilityActivated();

                packetWriter.Write(me.GetDestroyed());

                bool rewardOpposition = me.GetAwardOpposition();

                packetWriter.Write(rewardOpposition);

                if (rewardOpposition)
                {
                    int team = me.GetTeam();

                    me.SetAwardOpposition(false);

                    if (team == 1)
                        blueScored();
                    else
                        redScored();
                }

                //deal with ship upgrades
                bool sendShipUpdates = me.WasShipUpgraded();
                packetWriter.Write(sendShipUpdates);
                if (sendShipUpdates)
                {
                    ISpaceShip ship = me.GetShip();
                    packetWriter.Write(ship.GetArmorThreshold());
                    packetWriter.Write(ship.GetMaxThrust());
                    packetWriter.Write(ship.GetMaxTurnSpeed());

                    packetWriter.Write(ship.GetWeapons());
                    packetWriter.Write(ship.GetAbilities());
                }

                //packetWriter.Write(me.GetNextWeaponChange());
                //packetWriter.Write(me.GetPrevWeaponChange());
                packetWriter.Write(me.GetNextAbilityChange());
                packetWriter.Write(me.GetPrevAbilityChange());

                if (gamer.IsHost)
                {
                    //Has an asteroid been generated?
                    bool generatedAsteroid = me.HasHostGeneratedAsteroid();
                    packetWriter.Write(generatedAsteroid);

                    if (generatedAsteroid)
                    {
                        IObstacle asteroid = me.GetGeneratedAsteroid();
                        packetWriter.Write("asteroid");
                        packetWriter.Write((asteroid as Unit).GetPosition());
                        packetWriter.Write((asteroid as Unit).getVelocity());

                        me.SetHasGeneratedAsteroid();
                    }


                }

                gamer.SendData(packetWriter, SendDataOptions.None);
            }
	}

        [Obsolete("This method is obsolete, use methods in ConcreteObstacleFactory instead")]
        public IObstacleFactory getOFactory(String name)
        {
            Dictionary<String, IObstacleFactory> f = ConcreteObstacleFactory.GetFactories();
            return f[name];
        }

    }
}
