using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Utilities;

namespace SpacePirates.Obstacles
{
    class ConcreteObstacle_Bullet : Unit, IObstacle
    {
        public ConcreteObstacle_Bullet(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 0, 0, 0, 0, 0, 0, 0, 0, GraphicBank.getInstance().getGraphic("bullet"))
        {

            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
        }
    }

    class ConcreteObstacle_Asteroid : Unit, IObstacle
    {
        public ConcreteObstacle_Asteroid(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 0, 0, 0, 0, 0, 0, 0, 0, GraphicBank.getInstance().getGraphic("asteroid"))
        {
          
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
        }
    }

}
