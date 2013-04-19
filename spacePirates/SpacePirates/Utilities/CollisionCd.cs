using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacePirates.Utilities
{
    public class CollisionCd
    {
        Unit unit;
        double cooldown;

        public CollisionCd(Unit unit)
        {
            this.unit = unit;
            cooldown = 50;
        }

        public void update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public bool cdOver()
        {
            if (cooldown <= 0) { return true; }
            else { return false; }
        }

        public Unit getUnit()
        {
            return unit;
        }
    }
}
