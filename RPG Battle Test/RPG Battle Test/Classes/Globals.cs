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
        /// None represents neutral damage, which is not reduced by Defense or Magic Defense.
        /// None uses the higher value of the attacker's total Attack or MagicAtk
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
            Neutral, Fire, Earth, Ice, Poison
        }

        /// <summary>
        /// The amount damage is modified by when an entity resists a particular element
        /// </summary>
        public const float RESISTANCE_MOD = .5f;

        /// <summary>
        /// The amount damage is modified by when an entity is weak to a particular element
        /// </summary>
        public const float WEAKNESS_MOD = 2f;

        /// <summary>
        /// The minimum allowed damage
        /// </summary>
        public const int MIN_DMG = 0;

        /// <summary>
        /// The maximum allowed damage
        /// </summary>
        public const int MAX_DMG = 99999;

        public static readonly Random Randomizer = new Random();
    }
}
