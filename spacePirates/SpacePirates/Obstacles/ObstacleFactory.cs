using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.Obstacles
{
    interface ObstacleFactory
    {
        IObstacle CreateObstacle(Vector2 velocity, Vector2 position);
    }
}
