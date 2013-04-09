using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Collections;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates
{
    class GameObject : Microsoft.Xna.Framework.Game, GameStates
    {
        private static GameObject instance;
        static readonly object padlock = new Object();

        // Holds the with and the height of the viewport
        private int windowWidth;
        private int windowHeight;

        // Holds the level object
        private Level level;

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

            self.windowWidth = w;
            self.windowHeight = h;

            self.level = new Level();

            // Holds the spaceShips belonging to each team
            self.redTeam = new List<ISpaceShip>();
            self.blueTeam = new List<ISpaceShip>();

            // Holds a collection of obstacles: asteroids, fired obstacles ...
            self.obstacles = new List<IObstacle>();

            maxSpeed = 25;
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

        public void executeGameLogic(float elapsed)
        {
        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
        }

        public double getMaxSpeed()
        {
            return maxSpeed;
        }
    }
}
