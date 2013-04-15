using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates.Utilities
{
    class GraphicBank
    {
        private static GraphicBank instance;
        static readonly object padlock = new Object();

        private Dictionary<String, Texture2D> graphics;
        Texture2D bullet;
        Texture2D fighter;

        private GraphicBank()
        {
            graphics = new Dictionary<String, Texture2D>();
            graphics.Add("bullet", GameObject.GetContentManager().Load<Texture2D>("Graphics/Obstacles/Projectile01"));
            graphics.Add("fighter", GameObject.GetContentManager().Load<Texture2D>("Graphics/Ships/NFighterSheeth"));

        }

        public static GraphicBank getInstance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GraphicBank();
                }
                return instance;
            }
        }

        public Texture2D getGraphic(String name)
        {
            return graphics[name];
        }
    }
}
