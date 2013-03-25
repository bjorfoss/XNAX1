using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Obstacle
{
    public class Factory_Asteroid : ObstacleFactory
    { // Executes third if OS:OSX
        IObstacle ObstacleFactory.CreateObstacle()
        {
            return new ConcreteObstacle_Asteroid();
        }
    }
}
