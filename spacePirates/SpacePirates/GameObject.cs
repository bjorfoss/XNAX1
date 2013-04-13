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

        //Holds the global maximum speed of any object
        private double maxSpeed;

        //Game mode variables.
        private int goalLimit;
        private int redScore;
        private int blueScore;


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

            maxSpeed = 25;


            shipFactoryCollection = new Dictionary<String, IShipFactory>();
            shipFactoryCollection.Add("fighter", new Factory_Fighter());

            self.goalLimit = goalsToWin;
            self.redScore = 0;
            self.blueScore = 0;

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

        private ISpaceShip setUpShip()
        {
            String shipType = "fighter";
            return setUpShip(Ai.createController(), shipType, Vector2.Zero);
        }

        private ISpaceShip setUpShip(IPlayer controller, String shipType, Vector2 position)
        {
            Ownership registration = new Ownership();
            registration.SetOwner(controller);

            ISpaceShip ship = shipFactoryCollection[shipType].BuildSpaceship(registration, position, 0);


            registration.SetShip(ship);
            return ship;
        }

        public static ISpaceShip GetCameraTarget()
        {
            return GameObject.Instance().cameraTarget;
        }

        public void setUpGame()
        {
            gameSetup = false;

          
            IPlayer player = Human.createController();

            cameraTarget = setUpShip(player, "fighter", new Vector2(500, 600));

            redTeam.Add(cameraTarget);

            for (int i = 0; i < numberOfShips; i++)
            {
                spaceShips[i] = setUpShip();
                if (i < (numberOfShips / 2) - 1)
                    redTeam.Add(spaceShips[i]);
                else
                    blueTeam.Add(spaceShips[i]);
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

            //Handle input from players.
            foreach (ISpaceShip ship in redTeam)
            {
                IPlayer owner = ship.GetOwner();

                if (owner is Human)
                    (owner as Human).HandleInput();
                //else
                //(ship.GetOwner() as Ai)
            }
            foreach (ISpaceShip ship in blueTeam)
            {
                IPlayer owner = ship.GetOwner();

                if (owner is Human)
                    (owner as Human).HandleInput();
                //else
                //(ship.GetOwner() as Ai)
            }

            for(int i = 0; i < blueTeam.Count; i++)
            {
                Unit unit = (blueTeam.ElementAt(i) as Unit);
                unit.Update(gameTime);
                unit.CalculateDirectionAndSpeed();
                unit.UpdatePosition(gameTime);
                unit.UpdateFacing(gameTime);
            }
            for (int i = 0; i < redTeam.Count; i++)
            {
                Unit unit = (redTeam.ElementAt(i) as Unit);
                unit.Update(gameTime);
                unit.CalculateDirectionAndSpeed();
                unit.UpdatePosition(gameTime);
                unit.UpdateFacing(gameTime);
            }

            
           

                       

            //Vector2 playerPosition = cameraTarget.UpdatePosition(new Vector2(0, 0));

            //foreach
            //UnitARRAY.UpdatePosition(playerPosition);

        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            level.executeDraw(spriteBatch);

            //TODO: Investigate why not drawn for bjorfoss without this:
            (cameraTarget as Unit).Draw(spriteBatch);

            foreach (ISpaceShip ship in redTeam )
            {
                ((Unit)ship).Draw(spriteBatch);
            }
            foreach (ISpaceShip ship in blueTeam)
            {
                ((Unit)ship).Draw(spriteBatch);
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
    }
}
