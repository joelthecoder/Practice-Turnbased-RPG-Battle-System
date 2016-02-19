using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public class EntityDamageEffect : EntityStatusEffect
    {
        public int Damage = 0;
        public DamageTypes DamageType = DamageTypes.None;
        public Elements Element = Elements.Neutral;

        public EntityDamageEffect(string name, int damage, DamageTypes damageType, Elements element) : base(name, null, 0f)
        {

        }

        public EntityDamageEffect(string name, int damage, DamageTypes damageType, Elements element, StatusEffect status, float statuspercentage)
            : base(name, status, statuspercentage)
        {

        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                //Calculate damage and add this Effect's damage
                Entities[i].TakeDamage(new AffectableInfo(User, this), User.CalculateDamageDealt(DamageType, Element) + Damage, DamageType, Element);

                //If no Status (only damage), don't bother inflicting
                if (Status != null)
                {
                    float percent = (float)Math.Round(Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        Entities[i].InflictStatus(new AffectableInfo(User, this), Status);
                    }
                }
            }
        }

        public override EntityEffect Copy()
        {
            return new EntityDamageEffect(Name, Damage, DamageType, Element, Status?.Copy(), StatusPercent);
        }
    }
}
