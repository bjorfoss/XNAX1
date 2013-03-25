using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Obstacles
{
    interface ObstacleFactory
    {
        Obstacle CreateObstacle();
    }
}
