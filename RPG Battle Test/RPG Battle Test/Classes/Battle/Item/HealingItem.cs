using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class HealingItem: Item
    {
        private uint HPRestored = 0;
        private uint MPRestored = 0;

        public HealingItem(string name, bool multitarget, uint hpRestored, uint mpRestored) : base(name, multitarget)
        {
            HPRestored = hpRestored;
            MPRestored = mpRestored;

            MultiTarget = multitarget;
            TypeList.Add(ItemTypes.Heal, true);
        }

        protected HealingItem(string name, bool multitarget) : this(name, multitarget, 0, 0)
        {

        }

        protected override void UseItem(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].Restore(HPRestored, MPRestored);

                Debug.Log($"Healed {Entities[i].Name} for {HPRestored} HP and {MPRestored} MP!");
            }
        }

        public override Item Copy()
        {
            return new HealingItem(Name, MultiTarget, HPRestored, MPRestored);
        }
    }
}
