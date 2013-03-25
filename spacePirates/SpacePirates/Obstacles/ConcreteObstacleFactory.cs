using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Obstacles
{
    public class Factory_Asteroid : ObstacleFactory
    { 
        IObstacle ObstacleFactory.CreateObstacle()
        {
            return new ConcreteObstacle_Asteroid();
        }
    }
}
