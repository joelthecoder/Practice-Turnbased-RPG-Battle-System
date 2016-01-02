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

        public StatModifiers()
        {
            
        }

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
        }

        public void ClearModifiers()
        {
            StatsModified.Clear();
        }

        public int GetModifierAmount(StatModTypes statModType)
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

        public float GetModifierPercent(StatModTypes statModType)
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
