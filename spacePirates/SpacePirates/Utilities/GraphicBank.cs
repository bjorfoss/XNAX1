using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace SpacePirates.Utilities
{
    class GraphicBank
    {
        private static GraphicBank instance;
        static readonly object padlock = new Object();
        private ContentManager content;

        private Dictionary<String, Texture2D> graphics;
        private Dictionary<String, SpriteFont> fonts;
        private Dictionary<String, SoundEffect> sounds;

        private GraphicBank()
        {
            content = GameObject.GetContentManager();
            graphics = new Dictionary<String, Texture2D>();
            graphics.Add("bullet", content.Load<Texture2D>("Graphics/Obstacles/Projectile01"));
            graphics.Add("astroid", content.Load<Texture2D>("Graphics/Obstacles/astroid"));
            graphics.Add("fighter", content.Load<Texture2D>("Graphics/Ships/NFighterSheeth"));
            graphics.Add("station", content.Load<Texture2D>("Graphics/Obstacles/SpaceStation"));
            graphics.Add("dockMenu", content.Load<Texture2D>("MenuButtons/DockMenu"));
            graphics.Add("eightwing", content.Load<Texture2D>("Graphics/Ships/Eightwing"));
            graphics.Add("explosion", content.Load<Texture2D>("Graphics/explosion01"));

            fonts = new Dictionary<String, SpriteFont>();
            fonts.Add("Menutext", content.Load<SpriteFont>("Graphics/SpriteFonts/Menutext"));

            sounds = new Dictionary<string, SoundEffect>();
            sounds.Add("boom1", content.Load<SoundEffect>("Sound/Effects/boom1"));
            sounds.Add("boom2", content.Load<SoundEffect>("Sound/Effects/boom2"));
            sounds.Add("boom3", content.Load<SoundEffect>("Sound/Effects/boom3"));
            sounds.Add("boom4", content.Load<SoundEffect>("Sound/Effects/boom4"));
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

        public SoundEffect getSound(String name)
        {
            return sounds[name];
        }
    }
}
