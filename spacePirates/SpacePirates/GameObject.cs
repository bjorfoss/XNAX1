using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;
using SpacePirates.Player;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


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

        // Holds the player unit : spaceship
        private ISpaceShip cameraTarget;
        
        //Hashtable with all spaceships
        private Dictionary<String, IShipFactory> shipFactoryCollection;

        private bool gameSetup;

        public bool active = false; // Is true if this is the currentObject in Game1.cs

        // Holds the spaceShips belonging to each team
        private List<ISpaceShip> redTeam;
        private List<ISpaceShip> blueTeam;

        // Holds a collection of obstacles: asteroids, fired obstacles ...
        private List<IObstacle> obstacles;

        private List<Unit> objectsInGame;

        //Holds the global maximum speed of any object
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

            maxSpeed = 300;


            shipFactoryCollection = new Dictionary<String, IShipFactory>();
            shipFactoryCollection.Add("fighter", new Factory_Fighter());

            self.goalLimit = goalsToWin;
            self.redScore = 0;
            self.blueScore = 0;

            self.spritefont = Content.Load<SpriteFont>("Graphics/Spritefonts/Menutext");

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

        public static GameObject Instance(int w, int h, ContentManager Content, int defaultGoalLimit=25)
        {
            lock (padlock) {
                if (instance == null) {
                    instance = new GameObject(w, h, Content, defaultGoalLimit);
                }
                return instance;
            }
        }

        public static GameObject Instance()
        {
            lock (padlock)
            {
                return instance;
            }
        }


        // TODO: make ship selection random

        // Sets up AI ship
        private ISpaceShip setUpShip()
        {
            String shipType = "fighter";
            return setUpShip(Ai.createController(), shipType, Vector2.Zero);
        }

        // Sets up ship for Player
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
        private void addToGame(List<ISpaceShip> list, ISpaceShip iSpaceShip)
        {
            list.Add(iSpaceShip);
            addToGame((Unit)iSpaceShip);
        }

        // Adds to unit collection in game
        public void addToGame(Unit unit)
        {
            objectsInGame.Add(unit);
            
        }

        public void setUpGame()
        {
            gameSetup = false;

            if (NetworkObject.Instance().getNetworked())
            {
                int nPlayers = NetworkObject.Instance().getNetworksession().AllGamers.Count;
                int nPut = 0;

                foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
                {
                    IPlayer human = player.Tag as Human;

                    
                    if (nPut < (nPlayers / 2))
                    {
                        ISpaceShip ship = setUpShip(human, "fighter", new Vector2(300, 600));
                        addToGame(redTeam, ship);
                        if (player.IsLocal)
                            cameraTarget = ship;
                    }
                    else
                    {
                        ISpaceShip ship = setUpShip(human, "fighter", new Vector2(500, 600));
                        addToGame(blueTeam, ship);
                        if (player.IsLocal)
                            cameraTarget = ship;
                    }

                    nPut++;
                }
            }
            else
            {
                IPlayer player = Human.createController();

                cameraTarget = setUpShip(player, "fighter", new Vector2(500, 600));

                addToGame(redTeam, cameraTarget);



                
                for (int i = 0; i < numberOfShips; i++)
                {
                    spaceShips[i] = setUpShip();
                    if (i < (numberOfShips / 2) - 1)
                    {
                        redTeam.Add(spaceShips[i]);
                        addToGame(redTeam, spaceShips[i]);
                    }
                    else
                    {
                        blueTeam.Add(spaceShips[i]);
                        addToGame(blueTeam, spaceShips[i]);
                    }
                }

            }
           
        }

        

        public void executeGameLogic(GameTime gameTime)
        {
            //If we need to set up the game first, do so.
            if (gameSetup)
            {
                setUpGame();
            }



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
                        (player.Tag as Human).HandleInput(Keyboard.GetState());
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
                        (owner as Human).HandleInput(Keyboard.GetState());
                    //else
                    //(ship.GetOwner() as Ai)
                }
                foreach (ISpaceShip ship in blueTeam)
                {
                    IPlayer owner = ship.GetOwner();

                    if (owner is Human)
                        (owner as Human).HandleInput(Keyboard.GetState());
                    //else
                    //(ship.GetOwner() as Ai)
                }
            }


            for (int i = 0; i < objectsInGame.Count; i++)
            {
                Unit unit = (objectsInGame.ElementAt(i) as Unit);
                unit.Update(gameTime);
                unit.CalculateDirectionAndSpeed(gameTime);
                unit.UpdatePosition(gameTime);
                unit.UpdateFacing(gameTime);
            }

            if (NetworkObject.Instance().getNetworked())
            {
                NetworkObject.Instance().getNetworksession().Update();
            }

            //Vector2 playerPosition = cameraTarget.UpdatePosition(new Vector2(0, 0));

            //foreach
            //UnitARRAY.UpdatePosition(playerPosition);

        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            level.executeDraw(spriteBatch);

            foreach (NetworkGamer player in NetworkObject.Instance().getNetworksession().AllGamers)
            {
                if (player.IsLocal)
                {
                    (cameraTarget as Unit).Draw(spriteBatch);
                }
                else
                {
                    IPlayer human = player.Tag as Human;

                    ISpaceShip ship = human.GetShip();

                    Vector2 pos = new Vector2(300, 600);
                    Color col = Color.OrangeRed;

                    if (blueTeam.Contains(ship))
                    {
                        pos = new Vector2(500, 600);
                        col = Color.LightBlue;
                    }

                    (ship as Unit).Draw(spriteBatch);
                    spriteBatch.DrawString(spritefont, player.Gamertag, pos, col);
                }
            }
            /*
            foreach (ISpaceShip ship in redTeam )
            {
                (ship as Unit).Draw(spriteBatch);
                spriteBatch.DrawString(spritefont, ship.GetOwner().GetName(), new Vector2(300, 600), Color.OrangeRed); 
            }
            foreach (ISpaceShip ship in blueTeam)
            {
                (ship as Unit).Draw(spriteBatch);
                spriteBatch.DrawString(spritefont, ship.GetOwner().GetName(), new Vector2(500, 600), Color.LightBlue);
            }*/

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
    }
}
