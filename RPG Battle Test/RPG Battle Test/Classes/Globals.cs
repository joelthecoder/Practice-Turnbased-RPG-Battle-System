using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public static class Globals
    {
        #region Enumerations

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

        #endregion

        #region Structs

        /// <summary>
        /// A struct containing information about the BattleEntity and the AffectableBase affecting another BattleEntity.
        /// Keep in mind that the Entity reference here can be the same as the Entity being affected if it affected itself
        /// </summary>
        public struct AffectableInfo
        {
            public BattleEntity Affector;
            public AffectableBase AffectableObj;

            public AffectableInfo(BattleEntity affector, AffectableBase affectableobj)
            {
                Affector = affector;
                AffectableObj = affectableobj;
            }
        }

        #endregion

        #region Constant Values

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

        private static readonly string CurPath = Environment.CurrentDirectory + "/";
        public static readonly string ContentPath = CurPath + "Content/";

        public const float BASE_BACKGROUND_LAYER = 0f;
        public const float BASE_ENTITY_LAYER = 1f;
        public const float BASE_UI_LAYER = 2f;

        #endregion

        #region Static Global Objects

        public static readonly Random Randomizer = new Random();

        #endregion
    }
}
