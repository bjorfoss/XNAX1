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

        public WeaponState_Gun()
        {
            fireRate = 500;
            time = 0;
        }


        public void Fire(GameTime gameTime, Unit ship)
        {
            //Records the current time in milliseconds, then checks it against when the last shot was fired.
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(time >= fireRate)
            {
                System.Diagnostics.Debug.WriteLine(ship.rotation);

                Vector2 vel = new Vector2((float)(Math.Sin(ship.rotation) * 300), (float)(Math.Cos(ship.rotation) * 300));
                System.Diagnostics.Debug.WriteLine(vel.X + ", " + vel.Y);
                vel.X += ship.getVelocity().X;
                vel.Y += ship.getVelocity().Y;
                System.Diagnostics.Debug.WriteLine(ship.getVelocity().X + ", " + ship.getVelocity().Y);
                System.Diagnostics.Debug.WriteLine(vel.X + ", " + vel.Y);
                Vector2 pos = new Vector2(ship.GetPosition().X, ship.GetPosition().Y);

                Factory_Bullet factory = GameObject.Instance().getOFactory("bullet") as Factory_Bullet;
                ConcreteObstacle_Bullet bullet = factory.CreateObstacle(vel, pos) as ConcreteObstacle_Bullet;

                GameObject.Instance().addToGame(bullet);

                time = 0;
            }
        }
    }
}
