using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class HealingItem: Item
    {
        public uint HPRestored = 0;
        public uint MPRestored = 0;

        public HealingItem(string name, bool multitarget, uint hpRestored, uint mpRestored) : base(name, multitarget)
        {
            HPRestored = hpRestored;
            MPRestored = mpRestored;

            MultiTarget = multitarget;
            TypeList.Add(ItemTypes.Heal, true);
        }

        protected override void OnUse(params BattleEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Restore(HPRestored, MPRestored);

                Debug.Log($"Healed {entities[i].Name} for {HPRestored} HP and {MPRestored} MP!");
            }
        }
    }
}
