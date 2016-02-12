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
    public abstract class Item
    {
        /// <summary>
        /// Types of items. An item can be classified as more than one type
        /// </summary>
        public enum ItemTypes
        {
            None, Heal, Damage, NegativeStatus, PositiveStatus
        }

        public delegate void ItemUse(Item item);

        public static event ItemUse ItemUseEvent = null;

        /// <summary>
        /// The table of all existing items in the game
        /// </summary>
        public static Item[] ItemTable = null;

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; protected set; } = "Item";

        /// <summary>
        /// Whether the item is multi-target or not.
        /// If false only one target can be selected, otherwise all targets will be selected
        /// </summary>
        public bool MultiTarget { get; protected set; } = false;

        /// <summary>
        /// The state of the BattleEntities that the Item affects
        /// </summary>
        public BattleManager.EntityFilterStates FilterState { get; protected set; } = BattleManager.EntityFilterStates.Alive;

        /// <summary>
        /// The dictionary of types the item is classified as. A particular key existing means the item is classified as that type
        /// </summary>
        public readonly Dictionary<ItemTypes, bool> TypeList = new Dictionary<ItemTypes, bool>();

        static Item()
        {
            ItemTable = new Item[]
            {
                new HealingItem("Potion", false, 20, 0),
                new HealingItem("Ether", false, 0, 20),
                new DamageItem("Bomb", 10, Globals.DamageTypes.Physical, Globals.Elements.Neutral),
                new PercentageHealingItem("Phoenix Down", false, .2f, 0f, BattleManager.EntityFilterStates.Dead)
            };
        }

        protected Item(string name)
        {
            Name = name;
        }

        protected Item(string name, bool multitarget) : this(name)
        {
            MultiTarget = multitarget;
        }

        protected Item(Item item)
        {
            Name = item.Name;
            MultiTarget = MultiTarget;
            TypeList = new Dictionary<ItemTypes, bool>(item.TypeList);
        }

        /// <summary>
        /// Uses the item. This is here so we can call events
        /// </summary>
        /// <param name="entities"></param>
        public void Use(params BattleEntity[] entities)
        {
            //Call use event
            ItemUseEvent?.Invoke(this);

            OnUse(entities);
        }

        /// <summary>
        /// What happens to the entities when the Item is used on it
        /// </summary>
        /// <param name="entities"></param>
        protected abstract void OnUse(params BattleEntity[] entities);

        /// <summary>
        /// Tells if an item is a particular type
        /// </summary>
        /// <param name="itemtype">The type of item to test comparison for</param>
        /// <returns>true if the item is classified as the given type</returns>
        public bool IsOfType(ItemTypes itemtype)
        {
            return TypeList.ContainsKey(itemtype);
        }

        /// <summary>
        /// Tells if an item is of all the given types
        /// </summary>
        /// <param name="itemtypes">An array of ItemTypes to test comparison for</param>
        /// <returns>true if the item is classified as all of the given types, otherwise false</returns>
        public bool IsOfAllTypes(params ItemTypes[] itemtypes)
        {
            for (int i = 0; i < itemtypes.Length; i++)
            {
                if (IsOfType(itemtypes[i]) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a new copy of the Item instance
        /// </summary>
        /// <returns></returns>
        public abstract Item Copy();
    }
}
