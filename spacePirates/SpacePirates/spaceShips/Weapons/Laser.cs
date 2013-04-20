using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Obstacles;

namespace SpacePirates.spaceShips.Weapons
{
    class Laser : IWeapon
    {
        double fireRate; //The time, in milliseconds, from a shot is fired until another shot can be fired.
        double time; //The time, in milliseconds, when the previous shot was fired.
        string name; //The name of the weapon.

        public Laser()
        {
            fireRate = 700;
            time = 0;
            name = "Laser";
        }

        public string GetName()
        {
            return name;
        }


        public void Fire(GameTime gameTime, Unit ship)
        {
            //Records the current time in milliseconds, then checks it against when the last shot was fired.
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time >= fireRate)
            {
                System.Diagnostics.Debug.WriteLine(ship.rotation);
                double speed = 150;
                double offset = 20;
                Vector2 vel = new Vector2((float)(Math.Sin(ship.rotation) * speed), (float)(Math.Cos(ship.rotation) * speed));
                double edge = Math.Sqrt(Math.Pow(ship.getUnitRectangle().Width / 2, 2) + Math.Pow(ship.getUnitRectangle().Height / 2, 2));
                float var = (float)Math.Sqrt(Math.Pow(((ship.getUnitRectangle().Height / 2) + offset), 2) /
                    (Math.Pow(vel.X, 2) + Math.Pow(vel.Y, 2)));
                Vector2 pos = new Vector2(ship.GetPosition().X + var * vel.X, ship.GetPosition().Y + var * vel.Y);
                //vel.X += ship.getVelocity().X;
                //vel.Y += ship.getVelocity().Y;

                ConcreteObstacle_Laser laser = (ConcreteObstacleFactory.CreateObstacle("laser", pos, vel, (float)ship.rotation)) as ConcreteObstacle_Laser;

                GameObject.Instance().addToGame(laser);

                time = 0;
            }
        }
        
    }
}
