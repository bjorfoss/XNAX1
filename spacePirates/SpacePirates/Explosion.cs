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
        double animationTime;
        Rectangle animationFrame;
        Color explosionColor;

        public Explosion(Vector2 position, Vector2 size, double damage, Color explColor)
        {
            graphic = GraphicBank.getInstance().GetGraphic("explosion");
            this.position = position;
            this.size = size;
            this.damage = damage;
            timeToLive = 1000;
            animationFrame = new Rectangle(0, 0, 32, 32);
            animationTime = 0;
            explosionColor = explColor;
        }

        public void SetAlternateExplosion(string explosionPNG)
        {
            graphic = GraphicBank.getInstance().GetGraphic(explosionPNG);
            if(graphic == null)
                graphic = GraphicBank.getInstance().GetGraphic("explosion");
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

            animationTime += gameTime.ElapsedGameTime.Milliseconds;
            if (animationTime >= 84)
            {
                // jumps from line 2 to line 3
                if (animationFrame.X / 32 >= 3 && animationFrame.Y / 32 >= 2)
                {
                    animationFrame.X = 0;
                    animationFrame.Y += 32;
                }
                
                //This is where the animation switches to line 2
                else if (animationFrame.X / 32 >= 3)
                {
                    animationFrame.X = 0;
                    animationFrame.Y += 32;
                }
                //Normal animation
                else
                {
                    animationFrame.X += 32;
                }
                animationTime = 0;

            }
            if (timeToLive <= 0)
            {
                GameObject.Instance().removeFromGame(this);
            }
            return result;
        }

        public void Draw(SpriteBatch batch)
        {
            
            batch.Draw(graphic, Unit.WorldPosToScreenPos(position), animationFrame, explosionColor, 0,
                    new Vector2(animationFrame.Width / 2, animationFrame.Height / 2), 
                    size.X/animationFrame.Width, SpriteEffects.None, 0f);
        }

        public void playSound()
        {
            Vector2 pos = Unit.WorldPosToScreenPos(position);
            Rectangle screen = GameObject.GetScreenArea();
            if (pos.X > -screen.Width / 2 && pos.X < screen.Width + screen.Width / 2
                && pos.Y > -screen.Height / 2 && pos.Y < screen.Height + screen.Height / 2)
            {
                GraphicBank.getInstance().GetSound("boom"+((new Random()).Next(4)+1)).Play();
            }
        }
    }
}
