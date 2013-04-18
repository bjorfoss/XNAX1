using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpacePirates.Utilities
{
    class GraphicBank
    {
        private static GraphicBank instance;
        static readonly object padlock = new Object();
        private ContentManager content;

        private Dictionary<String, Texture2D> graphics;
        private Dictionary<String, SpriteFont> fonts;
        Texture2D bullet;
        Texture2D fighter;

        private GraphicBank()
        {
            content = GameObject.GetContentManager();
            graphics = new Dictionary<String, Texture2D>();
            graphics.Add("bullet", content.Load<Texture2D>("Graphics/Obstacles/Projectile01"));
            graphics.Add("fighter", content.Load<Texture2D>("Graphics/Ships/NFighterSheeth"));
            graphics.Add("station", content.Load<Texture2D>("Graphics/Obstacles/SpaceStation"));
            graphics.Add("explosion", content.Load<Texture2D>("Graphics/explosion01"));

            fonts = new Dictionary<String, SpriteFont>();
            fonts.Add("Menutext", content.Load<SpriteFont>("Graphics/SpriteFonts/Menutext"));
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

        public SpriteFont GetFont(String name)
        {
            return fonts[name];
        }

        public Texture2D getGraphic(String name)
        {
            return graphics[name];
        }
    }
}
