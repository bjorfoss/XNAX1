using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePirates.Utilities;
using SpacePirates.spaceShips;

namespace SpacePirates
{
    class Explosion
    {
        Texture2D graphic;
        Vector2 position;
        Vector2 size;
        double damage;
        double timeToLive;

        public Explosion(Vector2 position, Vector2 size, double damage)
        {
            graphic = GraphicBank.getInstance().getGraphic("explosion");
            this.position = position;
            this.size = size;
            this.damage = damage;
            timeToLive = 1000;
        }

        public Rectangle getExplosionRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public double getDamage()
        {
            return damage;
        }

        public bool update(GameTime gameTime)
        {
            bool result = timeToLive == 1000;
            timeToLive -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeToLive <= 0)
            {
                GameObject.Instance().removeFromGame(this);
            }
            return result;
        }

        public void Draw(SpriteBatch batch)
        {
            Rectangle animationFrame = new Rectangle(0, 0, 32, 32);
            batch.Draw(graphic, Unit.WorldPosToScreenPos(position), animationFrame, Color.White, 0,
                    new Vector2(animationFrame.Width / 2, animationFrame.Height / 2), 
                    size.X/graphic.Width, SpriteEffects.None, 0f);
        }

        public void playSound()
        {
            Vector2 pos = Unit.WorldPosToScreenPos(position);
            Rectangle screen = GameObject.GetScreenArea();
            if (pos.X > -screen.Width / 2 && pos.X < screen.Width + screen.Width / 2
                && pos.Y > -screen.Height / 2 && pos.Y < screen.Height + screen.Height / 2)
            {
                GraphicBank.getInstance().getSound("boom"+((new Random()).Next(4)+1)).Play();
            }
        }
    }
}
