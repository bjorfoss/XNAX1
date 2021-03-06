﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using SpacePirates.spaceShips;
using SpacePirates.Obstacles;
using SpacePirates.Player;
using SpacePirates.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpacePirates.Player
{
    class Hud
    {
        private Vector2 weaponPos;
        private Vector2 utilityPos;

        private Rectangle maxHealth;
        private Rectangle currentHealth;
        private Rectangle maxArmor;
        private Rectangle currentArmor;
        
        public IPlayer playerObject;

        public Hud(IPlayer player)
        {
            Console.WriteLine("Sets playerObject: " + player);
            playerObject = player;
            
        }

        public void executeDraw(SpriteBatch batch, Rectangle screenArea)
        {
            Unit UnitObject = playerObject.GetShip() as Unit;

            int _currentHealth = Convert.ToInt32((UnitObject.getHealth() / UnitObject.getMaxHealth()) * 200);

            currentHealth = new Rectangle(100, 5, _currentHealth, 20);
            maxHealth = new Rectangle(100, 5, 200, 20);

            int _currentAmor = Convert.ToInt32(UnitObject.getArmorEffectiveness()*2);

            currentArmor = new Rectangle(100, 30, _currentAmor, 20);
            maxArmor = new Rectangle(100, 30, 200, 20);

            GraphicBank.getInstance().GetFont("Weapon");
            GraphicBank.getInstance().GetFont("Utility");
            GraphicBank.getInstance().GetFont("Health");
            GraphicBank.getInstance().GetFont("TeamScore");
            GraphicBank.getInstance().GetFont("Armor");
            GraphicBank.getInstance().GetFont("KillCount");

            batch.Draw(GraphicBank.getInstance().GetGraphic("box"), maxHealth, Color.DarkRed);
            batch.Draw(GraphicBank.getInstance().GetGraphic("box"), currentHealth, Color.Red);

            batch.Draw(GraphicBank.getInstance().GetGraphic("box"), maxArmor, Color.DarkBlue);
            batch.Draw(GraphicBank.getInstance().GetGraphic("box"), currentArmor, Color.Blue);

            batch.DrawString(GraphicBank.getInstance().GetFont("Health"), "Health:", Vector2.Zero, Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), "Armor:", new Vector2(0.0f, 30.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            SpaceShip ship = (playerObject.GetShip() as SpaceShip);
            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), "Weapon:  " + ship.GetSelWeaponName(), new Vector2(0.0f, 60.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), "Weapon:  " + ship.GetSelWeaponName(), new Vector2(0.0f, 60.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            String abilityString = "Ability: " + ship.GetSelAbilityName();
            if(ship.getAbilityActive()){ abilityString += " - " + Extensions.round(ship.getAbilityTimer()/1000, 1); }
            else { abilityString += " - " + Extensions.round(ship.getAbilityTimer()/1000, 1); }
            
            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), abilityString, new Vector2(0.0f, 90.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            batch.DrawString(GraphicBank.getInstance().GetFont("Health"), "Currency: " + playerObject.GetCurrency(), new Vector2(0.0f, 120.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
            
            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), "Red: " + GameObject.Instance().getRedScore().ToString() + " -- Blue: " + GameObject.Instance().getBlueScore().ToString(), new Vector2(0.0f, 150.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);

            batch.DrawString(GraphicBank.getInstance().GetFont("Armor"), "Deaths: " + (playerObject as Human).GetTimesDied(), new Vector2(0.0f, 180.0f), Color.LightGreen, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
        }

        
    }
}
