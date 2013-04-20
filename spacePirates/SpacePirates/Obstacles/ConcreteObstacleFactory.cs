using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.Obstacles
{
    public class ConcreteObstacleFactory
    {
        private Dictionary<String, IObstacleFactory> factories;
        private static ConcreteObstacleFactory instance;
        static readonly object padlock = new Object();

        private ConcreteObstacleFactory()
        {
            factories = new Dictionary<string, IObstacleFactory>();
            factories.Add("asteroid", new Factory_Asteroid());
            factories.Add("bullet", new Factory_Bullet());
            factories.Add("laser", new Factory_Laser());
        }

        /// <summary>
        /// Get an instance of the factory
        /// </summary>
        /// <returns></returns>
        public static ConcreteObstacleFactory Instance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new ConcreteObstacleFactory();
                }
                return instance;
            }
        }

        /// <summary>
        /// Create the obstacle
        /// </summary>
        /// <param name="type">Obstacle type</param>
        /// <param name="position">Where to spawn</param>
        /// <param name="velocity">How it moves</param>
        /// <returns></returns>
        public static IObstacle CreateObstacle(String type, Vector2 position, Vector2 velocity, float obstRotation=0.0f) {
            return Instance().factories[type].CreateObstacle(position, velocity, obstRotation);
        }

        /// <summary>
        /// Get the factories available.
        /// Consider using GetObstacleTypes and CreateObstacle instead.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, IObstacleFactory> GetFactories()
        {
            return Instance().factories;
        }

        /// <summary>
        /// Get a list of available ship types.
        /// </summary>
        /// <returns></returns>
        public static List<String> GetObstacleTypes()
        {
            return Instance().factories.Keys.ToList();
        }

        class Factory_Bullet : IObstacleFactory
        {

            public IObstacle CreateObstacle(Vector2 position, Vector2 velocity, float obstRotation)
            {
                return new ConcreteObstacle_Bullet(velocity, position);
            }
        }

        class Factory_Asteroid : IObstacleFactory
        {

            public IObstacle CreateObstacle(Vector2 position, Vector2 velocity, float obstRotation)
            {
                return new ConcreteObstacle_Asteroid(velocity, position);
            }
        }

        class Factory_Laser : IObstacleFactory
        {

            public IObstacle CreateObstacle(Vector2 position, Vector2 velocity, float rotation=0.0f)
            {
                return new ConcreteObstacle_Laser(velocity, position, rotation);
            }
        }
    }
}
