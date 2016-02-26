using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base class for items, which have a variety of effects. They can heal, damage, inflict status effects, and more
    /// </summary>
    public class Item : UsableBase, IUsable
    {
        /// <summary>
        /// Types of items. An item can be classified as more than one type
        /// </summary>
        public enum ItemTypes
        {
            None, Heal, Damage, NegativeStatus, PositiveStatus
        }

        /// <summary>
        /// Delegate for the ItemUseEvent
        /// </summary>
        /// <param name="Item">The Item that was used</param>
        /// <param name="User">The BattleEntity that used the Item</param>
        public delegate void ItemUse(Item Item, BattleEntity User);

        /// <summary>
        /// The event called whenever an Item is used
        /// </summary>
        public static event ItemUse ItemUseEvent = null;

        /// <summary>
        /// The table of all existing items in the game
        /// </summary>
        public static Item[] ItemTable = null;

        static Item()
        {
            ItemTable = new Item[]
            {
                new Item("Potion", false, UsableAlignment.Positive, BattleManager.EntityFilterStates.Alive, new EntityHealEffect("Potion", 20, 0)),
                new Item("Ether", false, UsableAlignment.Positive, BattleManager.EntityFilterStates.Alive, new EntityHealEffect("Ether", 0, 20)),
                new Item("Bomb", false, UsableAlignment.Negative, BattleManager.EntityFilterStates.Alive, new EntityDamageEffect("Bomb", 10, Globals.DamageTypes.Physical, Globals.Elements.Neutral, false)),
                new Item("Phoenix Down", false, UsableAlignment.Positive, BattleManager.EntityFilterStates.Dead, new EntityPercentHealEffect("Phoenix Down", .2f, 0f))
            };
        }

        public Item(string name, bool multitarget, UsableAlignment alignment, EntityEffect entityeffect)
            : base(name, multitarget, alignment, BattleManager.EntityFilterStates.Alive, entityeffect)
        {
            
        }

        public Item(string name, bool multitarget, UsableAlignment alignment, BattleManager.EntityFilterStates filterState, EntityEffect entityeffect)
            : base(name, multitarget, alignment, filterState, entityeffect)
        {
            
        }

        protected Item(string name) : base(name)
        {
            Name = name;
            AffectableType = AffectableTypes.Item;
        }

        protected Item(string name, bool multitarget) : this(name)
        {
            MultiTarget = multitarget;
        }

        /// <summary>
        /// Uses the item, inflicting the EntityEffect on the designated entities
        /// </summary>
        /// <param name="User">The BattleEntity that used the Item</param>
        /// <param name="Entities">The BattleEntities affected by the Item</param>
        public void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            //Call use event
            ItemUseEvent?.Invoke(this, User);

            if (Entityeffect == null)
            {
                Debug.LogError($"Item {Name}'s EntityEffect is null!");
                return;
            }

            Entityeffect.UseEffect(new Globals.AffectableInfo(User, this), Entities);
        }

        /// <summary>
        /// Returns a new copy of the Item instance
        /// </summary>
        /// <returns>A deep copy of this Item</returns>
        public Item Copy()
        {
            return new Item(Name, MultiTarget, Alignment, FilterState, Entityeffect?.Copy());
        }
    }
}
