using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.Player
{
    class Ai : Player
    {
        Ownership ownerLink;

        /// <summary>
        /// Determine general goal/strategy
        /// </summary>
        void MakeGeneralPlan();

        /// <summary>
        /// Make local decisions supporting the strategy
        /// </summary>
        void MakeLocalPlan();
    }
}
