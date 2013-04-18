using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Utilities
{
    static class Extensions
    {
        public static List<Explosion> CloneExplosions(List<Explosion> listToClone)
        {
            List<Explosion> ts = new List<Explosion>();
            foreach (Explosion t in listToClone)
            {
                ts.Add(t);
            }
            return ts;
        }
    }

}
