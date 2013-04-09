using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Player;

namespace SpacePirates
{
    

    class GameObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static GameObject instance;
        static readonly object padlock = new Object();
        private ContentManager Content;

        public static int numberOfShips = 10;

        ISpaceShip[] spaceShips = new ISpaceShip[10];

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;

        // Holds the level object
        private Level level;

        // Holds the player unit : spaceship
        private Unit cameraTarget;
        
        //Hashtable with all spaceships
        Dictionary<String, ShipFactory> shipFactoryCollection = new Dictionary<String, ShipFactory>();
        //shipFactoryCollection.Add("fighter", new Factory_Fighter());

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

            self.gameSetup = true;

            self.level = new Level(Content);

            // Holds the spaceShips belonging to each team
            self.redTeam = new List<ISpaceShip>();
            self.blueTeam = new List<ISpaceShip>();

            // Holds a collection of obstacles: asteroids, fired obstacles ...
            self.obstacles = new List<IObstacle>();

            maxSpeed = 25;
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

        private ISpaceShip setUpShip()
        {
            String shipType = "make random ship appear";
            return setUpShip(Ai.createController(), shipType);
        }

        private ISpaceShip setUpShip(IPlayer controller, String shipType)
        {
            Ownership registration = new Ownership();
            registration.setOwner(controller);

            ISpaceShip ship = shipFactoryColletion(shipType).BuildSpaceship(registration, new Vector2(0, 0), 0);

            registration.setShip(ship);

            return ship;
        }

        public void setUpGame()
        {
            gameSetup = false;

          
            IPlayer player = Human.createController();
         
            cameraTarget = setUpShip(player, "Type of ship");

           

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

        }

        public double getMaxSpeed()
        {
            return maxSpeed;
        }
    }
}
