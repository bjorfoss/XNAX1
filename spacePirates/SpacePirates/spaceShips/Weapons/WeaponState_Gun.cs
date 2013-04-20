using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Obstacles;

namespace SpacePirates.spaceShips
{
    class WeaponState_Gun : IWeapon
    {
        double fireRate; //The time, in milliseconds, from a shot is fired until another shot can be fired.
        double time; //The time, in milliseconds, when the previous shot was fired.
        string name; //The name of the weapon.

        public WeaponState_Gun()
        {
            fireRate = 500;
            time = 0;
            name = "Standard Gun";
        }

        public string GetName()
        {
            return name;
        }


        public void Fire(GameTime gameTime, Unit ship)
        {
            //Records the current time in milliseconds, then checks it against when the last shot was fired.
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(time >= fireRate)
            {
                System.Diagnostics.Debug.WriteLine(ship.rotation);

                Vector2 vel = new Vector2((float)(Math.Sin(ship.rotation) * 300), (float)(Math.Cos(ship.rotation) * 300));
                double edge = Math.Sqrt(Math.Pow(ship.getUnitRectangle().Width / 2, 2) + Math.Pow(ship.getUnitRectangle().Height / 2, 2));
                float var = (float)Math.Sqrt(Math.Pow(((ship.getUnitRectangle().Height / 2) + 8), 2) / 
                    (Math.Pow(vel.X, 2) + Math.Pow(vel.Y, 2)));
                Vector2 pos = new Vector2(ship.GetPosition().X + var * vel.X, ship.GetPosition().Y + var * vel.Y);
                vel.X += ship.getVelocity().X;
                vel.Y += ship.getVelocity().Y;

                IObstacle  bullet = ConcreteObstacleFactory.CreateObstacle("bullet", pos, vel);

                GameObject.Instance().addToGame(bullet);

                time = 0;
            }
        }
    }
}
