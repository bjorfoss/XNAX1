using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.Obstacles
{
    public interface IObstacleFactory
    {
        IObstacle CreateObstacle(Vector2 velocity, Vector2 position, float rotation=0.0f);
    }
}
