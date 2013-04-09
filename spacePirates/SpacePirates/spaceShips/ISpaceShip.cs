﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;


namespace SpacePirates.spaceShips
{
    public interface ISpaceShip
    {
        void Turn(double turnRate);

        void Thrust(double thrust);

        void Fire();

        void NextWeapon();

        void PreviousWeapon();

        void UseAbility();

        void NextAbility();

        void PreviousAbility();

        IPlayer GetOwner();

    }
}
