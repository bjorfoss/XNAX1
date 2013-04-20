using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SpacePirates.Utilities;

namespace SpacePirates.spaceShips
{
    class SpaceStation
    {

        private String team;
        private List<ISpaceShip> dockedShips;
        private Vector2 position;
        protected Texture2D graphics;
        protected Color spacestationColor;
        //protected Rectangle animationFrame;
        protected double rotation;
        protected double rotationSpeed;

        protected Texture2D dockMenu;


        public SpaceStation(Vector2 position, Color stationColor)
            //: base(position, 0, Vector2.Zero, new Vector2(0), 100000, MathHelper.Pi/16, 10000, 10000, 50, 100, 30, 40, GraphicBank.getInstance().getGraphic("station"))
        
        {
            dockMenu = GraphicBank.getInstance().getGraphic("dockMenu");
            this.position = position;
            graphics = GraphicBank.getInstance().getGraphic("station");
            rotation = 0;
            rotationSpeed = MathHelper.Pi / 16;

            spacestationColor = stationColor;

            dockedShips = new List<ISpaceShip>();

            //animationFrame = new Rectangle(0, 0, 256, 256);
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

            if (!dockedShips.Contains(ship))
            {

                dockedShips.Add(ship);
            }



        }

        /// <summary>
        /// repairs a ship
        /// TODO : actually repair it
        /// TODO : buy repair from the shop? 
        /// </summary>
        /// <param name="ship"></param>
        public void repairShip(ISpaceShip ship)
        {
            double maxhealth = (ship as Unit).getMaxHealth();

        }

        /// <summary>
        /// open shop interface
        /// TODO : need list of upgrades available for ship
        /// TODO : make shop interface in a different class ?
        /// </summary>
        /// <param name="ship">the ship that wishes to shop</param>
        public void shop(ISpaceShip ship)
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

        /// <summary>
        /// lock ship to station when docked?
        /// </summary>
        public void undock()
        {

        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(position.X - (double)graphics.Bounds.Width / 2),
                (int)(position.Y - (double)graphics.Bounds.Height / 2), graphics.Bounds.Width, graphics.Bounds.Height);
        }

        public void Update(GameTime gameTime)
        {

            dockedShips.Clear();
            //handle rotation
            double newRotation = rotation;
            newRotation += (rotationSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (newRotation < 0)
            {
                rotation = MathHelper.TwoPi + newRotation;
            }
            else if (newRotation >= MathHelper.TwoPi)
            {
                rotation = MathHelper.TwoPi - newRotation;
            }
            else
            {
                rotation = newRotation;
            }
        }

        public static Vector2 WorldPosToScreenPos(Vector2 position)
        {
            Rectangle screen = GameObject.GetScreenArea();
            Vector2 cameraPos = (GameObject.GetCameraTarget() as Unit).GetPosition();
            Vector2 screenPos = new Vector2(position.X, position.Y);
            screenPos.X -= cameraPos.X;
            screenPos.X += (float)screen.Width / 2;

            screenPos.Y -= cameraPos.Y;
            screenPos.Y += (float)screen.Height / 2;
            screenPos.Y = screen.Height - screenPos.Y;

            return screenPos;
        }

        public static Vector2 BottomLeftScreenPos(Rectangle menuBounds)
        {

            Rectangle screen = GameObject.GetScreenArea();
            Vector2 pos = new Vector2(menuBounds.Width/2, (float)(screen.Bottom - (menuBounds.Height/2)));
            return pos;

        }

        public void Draw(SpriteBatch batch)
        {

            Vector2 screenPos = WorldPosToScreenPos(position);
            SpriteFont font = GraphicBank.getInstance().GetFont("Menutext");

            batch.Draw(graphics, screenPos, graphics.Bounds, spacestationColor, (float)rotation,
                  new Vector2(graphics.Bounds.Width / 2, graphics.Bounds.Height / 2),
                  1.0f, SpriteEffects.None, 0f);
            /*/
            if (dockedShips.Count > 0)
            {
                Vector2 menuPos = BottomLeftScreenPos(dockMenu.Bounds);
                batch.Draw(dockMenu, menuPos, dockMenu.Bounds, Color.White, 0f, new Vector2(dockMenu.Bounds.Width / 2, dockMenu.Bounds.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }/*/


            


        }

        
    }
}
