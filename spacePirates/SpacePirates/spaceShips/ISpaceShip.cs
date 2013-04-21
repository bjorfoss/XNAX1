using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePirates.Player;
using Microsoft.Xna.Framework.Graphics;


namespace SpacePirates.spaceShips
{
    public interface ISpaceShip
    {
        /// <summary>
        /// Make the ship turn. Implementations should consider the ship's limits.
        /// </summary>
        /// <param name="turnRate">
        /// How fast and in which direction the turn should be executed. 
        /// Give a positive value to turn right, negative to turn left.
        /// Value should be the percentage of maximum turning speed.
        /// </param>
        void Turn(double turnRate);

        /// <summary>
        /// Have the ship's engines provide thrust, accelerating the ship.
        /// Implementation need to keep a maximum thrust in Newtons saved for use
        /// by this method.
        /// </summary>
        /// <param name="thrust">Thrust in percent of maximum thrust, positive values only</param>
        void Thrust(double thrust);

        /// <summary>
        /// Fire the ship's currently selected weapon.
        /// </summary>
        void Fire(GameTime gameTime);

        /// <summary>
        /// Change the currently selected weapon to the next one
        /// </summary>
        void NextWeapon();

        /// <summary>
        /// Change the currently selected weapon to the previous one
        /// </summary>
        void PreviousWeapon();

        /// <summary>
        /// Use the ship's currently selected ability
        /// </summary>
        void UseAbility(GameTime gameTime);

        /// <summary>
        /// Updates cooldowns and durations of abilities
        /// </summary>
        /// <param name="gameTime"></param>
        void updateAbilities(GameTime gameTime);

        void drawAbilities(SpriteBatch batch);

        /// <summary>
        /// Change the currently selected ability to the next one
        /// </summary>
        void NextAbility();

        /// <summary>
        /// Change the currently selected ability to the previous one
        /// </summary>
        void PreviousAbility();

        /// <summary>
        /// Get the owner of the ship. Implementations should use the Ownership class
        /// for this.
        /// </summary>
        /// <returns>The ship's owner. Can be Human or Ai.</returns>
        IPlayer GetOwner();

        /// <summary>
        /// If the ship is docked at a space station
        /// </summary>
        /// <returns></returns>
        bool GetIsDocked();

        /// <summary>
        /// Return name of currently selected weapon
        /// </summary>
        /// <returns></returns>
        string GetSelWeaponName();

        /// <summary>
        /// Return name of currently selected ability
        /// </summary>
        /// <returns></returns>
        string GetSelAbilityName();

        /// <summary>
        /// Get the total number of weapon slots for this ship
        /// </summary>
        /// <returns></returns>
        int GetNumWeaponSlots();

        /// <summary>
        /// Get the total number of ability slots for this ship
        /// </summary>
        /// <returns></returns>
        int GetNumAbilitySlots();
    }
}
