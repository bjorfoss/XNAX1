using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePirates.spaceShips
{
    public interface IAbility
    {
        void Activate();

        void Update();

        string GetName();

        string GetType();
    }
}
