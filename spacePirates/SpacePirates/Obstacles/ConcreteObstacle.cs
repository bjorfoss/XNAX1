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
        public ConcreteObstacle_Bullet(Vector2 velocity, Vector2 position)
        {
            acceleration = Vector2.Zero;
            this.velocity = velocity;
            this.position = position;

            rotation = 0;
            rotationSpeed = 0;

            mass = 10;
            maxHealth = 1;
            health = maxHealth;

            armorEffectiveness = 0;
            armorThreshold = 0;

            blastRadius = 1;
            blastDamage = 100;

            graphics = GraphicBank.getInstance().getBulletGraphic();
            animationFrame = new Rectangle(0, 0, graphics.Width, graphics.Height);
        }
    }
}
