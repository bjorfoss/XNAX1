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
        
        //Hashtable with all spaceships
        private Dictionary<String, IShipFactory> shipFactoryCollection;

        //Hashtable with obstacle types
        private Dictionary<String, ObstacleFactory> obstacleFactoryCollection;

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

        SpriteFont spritefont;

        private PacketReader packetReader = new PacketReader();
        private PacketWriter packetWriter = new PacketWriter();

        private GameObject(int w, int h, ContentManager Content, int goalsToWin)
        {
            GameObject self = this;
            this.Content = Content;
            self.windowWidth = w;
            self.windowHeight = h;
            screenArea = new Rectangle(0, 0, w, h);

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

            shipFactoryCollection = new Dictionary<String, IShipFactory>();
            shipFactoryCollection.Add("fighter", new Factory_Fighter());
            shipFactoryCollection.Add("eightwing", new Factory_Eightwing());

            obstacleFactoryCollection = new Dictionary<String, ObstacleFactory>();
            obstacleFactoryCollection.Add("astroid", new Factory_Asteroid());
            obstacleFactoryCollection.Add("bullet", new Factory_Bullet());

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
        public static GameObject Instance(int w, int h, ContentManager Content, int defaultGoalLimit=25)
        {
            lock (padlock) {
                if (instance == null) {
                    instance = new GameObject(w, h, Content, defaultGoalLimit);
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

        public Dictionary<String, IShipFactory> GetShipCollection()
        {
            return shipFactoryCollection;
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

            ISpaceShip ship = shipFactoryCollection[shipType].BuildSpaceship(registration, position, 0);

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
                        ship = setUpShip((player.Tag as Human), shipType, new Vector2(300, 600));
                        addToGame(redTeam, ship);
                    }
                    else
                    {
                        ship = setUpShip((player.Tag as Human), shipType, new Vector2(500, 600));
                        addToGame(blueTeam, ship);
                    }

                    if (player.IsLocal)
                        cameraTarget = ship;
                }
                addToGame(spaceStations, new SpaceStation(new Vector2(200, 200)));



                addToGame(spaceStations, new SpaceStation(new Vector2(1000, 1000)));

	   }

           
        }

        private void generateAstroids(GameTime gameTime)
        {

            String obstacleType = "astroid";

            // chance of Asteroid being created
            Random random = new Random();
            
            chanceOfAstroidPerSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (chanceOfAstroidPerSecond > 10)
            {
                chanceOfAstroidPerSecond = 0;
                // Generate outside level limit with a directional velocity that will make it go across the level

                int astroidMaxSpeed = 120;
                int astroidMinSpeed = 80;

                Vector2 astroidStart;
                Vector2 astroidVelocity;

                Rectangle levelBounds = level.GetLevelBounds();

                bool sideOrTop = new Random().Next() % 2 == 0 ? true : false;
                bool leftOrRight = new Random().Next() % 2 == 0 ? true : false;

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

                IObstacle asteroid = obstacleFactoryCollection[obstacleType].CreateObstacle(astroidStart, astroidVelocity);
                addToGame(obstacles, asteroid);

                Console.WriteLine("Created astroid with position " + astroidStart.X + "," + astroidStart.Y + " and velocity " + astroidVelocity.X + "," + astroidVelocity.Y);
            }


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
            } else if (blueScore >= goalLimit)
            {
                //Show victory blue team.
            }

            if (NetworkObject.Instance().getNetworked())
            {
                foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
                {
                    if (player.IsLocal)
                    {
                        Human test = (player.Tag as Human);
                        //Human test = NetworkObject.Instance().getPlayer();
                        test.HandleInput(Keyboard.GetState(), (gameTime));
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
            
            List<Explosion> exClone = Extensions.CloneExplosions(explosions);
            foreach (Explosion ex in exClone)
            {
                foreach (Unit unit in objectsInGame)
                {
                    if (ex.update(gameTime))
                    {
                        if (ex.getExplosionRectangle().Intersects(unit.getUnitRectangle()))
                        {
                            unit.damage(ex.getDamage());
                        }
                    }
                }
            }

            generateAstroids(gameTime);

            //Updates all the units in the game
            for (int i = 0; i < objectsInGame.Count; i++)
            {
                Unit unit = objectsInGame.ElementAt(i);
                unit.Update(gameTime);
                unit.UpdateUnit(gameTime);
                
            }
            foreach (SpaceStation station in spaceStations)
            {
                station.Update(gameTime);
            }
            foreach (ISpaceShip ship in redTeam){

                if((ship as Unit).getUnitRectangle().Intersects(spaceStations.ElementAt(0).getRectangle()))
                {
                    spaceStations.ElementAt(0).dockShip(ship);
                }
            }
            foreach (ISpaceShip ship in blueTeam)
            {

                if ((ship as Unit).getUnitRectangle().Intersects(spaceStations.ElementAt(1).getRectangle()))
                {
                    spaceStations.ElementAt(1).dockShip(ship);
                }
            }

            if (NetworkObject.Instance().getNetworked())
            {
                NetworkObject.Instance().getNetworksession().Update();
            }            

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
                    //(cameraTarget as Unit).Draw(spriteBatch);
                    //IPlayer human = player.Tag as Human;
                    Human human = NetworkObject.Instance().getPlayer();

                    ISpaceShip ship = human.GetShip();

                    Vector2 pos = new Vector2(300, 600);
                    Color col = Color.OrangeRed;

                    if (blueTeam.Contains(ship))
                    {
                        pos = new Vector2(500, 600);
                        col = Color.LightBlue;
                    }

                    Unit unit = (ship as Unit);

                    if (unit.getHealth() > 0)
                    {
                        unit.Draw(spriteBatch);
                        Vector2 screenPos = Unit.WorldPosToScreenPos(pos);
                        screenPos -= new Vector2(50,150);
                        spriteBatch.DrawString(spritefont, player.Gamertag, pos, col);
                    }
                }
                else
                {
                    Human human = player.Tag as Human;
                    if (human != null && !player.HasLeftSession)
                    {
                        ISpaceShip ship = human.GetShip();

                        Vector2 pos = new Vector2(300, 600);
                        Color col = Color.OrangeRed;

                        if (blueTeam.Contains(ship))
                        {
                            pos = new Vector2(500, 600);
                            col = Color.LightBlue;
                        }

                        Unit unit = (ship as Unit);
                        if (unit.getHealth() > 0)
                        {
                            unit.Draw(spriteBatch);
                            Vector2 screenPos = Unit.WorldPosToScreenPos(pos);
                            screenPos -= new Vector2(50, 150);
                            spriteBatch.DrawString(spritefont, player.Gamertag, screenPos, col);
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
                unit.Draw(spriteBatch);
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
            redScore++;
        }

        //Get the GameObject instance and call this when blue team destroys an enemy ship.
        public void blueScored()
        {
            blueScore++;
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
                        Vector2 xy;
                        Vector2 wh;
                        bool firing;

                        try
                        {
                            //This should be the same as was is sent in the send function.
                            
                            
                            pos = packetReader.ReadVector2();
                            rot = packetReader.ReadDouble();

                            xy = packetReader.ReadVector2();
                            wh = packetReader.ReadVector2();
                            

                            firing = packetReader.ReadBoolean();

                            (ship as Unit).setPosition(pos);
                            (ship as Unit).SetRotation(rot);

                            (ship as Unit).SetAnimationFrame(new Rectangle((int)xy.X, (int)xy.Y, (int)wh.X, (int)wh.Y));

                            if (firing)
                                ship.Fire(gameTime);

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

                Rectangle anim = unit.GetAnimationFrame();
                packetWriter.Write(new Vector2(anim.X, anim.Y));
                packetWriter.Write(new Vector2(anim.Width, anim.Height));

                packetWriter.Write(me.GetFiring());
                me.ShipFired();
                
                gamer.SendData(packetWriter, SendDataOptions.None);
            }
	}
        public ObstacleFactory getOFactory(String name)
        {
            return obstacleFactoryCollection[name];
        }
    }
}
