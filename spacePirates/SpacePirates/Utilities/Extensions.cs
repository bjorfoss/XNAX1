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

        public static double round(double value, int places)
        {
            if (places < 0) throw new ArgumentOutOfRangeException();

            long factor = (long)Math.Pow(10, places);
            value = value * factor;
            long tmp = (long)Math.Round(value);
            return (double)tmp / factor;
        }
    }

}
