using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SpacePirates.spaceShips;

namespace SpacePirates
{
    class Level
    {
        private Texture2D background;
        private HashSet<Rectangle> foreground;
        private Rectangle bounds;
        private Vector2 position;
        private Rectangle croppedBackground; //the visible part

        public Level(ContentManager Content)
        {
            Level self = this;

            self.background = Content.Load<Texture2D>("Graphics/background");
            bounds = new Rectangle(0, 0, 30000, 30000);
            position = new Vector2(0, 0);
        }

        /// <summary>
        /// Get a rectangle representing the entire level
        /// </summary>
        /// <returns></returns>
        public Rectangle GetLevelBounds()
        {
            return bounds;
        }

        /// <summary>
        /// Check if a unit is outside of the level
        /// </summary>
        /// <param name="unit">The unit to check</param>
        /// <returns>true if the unit is outside, otherwise false</returns>
        public bool IsOutsideLevel(Unit unit)
        {
            Vector2 position = unit.GetPosition();
            //Don't bother getting an accurate size for the unit rectangle,
            //the level check doesn't require that much accuracy
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, 10, 10);
            //The level rectangle intersects with the unit's rectangle
            if (bounds.Intersects(rect)) {
                return false;
            }
            return true;
        }

        private void CalculateLevelCrop(SpriteBatch sb)
        {
            //draw a section of the background as large as the window
            croppedBackground = GameObject.GetScreenArea();

            //draw the world relative to the camera target
            ISpaceShip target = GameObject.GetCameraTarget();
            Vector2 targetPos = (target as Unit).GetPosition();
            croppedBackground.X = (int) (((float) croppedBackground.Width / 2.0f) + targetPos.X);
            croppedBackground.Y = (int) (((float) croppedBackground.Height / 2.0f) - targetPos.Y);
        }

        public void executeDraw(SpriteBatch spriteBatch)
        {
            CalculateLevelCrop(spriteBatch);
            spriteBatch.Draw(background, position, croppedBackground, Color.White, 0.0f, position, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
