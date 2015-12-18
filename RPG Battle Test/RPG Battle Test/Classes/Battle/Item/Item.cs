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
            None, Heal, Damage, Status
        }

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name = "Item";

        /// <summary>
        /// The dictionary of types the item is classified as. A particular key existing means the item is classified as that type
        /// </summary>
        public readonly Dictionary<ItemTypes, bool> TypeList = new Dictionary<ItemTypes, bool>();

        /// <summary>
        /// What happens to the entity when the Item is used on it
        /// </summary>
        /// <param name="entity"></param>
        public abstract void OnUse(BattleEntity entity);

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
    }
}
