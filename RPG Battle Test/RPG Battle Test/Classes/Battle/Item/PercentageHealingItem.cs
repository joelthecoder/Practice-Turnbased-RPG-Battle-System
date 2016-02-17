using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// An item that restores a percentage of an entity's MaxHP or MaxMP
    /// </summary>
    public class PercentageHealingItem : HealingItem
    {
        private float PercentageHP = 0f;
        private float PercentageMP = 0f;

        public PercentageHealingItem(string name, bool multitarget, float percentagehp, float percentagemp, BattleManager.EntityFilterStates filterState)
            : base(name, multitarget)
        {
            PercentageHP = Helper.Clamp(percentagehp, 0f, 1f);
            PercentageMP = Helper.Clamp(percentagemp, 0f, 1f);

            FilterState = filterState;
        }

        protected override void UseItem(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                uint hpRestored = (uint)(Entities[i].MaxHP * PercentageHP);
                uint mpRestored = (uint)(Entities[i].MaxMP * PercentageMP);

                Entities[i].Restore(new Globals.AffectableInfo(User, this), hpRestored, mpRestored);

                Debug.Log($"Healed {Entities[i].Name} for {hpRestored} HP and {mpRestored} MP!");
            }
        }

        public override Item Copy()
        {
            return new PercentageHealingItem(Name, MultiTarget, PercentageHP, PercentageMP, FilterState);
        }
    }
}
