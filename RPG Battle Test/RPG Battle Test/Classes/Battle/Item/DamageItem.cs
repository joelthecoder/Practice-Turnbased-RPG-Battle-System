using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public class DamageItem : Item
    {
        public int Damage = 0;
        public DamageTypes DamageType = DamageTypes.None;
        public Elements Element = Elements.Neutral;

        public DamageItem(string name, int damage, DamageTypes damageType, Elements element) : base(name)
        {
            Damage = damage;
            DamageType = damageType;
            Element = element;

            TypeList.Add(ItemTypes.Damage, true);
        }

        protected override void OnUse(BattleEntity entity)
        {
            entity.TakeDamage(Damage, DamageType, Element);
        }
    }
}
