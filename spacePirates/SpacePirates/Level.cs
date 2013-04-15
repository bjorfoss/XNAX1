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
        private Vector2 size;
        private Vector2 position;
        private Rectangle croppedBackground; //the visible part

        public Level(ContentManager Content)
        {
            Level self = this;

            self.background = Content.Load<Texture2D>("background");
            size = new Vector2(background.Width, background.Height);
            position = new Vector2(0, 0);
        }

        public Vector2 GetLevelSize()
        {
            return size;
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
