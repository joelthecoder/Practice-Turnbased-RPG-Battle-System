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

        public delegate void ItemUse(Item item, BattleEntity User);

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
        /// Uses the item. This is here so we can call events
        /// </summary>
        /// <param name="Entities"></param>
        public void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            //Call use event
            ItemUseEvent?.Invoke(this, User);

            UseItem(User, Entities);
        }

        /// <summary>
        /// What happens to the entities when the Item is used on them
        /// </summary>
        /// <param name="Entities"></param>
        protected void UseItem(BattleEntity User, params BattleEntity[] Entities)
        {
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
        /// <returns></returns>
        public Item Copy()
        {
            return new Item(Name, MultiTarget, Alignment, FilterState, Entityeffect?.Copy());
        }
    }
}
