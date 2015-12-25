using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public static class Globals
    {
        /// <summary>
        /// Enumeration of different damage types. 
        /// None represents neutral damage, which is not reduced by Defense or Magic Defense
        /// </summary>
        public enum DamageTypes
        {
            None, Physical, Magic
        }

        /// <summary>
        /// Enumeration of different element types. 
        /// </summary>
        public enum Elements
        {
            Neutral, Fire, Earth, Ice, Poison, DefPierce, MDefPierce
        }

        /// <summary>
        /// The amount damage is modified by when an entity resists a particular element
        /// </summary>
        public const float ResistanceModifier = .5f;

        /// <summary>
        /// The amount damage is modified by when an entity is weak to a particular element
        /// </summary>
        public const float WeaknessModifier = 2f;

        /// <summary>
        /// The minimum allowed damage
        /// </summary>
        public const int MinDamage = 0;

        /// <summary>
        /// The maximum allowed damage
        /// </summary>
        public const int MaxDamage = 99999;
    }
}
