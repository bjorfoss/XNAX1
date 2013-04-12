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
        private Boolean test = true;
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


        

        private GameObject(int w, int h, ContentManager Content)
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
    
        public static GameObject Instance(int w, int h, ContentManager Content)
        {
            lock (padlock) {
                if (instance == null) {
                    instance = new GameObject(w, h, Content);
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
            return setUpShip(Ai.createController(), shipType);
        }

        private ISpaceShip setUpShip(IPlayer controller, String shipType)
        {
            Ownership registration = new Ownership();
            registration.SetOwner(controller);

            ISpaceShip ship = shipFactoryCollection[shipType].BuildSpaceship(registration, new Vector2(0, 0), 0);


            registration.SetShip(ship);
            redTeam.Add(ship);
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
         
            cameraTarget = setUpShip(player, "fighter");

           

            for (int i = 0; i < numberOfShips; i++) 
            {
                spaceShips[i] = setUpShip();
            }
           
        }

        public void executeGameLogic(float elapsed)
        {
            if (gameSetup)
            {
                setUpGame();
                

            }
            //Vector2 playerPosition = cameraTarget.UpdatePosition(new Vector2(0, 0));

            //foreach
            //UnitARRAY.UpdatePosition(playerPosition);

        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            level.executeDraw(spriteBatch);
            if (test)
            {
                //redTeam.Add(new ConcreteShip_Fighter(new Ownership(null, null), new Vector2(10, 10), 0));
                test = false;
            }

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
    }
}
