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
        protected double safeTime;
        double timeToLive = 15000;
        double check = 0;

        public ConcreteObstacle_Bullet(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 10, 0, 1, 1, 0, 0, 32, 500, GraphicBank.getInstance().GetGraphic("bullet"))
        {
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
            safeTime = 200;
        }

        public override bool readyToCollide(GameTime gameTime)
        {
            if (safeTime <= 0)
            {
                return true;
            }
            return false;
        }

        public bool GetLifetimeExpired(double millis)
        {
            check += millis;
            timeToLive -= millis;
            if (timeToLive <= 0)
            {
                return true;
            }
            return false;
        }


        public void updateSafeTime(GameTime gameTime)
        {
            if (safeTime > 0)
            {
                safeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }

    class ConcreteObstacle_Asteroid : Unit, IObstacle
    {
        public ConcreteObstacle_Asteroid(Vector2 velocity, Vector2 position) : base(position, 0, velocity, Vector2.Zero, 20000, 0, 10000, 10000, 10, 10, 0, 0, GraphicBank.getInstance().GetGraphic("astroid"))
        {
          
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
        }

        /// <summary>
        /// Asteroids are supposed to pass through the level, and will die once they've
        /// been outside for a while. Don't bother timing them.
        /// </summary>
        /// <returns></returns>
        public bool GetLifetimeExpired(double millis)
        {
            return false;
        }


        public void updateSafeTime(GameTime gameTime)
        {
            //No safe time for asteriods
        }
    }

    class ConcreteObstacle_Laser : Unit, IObstacle
    {
        double safeTime;
        double timeToLive = 8000;

        public ConcreteObstacle_Laser(Vector2 velocity, Vector2 position, float laserRot)
            : base(position, 0, velocity, Vector2.Zero, 10, 0, 1, 1, 0, 0, 18, 200, GraphicBank.getInstance().GetGraphic("laser"))
        {
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
            rotation = laserRot;
            safeTime = 100;
        }

        public override bool readyToCollide(GameTime gameTime)
        {
            if (safeTime <= 0)
            {
                return true;
            }
            return false;
        }

        public bool GetLifetimeExpired(double millis)
        {
            timeToLive -= millis;
            if (timeToLive < 0)
            {
                return true;
            }
            return false;
        }


        public void updateSafeTime(GameTime gameTime)
        {
            if (safeTime > 0)
            {
                safeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }

}
