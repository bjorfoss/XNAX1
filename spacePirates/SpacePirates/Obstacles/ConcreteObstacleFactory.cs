using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.Obstacles
{
    public class Factory_Bullet : ObstacleFactory
    { 

        public IObstacle CreateObstacle(Vector2 velocity, Vector2 position)
        {
            return new ConcreteObstacle_Bullet(velocity, position);
        }
    }
}
