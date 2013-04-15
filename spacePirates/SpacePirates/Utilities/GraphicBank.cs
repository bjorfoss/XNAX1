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

        Texture2D bullet;

        private GraphicBank()
        {
            GameObject.GetContentManager().Load<Texture2D>("Graphics/Obstacles/Projectile01");
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

        public Texture2D getBulletGraphic()
        {
            return bullet;
        }
    }
}
