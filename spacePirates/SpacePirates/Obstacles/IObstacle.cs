using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Obstacles
{
    public interface IObstacle
    {
        /// <summary>
        /// Check if obstacle should die from time expiry
        /// <param name="updateMillis">Milliseconds since last update</param>
        /// </summary>
        bool GetLifetimeExpired(double updateMillis);
    }
}
