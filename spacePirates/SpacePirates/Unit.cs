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
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected double mass;
        protected Vector2 position;
        protected double rotation;
        protected double rotationSpeed;

        protected Texture2D graphics;

        protected double health;
        protected double maxHealth;

        protected double armorThreshold; //how many hitpoints an attack needs to bypass armor - also reduces armor effectiveness
        protected double armorEffectiveness; //at 100% the full armor threshold is used, otherwise this percentage of it

        protected double blastRadius;
        protected double blastDamage;
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
