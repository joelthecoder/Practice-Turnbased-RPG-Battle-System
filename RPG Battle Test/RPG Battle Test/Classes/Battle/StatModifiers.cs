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
    /// Handles stat modifications made to a BattleEntity during battle. The base value of the stat is not changed.
    /// Percentage modifications start at 1 and stack additively.
    /// Resulting percentage modifications will look like 1.2, .7, and .55
    /// </summary>
    public sealed class StatModifiers
    {
        public enum StatModTypes
        {
            Attack, MagicAtk, Defense, MagicDef, Speed
        }

        private Dictionary<StatModTypes, List<StatMod>> StatsModified = new Dictionary<StatModTypes, List<StatMod>>();

        /// <summary>
        /// Total stats applied; easy access
        /// </summary>
        private Dictionary<StatModTypes, StatMod> TotalStats = new Dictionary<StatModTypes, StatMod>();

        public StatModifiers()
        {
            StatModTypes[] types = (StatModTypes[])Enum.GetValues(typeof(StatModTypes));
            for (int i = 0; i < types.Length; i++)
            {
                TotalStats.Add(types[i], new StatMod(0, 1f));
            }
        }

        /// <summary>
        /// Adds a stat modifier
        /// </summary>
        /// <param name="statModType">The type of stat modifier to add</param>
        /// <param name="amount">The flat Amount the stat is modified by</param>
        /// <param name="percentage">The amount in percentage the stat should be modified by</param>
        public void AddModifier(StatModTypes statModType, int amount, float percentage)
        {
            if (StatsModified.ContainsKey(statModType))
            {
                StatsModified[statModType].Add(new StatMod(amount, percentage));
            }
            else
            {
                StatsModified.Add(statModType, new List<StatMod>() { new StatMod(amount, percentage) });
            }

            TotalStats[statModType].Amount += amount;
            TotalStats[statModType].Percentage += percentage;
        }

        /// <summary>
        /// Removes a modifier with a designated Amount
        /// </summary>
        /// <param name="statModType">The type of the StatModifier</param>
        /// <param name="amount">The Amount corresponding to a particular StatMod</param>
        /// <returns>Whether the modifier was successfully found and removed or not</returns>
        public bool RemoveModifierWithAmount(StatModTypes statModType, int amount)
        {
            if (StatsModified.ContainsKey(statModType))
            {
                List<StatMod> statMods = StatsModified[statModType];
                for (int i = 0; i < statMods.Count; i++)
                {
                    if (statMods[i].Amount == amount)
                    {
                        TotalStats[statModType].Amount -= amount;
                        TotalStats[statModType].Percentage -= statMods[i].Percentage;
                        statMods.RemoveAt(i);
                        //Debug.Log($"Removed StatMod of type {statModType} with Amount {amount}!");
                        return true;
                    }
                }
            }
            
            Debug.Log($"Cannot find StatMod of type {statModType} with Amount {amount}");
            return false;
        }

        /// <summary>
        /// Removes a modifier with a designated percentage
        /// </summary>
        /// <param name="statModType">The type of the StatModifier</param>
        /// <param name="amount">The percentage corresponding to a particular StatMod</param>
        /// <returns>Whether the modifier was successfully found and removed or not</returns>
        public bool RemoveModifierWithPercentage(StatModTypes statModType, float percentage)
        {
            if (StatsModified.ContainsKey(statModType))
            {
                List<StatMod> statMods = StatsModified[statModType];
                for (int i = 0; i < statMods.Count; i++)
                {
                    if (statMods[i].Percentage == percentage)
                    {
                        TotalStats[statModType].Percentage -= percentage;
                        TotalStats[statModType].Amount -= statMods[i].Amount;
                        statMods.RemoveAt(i);
                        //Debug.Log($"Removed StatMod of type {statModType} with Percentage {percentage}!");
                        return true;
                    }
                }
            }
            
            Debug.Log($"Cannot find StatMod of type {statModType} with Percentage {percentage}");
            return false;
        }

        public void ClearModifiers()
        {
            foreach (KeyValuePair<StatModTypes, StatMod> pair in TotalStats)
            {
                pair.Value.Amount = 0;
                pair.Value.Percentage = 1f;
            }
            StatsModified.Clear();
        }

        /// <summary>
        /// Gets the value of all flat Amount modifiers affecting a particular stat. This method is in O(1) time
        /// </summary>
        /// <param name="statModType">The type of stat modifier to retrieve the Amount for</param>
        /// <returns>The total value of the amount modifiers affecting the stat. The base value is 0</returns>
        public int GetModifierAmount(StatModTypes statModType)
        {
            return TotalStats[statModType].Amount;
        }

        /// <summary>
        /// Returns the sum of all flat Amount modifiers affecting a particular stat
        /// </summary>
        /// <param name="statModType">The type of stat modifier to retrieve the Amount for</param>
        /// <returns>The total value of the Amount modifiers affecting the stat. The base value is 0</returns>
        public int SumAmountModifier(StatModTypes statModType)
        {
            int amount = 0;

            //Check if the key exists
            if (StatsModified.ContainsKey(statModType))
            {
                List<StatMod> modifiers = StatsModified[statModType];

                //Simply add all the values together
                for (int i = 0; i < modifiers.Count; i++)
                {
                    amount += modifiers[i].Amount;
                }
            }

            return amount;
        }

        /// <summary>
        /// Gets the value of all percent modifiers affecting a particular stat. This method is in O(1) time
        /// </summary>
        /// <param name="statModType">The type of stat modifier to retrieve the percentage for</param>
        /// <returns>The total value of the percent modifiers affecting the stat. The base value is 1</returns>
        public float GetModifierPercent(StatModTypes statModType)
        {
            return TotalStats[statModType].Percentage;
        }

        /// <summary>
        /// Returns the sum of all percent modifiers affecting a particular stat
        /// </summary>
        /// <param name="statModType">The type of stat modifier to retrieve the percentage for</param>
        /// <returns>The total value of the percent modifiers affecting the stat. The base value is 1</returns>
        public float SumPercentModifier(StatModTypes statModType)
        {
            float percentage = 1f;

            if (StatsModified.ContainsKey(statModType))
            {
                List<StatMod> modifiers = StatsModified[statModType];

                for (int i = 0; i < modifiers.Count; i++)
                {
                    //Percentages are stacked additively
                    percentage += modifiers[i].Percentage;
                }
            }

            return percentage;
        }

        private class StatMod
        {
            public int Amount = 0;
            public float Percentage = 0f;

            public StatMod(int amount, float percentage)
            {
                Amount = amount;
                Percentage = percentage;
            }
        }
    }
}
