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
    class GraphicBank : Microsoft.Xna.Framework.Game
    {
        private static GraphicBank instance;
        static readonly object padlock = new Object();
        private ContentManager content;

        private Dictionary<String, Texture2D> graphics;
        private Dictionary<String, SpriteFont> fonts;
        private Dictionary<String, SoundEffect> sounds;

        private bool loadedSounds;
        private bool loadedGraphics;
        private bool loadedFonts;
        private int tries;

        private GraphicBank()
        {
            content = GameObject.GetContentManager();

            graphics = new Dictionary<String, Texture2D>();
            fonts = new Dictionary<String, SpriteFont>();
            sounds = new Dictionary<String, SoundEffect>();

            if (!LoadSounds()) 
            {
                
                Environment.FailFast("Failed to Load Sounds");
            }
            LoadGraphics();
            LoadFonts();
            tries = 0;

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
            if (!loadedFonts)
            {
                
                LoadFonts();
                
            }
            return fonts[name];
        }

        public Texture2D GetGraphic(String name)
        {
            if (!loadedGraphics)
            {
                LoadGraphics();
            }
            return graphics[name];
        }

        public SoundEffect GetSound(String name)
        {
            if (!loadedSounds)
            {
                
                LoadSounds();

            }
            return sounds[name];
        }

        private void LoadFonts(){
            
            fonts.Add("Menutext", content.Load<SpriteFont>("Graphics/SpriteFonts/Menutext"));
            fonts.Add("Weapon", content.Load<SpriteFont>("Graphics/SpriteFonts/Weapon"));
            fonts.Add("Utility", content.Load<SpriteFont>("Graphics/SpriteFonts/Utility"));
            fonts.Add("Armor", content.Load<SpriteFont>("Graphics/SpriteFonts/Armor"));
            fonts.Add("Health", content.Load<SpriteFont>("Graphics/SpriteFonts/Health"));
            fonts.Add("KillCount", content.Load<SpriteFont>("Graphics/SpriteFonts/KillCount"));
            fonts.Add("TeamScore", content.Load<SpriteFont>("Graphics/SpriteFonts/TeamScore"));

            loadedFonts = true;
        }

        private bool LoadSounds(){

            Console.WriteLine("Try nr " + tries);
            try
            {

                sounds.Add("boom1", content.Load<SoundEffect>("Sound/Effects/boom1"));
                sounds.Add("boom2", content.Load<SoundEffect>("Sound/Effects/boom2"));
                sounds.Add("boom3", content.Load<SoundEffect>("Sound/Effects/boom3"));
                sounds.Add("boom4", content.Load<SoundEffect>("Sound/Effects/boom4"));
                loadedSounds = true;
                return true;
            }
            catch (InvalidOperationException e)
            {
                if (tries < 40)
                {

                    tries++;
                    return LoadSounds();

                }
                else
                {
                    return false;
                }
                
            }

            
           
        }

        private void LoadGraphics(){
            
            graphics.Add("bullet", content.Load<Texture2D>("Graphics/Obstacles/Projectile01"));
            graphics.Add("laser", content.Load<Texture2D>("Graphics/Obstacles/LaserProjectile1"));
            graphics.Add("astroid", content.Load<Texture2D>("Graphics/Obstacles/astroid"));
            graphics.Add("fighter", content.Load<Texture2D>("Graphics/Ships/NFighterSheeth"));
            graphics.Add("station", content.Load<Texture2D>("Graphics/Obstacles/SpaceStation"));
            graphics.Add("dockMenu", content.Load<Texture2D>("MenuButtons/DockMenu"));
            graphics.Add("eightwing", content.Load<Texture2D>("Graphics/Ships/Eightwing"));
            graphics.Add("explosion", content.Load<Texture2D>("Graphics/explosion01"));



            Texture2D texture = new Texture2D(GameObject.Instance().graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });
            graphics.Add("box", texture);

            loadedGraphics = true;

        }

    }
}
