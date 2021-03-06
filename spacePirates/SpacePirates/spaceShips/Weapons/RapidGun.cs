﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Obstacles;
using SpacePirates.Utilities;

namespace SpacePirates.spaceShips.Weapons
{
    class RapidGun : IWeapon
    {
        double fireRate; //The time, in milliseconds, from a shot is fired until another shot can be fired.
        double time; //The time, in milliseconds, when the previous shot was fired.
        string name; //The name of the weapon.

        public RapidGun()
        {
            fireRate = 200;
            time = 0;
            name = "Rapid Gun";
        }

        public string GetName()
        {
            return name;
        }
        public string GetTypeOf()
        {
            return "rapidgun";
        }


        public void Fire(GameTime gameTime, Unit ship)
        {
            //Records the current time in milliseconds, then checks it against when the last shot was fired.
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(time >= fireRate)
            {
                System.Diagnostics.Debug.WriteLine(ship.rotation);
                double speed = 250;
                double offset = 30;
                Vector2 vel = new Vector2((float)(Math.Sin(ship.rotation) * speed), (float)(Math.Cos(ship.rotation) * speed));
                double edge = Math.Sqrt(Math.Pow(ship.getUnitRectangle().Width / 2, 2) + Math.Pow(ship.getUnitRectangle().Height / 2, 2));
                float var = (float)Math.Sqrt(Math.Pow(((ship.getUnitRectangle().Height / 2) + offset), 2) / 
                    (Math.Pow(vel.X, 2) + Math.Pow(vel.Y, 2)));
                Vector2 pos = new Vector2(ship.GetPosition().X + var * vel.X, ship.GetPosition().Y + var * vel.Y);
                vel.X += ship.getVelocity().X;
                vel.Y += ship.getVelocity().Y;


                if (ship is ConcreteShip_Eightwing)
                {
                    Vector2 pos1;
                    Vector2 pos2;

                    int divide = 3;
                    offset = 50;

                    Vector2 var1 = new Vector2((float)(Math.Sin(ship.rotation + (Math.PI / divide)) * 1), (float)(Math.Cos(ship.rotation + (Math.PI / divide)) * 1));
                    Vector2 var2 = new Vector2((float)(Math.Sin(ship.rotation - (Math.PI / divide)) * 1), (float)(Math.Cos(ship.rotation - (Math.PI / divide)) * 1));

                    float s1 = (float)(Math.Sqrt((Math.Pow(offset, 2) + Math.Pow(44, 2)) / (Math.Pow(var1.X, 2) + Math.Pow(var1.Y, 2))));
                    float s2 = (float)(Math.Sqrt((Math.Pow(offset, 2) + Math.Pow(44, 2)) / (Math.Pow(var2.X, 2) + Math.Pow(var2.Y, 2))));

                    pos1 = new Vector2(ship.GetPosition().X + (var1.X * s1), ship.GetPosition().Y + (var1.Y * s1));
                    pos2 = new Vector2(ship.GetPosition().X + (var2.X * s2), ship.GetPosition().Y + (var2.Y * s2));

                    IObstacle laser = ConcreteObstacleFactory.CreateObstacle("bullet", pos1, vel, (float)ship.rotation);
                    IObstacle laser2 = ConcreteObstacleFactory.CreateObstacle("bullet", pos2, vel, (float)ship.rotation);

                    GameObject.Instance().addToGame(laser);
                    GameObject.Instance().addToGame(laser2);

                }
                else
                {
                    IObstacle bullet = ConcreteObstacleFactory.CreateObstacle("rapidGunBullet", pos, vel);

                    GameObject.Instance().addToGame(bullet);
                }
                time = 0;
            }
        }
    }

    class RapidGunBullet : ConcreteObstacle_Bullet, IObstacle
    {
        public RapidGunBullet(Vector2 velocity, Vector2 position) : base(velocity, position)
        {
            safeTime = 300;
        }
    }
}
