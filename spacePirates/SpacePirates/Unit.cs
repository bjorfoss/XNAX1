using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePirates
{
    class Unit
    {
        //todo make protected
        Vector2 velocity;
        Vector2 acceleration;
        double mass;
        Vector2 position;
        double rotation;
        double rotationSpeed;

        Texture2D graphics;
        
        double health;
        double maxHealth;

        double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
        double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it
        
        double blastRadius;
        double blastDamage;
        //Rectangle hitbox;

        //add getters and setters

        void CalculateDirectionAndSpeed() {}

        void UpdatePosition() { }

        /// <summary>
        /// Calculate own collision damage, check if colliding with an explosion
        /// Call OnDestroy/OnDeath and do blast damage (if applicable)
        /// </summary>
        /// <param name="unit"></param>
        void HandleCollision(Unit unit) { }

        void OnDestroy() { }


    }
}
