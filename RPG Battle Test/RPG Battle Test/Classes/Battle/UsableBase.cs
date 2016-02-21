using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base class for anything usable by BattleEntities.
    /// This includes, but is not limited to, Items and Spells
    /// </summary>
    public abstract class UsableBase : AffectableBase
    {
        /// <summary>
        /// The alignment types of the Usable
        /// </summary>
        public enum UsableAlignment
        {
            Neutral, Positive, Negative
        }

        /// <summary>
        /// Whether the Usable is multi-target or not.
        /// If false only one target can be selected, otherwise all targets will be selected
        /// </summary>
        public bool MultiTarget { get; protected set; } = false;

        /// <summary>
        /// The alignment of the Usable
        /// </summary>
        public UsableAlignment Alignment { get; protected set; } = UsableAlignment.Neutral;

        /// <summary>
        /// The state of the BattleEntities that the Usable affects
        /// </summary>
        public BattleManager.EntityFilterStates FilterState { get; protected set; } = BattleManager.EntityFilterStates.Alive;

        /// <summary>
        /// The EntityEffect that the Usable performs on the entities
        /// </summary>
        public EntityEffect Entityeffect { get; protected set; } = null;

        protected UsableBase() : base("Usable")
        {
            
        }

        protected UsableBase(string name) : base(name)
        {
            
        }

        protected UsableBase(string name, bool multitarget) : this(name)
        {
            MultiTarget = multitarget;
        }

        protected UsableBase(string name, bool multitarget, UsableAlignment alignment) : this(name, multitarget)
        {
            Alignment = alignment;
        }

        protected UsableBase(string name, bool multitarget, BattleManager.EntityFilterStates filterstate) : this(name, multitarget)
        {
            FilterState = filterstate;
        }

        protected UsableBase(string name, bool multitarget, UsableAlignment alignment, BattleManager.EntityFilterStates filterstate)
            : this(name, multitarget)
        {
            Alignment = alignment;
            FilterState = filterstate;
        }

        protected UsableBase(string name, bool multitarget, UsableAlignment alignment, BattleManager.EntityFilterStates filterstate, EntityEffect effect)
            : this(name, multitarget, alignment, filterstate)
        {
            Entityeffect = effect;
        }

        public BattleEntity.EntityTypes GetEntityTypeBasedOnAlignment(BattleEntity.EntityTypes entityType)
        {
            UsableAlignment alignment = Alignment;

            //Neutral or no EntityType
            if (alignment == UsableAlignment.Neutral || entityType == BattleEntity.EntityTypes.None)
            {
                return BattleEntity.EntityTypes.None;
            }
            
            //If positive, return EntityTypes.Player if a player, otherwise return EntityTypes.Enemy
            if (alignment == UsableAlignment.Positive)
            {
                return entityType == BattleEntity.EntityTypes.Player ? BattleEntity.EntityTypes.Player : BattleEntity.EntityTypes.Enemy;
            }
            //If negative, return EntityTypes.Enemy if a player, otherwise return EntityTypes.Player
            else
            {
                return entityType == BattleEntity.EntityTypes.Player ? BattleEntity.EntityTypes.Enemy : BattleEntity.EntityTypes.Player;
            }
        }
    }
}
