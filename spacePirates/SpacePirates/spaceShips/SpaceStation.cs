using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Utilities;

namespace SpacePirates.spaceShips
{
    class SpaceStation : Unit
    {

        private String team;
        private List<ISpaceShip> dockedShips;



        public SpaceStation(Vector2 position)
            : base(position, 0, Vector2.Zero, new Vector2(0), 20000, 0, 10000, 10000, 50, 100, 30, 40, GraphicBank.getInstance().getGraphic("fighter"))
        
        {

            
            
            
         

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        public void setTeam(String team)
        {
            this.team = team;

        }

        /// <summary>
        /// dock a spaceship
        /// </summary>
        /// <param name="ship"></param>
        public void dockShip(ISpaceShip ship)
        {
            dockedShips.Add(ship);
        }

        /// <summary>
        /// repairs a ship
        /// TODO : actually repair it
        /// TODO : buy repair from the shop? 
        /// </summary>
        /// <param name="ship"></param>
        public void repairShip(Unit ship)
        {
            double maxhealth = ship.getMaxHealth();

        }

        /// <summary>
        /// open shop interface
        /// TODO : need list of upgrades available for ship
        /// TODO : make shop interface in a different class ?
        /// </summary>
        public void shop()
        {

        }

        /// <summary>
        /// respawn a ship after it has been destroyed
        /// implement cost to respawn?? 
        /// </summary>
        /// <param name="ship"></param>
        public void respawnShip(Unit ship)
        {

        }

        /// <summary>
        /// make 
        /// </summary>
        /// <param name="ship"></param>
        public void spawnShip(ISpaceShip ship)
        {

        }

        
    }
}
