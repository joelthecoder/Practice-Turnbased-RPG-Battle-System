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

        public HealingItem(string name, uint hpRestored, uint mpRestored) : base(name)
        {
            HPRestored = hpRestored;
            MPRestored = mpRestored;

            TypeList.Add(ItemTypes.Heal, true);
        }

        protected override void OnUse(BattleEntity entity)
        {
            entity.Restore(HPRestored, MPRestored);

            Debug.Log($"Healed {entity.Name} for {HPRestored} HP and {MPRestored} MP!");
        }
    }
}
