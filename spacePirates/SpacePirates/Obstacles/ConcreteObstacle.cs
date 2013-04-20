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
        double safeTime;

        public ConcreteObstacle_Bullet(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 10, 0, 1, 1, 0, 0, 32, 500, GraphicBank.getInstance().getGraphic("bullet"))
        {
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
            safeTime = 0;
        }

        public override bool readyToCollide(GameTime gameTime)
        {
            double temp = gameTime.ElapsedGameTime.TotalMilliseconds;
            if (safeTime + temp > 100)
            {
                return true;
            }
            else
            {
                safeTime += temp;
                return false;
            }
        }
    }

    class ConcreteObstacle_Asteroid : Unit, IObstacle
    {
        public ConcreteObstacle_Asteroid(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 200, 0, 10000, 10000, 10, 10, 0, 0, GraphicBank.getInstance().getGraphic("astroid"))
        {
          
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
        }
    }

    class ConcreteObstacle_Laser : Unit, IObstacle
    {
        double safeTime;

        public ConcreteObstacle_Laser(Vector2 velocity, Vector2 position, float laserRot)
            : base(position, 0, velocity, Vector2.Zero, 10, 0, 1, 1, 0, 0, 18, 900, GraphicBank.getInstance().getGraphic("laser"))
        {
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
            rotation = laserRot;
            safeTime = 0;
        }

        public override bool readyToCollide(GameTime gameTime)
        {
            double temp = gameTime.ElapsedGameTime.TotalMilliseconds;
            if (safeTime + temp > 100)
            {
                return true;
            }
            else
            {
                safeTime += temp;
                return false;
            }
        }
    }

}
