using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class DamageItem : Item
    {
        public int Damage = 0;

        public DamageItem(int damage)
        {
            Damage = damage;

            TypeList.Add(ItemTypes.Damage, true);
        }

        public override void OnUse(BattleEntity entity)
        {
            //NOTE: There's currently no way to deal raw damage to an entity, so work on that
        }
    }
}
