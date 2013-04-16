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

        SpaceShip[] spaceShips = new SpaceShip[10];

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;
        private Rectangle screenArea;

        // Holds the level object
        private Level level;

        //percent chance of astroid spawning per second
        private int chanceOfAstroidPerSecond = 5;

        // Holds the player unit : spaceship
        private SpaceShip cameraTarget;
        
        //Hashtable with all spaceships
        private Dictionary<String, IShipFactory> shipFactoryCollection;

        //Hashtable with obstacle types
        private Dictionary<String, ObstacleFactory> obstacleFactoryCollection;

        private bool gameSetup;

        public bool active = false; // Is true if this is the currentObject in Game1.cs

        // Holds the spaceShips belonging to each team
        private List<SpaceShip> redTeam;
        private List<SpaceShip> blueTeam;

        // Holds a collection of obstacles: asteroids, fired obstacles ...
        private List<IObstacle> obstacles;

        private List<Unit> objectsInGame;

        private List<SpaceStation> spaceStations; 

        //Holds the global maximum speed of any normal object
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
            self.redTeam = new List<SpaceShip>();
            self.blueTeam = new List<SpaceShip>();

            // Holds a collection of obstacles: asteroids, fired obstacles ...
            self.obstacles = new List<IObstacle>();

            // Holds everything classified as a unit
            self.objectsInGame = new List<Unit>();

            // Holds the spacestation in the game
            self.spaceStations = new List<SpaceStation>();

            maxSpeed = 300;


            shipFactoryCollection = new Dictionary<String, IShipFactory>();
            shipFactoryCollection.Add("fighter", new Factory_Fighter());

            obstacleFactoryCollection = new Dictionary<String, ObstacleFactory>();
            obstacleFactoryCollection.Add("astroid", new Factory_Asteroid());
            obstacleFactoryCollection.Add("bullet", new Factory_Bullet());

            self.goalLimit = goalsToWin;
            self.redScore = 0;
            self.blueScore = 0;

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


        // TODO: make ship selection random

        // Sets up AI ship
        private SpaceShip setUpShip()
        {
            String shipType = "fighter";
            return setUpShip(new Ai(), shipType, Vector2.Zero);
        }

        // Sets up ship 
        private SpaceShip setUpShip(IPlayer controller, String shipType, Vector2 position)
        {
            Ownership registration = new Ownership();
            registration.SetOwner(controller);

            SpaceShip ship = shipFactoryCollection[shipType].BuildSpaceship(registration, position, 0);

            registration.SetShip(ship);
            return ship;
        }

        public static SpaceShip GetCameraTarget()
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
        private void addToGame(List<SpaceShip> list, SpaceShip spaceShip)
        {
            list.Add(spaceShip);
            addToGame(spaceShip);
        }

        // Adds spacestations to the game
        private void addToGame(List<SpaceStation> list, SpaceStation Station)
        {
            list.Add(Station);
            addToGame((Unit)Station);

        }

        // Adds to unit collection in game
        public void addToGame(Unit unit)
        {
            objectsInGame.Add(unit);
            
        }

        public void setUpGame()
        {
            gameSetup = false;

          
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
            addToGame(spaceStations, new SpaceStation(new Vector2(0f, 0f)));


           
        }

        private void generateAstroids(GameTime gameTime)
        {

            String obstacleType = "astroid";

            // chance of Asteroid being created
            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            float chance = randomNumber * (float)gameTime.ElapsedGameTime.TotalSeconds;

            

            if (chance < chanceOfAstroidPerSecond)
            {

                // Generate outside level limit with a directional velocity that will make it go across the level

                Vector2 position = new Vector2(0);

                Vector2 velocity = new Vector2(0);

                IObstacle asteroid = obstacleFactoryCollection[obstacleType].CreateObstacle(position, velocity);

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
            foreach (SpaceShip ship in redTeam)
            {
                IPlayer owner = ship.GetOwner();

                if (owner is Human)
                {
                    (owner as Human).HandleInput(gameTime);
                }
                //else
                //(ship.GetOwner() as Ai)
            }
            foreach (SpaceShip ship in blueTeam)
            {
                IPlayer owner = ship.GetOwner();

                if (owner is Human)
                {
                    (owner as Human).HandleInput(gameTime);
                }
                //else
                //(ship.GetOwner() as Ai)
            }

            //Updates all the units in the game
            for (int i = 0; i < objectsInGame.Count; i++)
            {
                Unit unit = objectsInGame.ElementAt(i);
                unit.Update(gameTime);
                unit.UpdateUnit(gameTime);
                
            }
           
            //Vector2 playerPosition = cameraTarget.UpdatePosition(new Vector2(0, 0));

            //foreach
            //UnitARRAY.UpdatePosition(playerPosition);

        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            level.executeDraw(spriteBatch);

            //TODO: Investigate why not drawn for bjorfoss without this:
            //(cameraTarget as Unit).Draw(spriteBatch);

            foreach (Unit unit in objectsInGame)
            {
                unit.Draw(spriteBatch);
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

        public ObstacleFactory getOFactory(String name)
        {
            return obstacleFactoryCollection[name];
        }
    }
}
