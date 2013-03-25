using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Obstacle
{
    interface ObstacleFactory
    {
        IObstacle CreateObstacle();
    }
}
