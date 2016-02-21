using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Party item inventory. Parties are not planned to be implemented, but if they are this will be revised
    /// </summary>
    public sealed class Inventory : IDisposable
    {
        private Dictionary<Item, int> ItemDictionary = null;

        public Inventory()
        {
            ItemDictionary = new Dictionary<Item, int>();

            //We don't have an actual party, so populate this for testing
            ItemDictionary.Add(Item.ItemTable[0], 5);
            ItemDictionary.Add(Item.ItemTable[1], 3);
            ItemDictionary.Add(Item.ItemTable[2], 1);
            ItemDictionary.Add(Item.ItemTable[3], 5);

            Item.ItemUseEvent += OnItemUse;
        }

        public Dictionary<Item, int> GetInventory()
        {
            return ItemDictionary;
        }

        public void Dispose()
        {
            Item.ItemUseEvent -= OnItemUse;
        }

        /// <summary>
        /// Returns the number of unique items in the party inventory
        /// </summary>
        /// <returns></returns>
        public int ItemCount()
        {
            return ItemDictionary.Count;
        }

        /// <summary>
        /// Returns a quantity of the specified item
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The quantity of the specified item, if it exists, otherwise 0</returns>
        public int GetQuantity(Item item)
        {
            if (item != null && ItemDictionary.ContainsKey(item))
            {
                return ItemDictionary[item];
            }
            
            return 0;
        }

        /// <summary>
        /// Returns a quantity of the specified item by name
        /// </summary>
        /// <param name="itemname">The name of the item to find</param>
        /// <returns>The quantity of the specified item, if it exists, otherwise 0</returns>
        public int GetQuantity(string itemname)
        {
            Item item = GetItemByName(itemname);

            return GetQuantity(item);
        }

        /// <summary>
        /// Returns a string representation of the inventory as a list. The format is "(ItemName) x(ItemQuantity)"
        /// </summary>
        /// <returns></returns>
        public List<string> GetInventoryList()
        {
            List<string> invList = new List<string>();

            foreach(KeyValuePair<Item, int> pair in ItemDictionary)
            {
                invList.Add($"{pair.Key} x{pair.Value}");
            }

            return invList;
        }

        private Item GetItemByName(string itemname)
        {
            foreach(KeyValuePair<Item, int> pair in ItemDictionary)
            {
                Item key = pair.Key;
                if (key.Name == itemname)
                {
                    return key;
                }
            }

            return null;
        }

        private void OnItemUse(Item item, BattleEntity User)
        {
            //Don't subtract from the Inventory if the user is an enemy
            if (User.IsEnemy == true)
                return;

            if (ItemDictionary.ContainsKey(item))
            {
                if (--ItemDictionary[item] <= 0)
                {
                    ItemDictionary.Remove(item);
                }
            }
            else
            {
                Debug.LogError($"Somehow used item {item.Name}, which doesn't exist in the inventory!");
            }
        }
    }
}
