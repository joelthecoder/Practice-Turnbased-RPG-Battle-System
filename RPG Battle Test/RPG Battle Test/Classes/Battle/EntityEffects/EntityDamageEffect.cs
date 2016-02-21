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

        /// <summary>
        /// Determines whether the damage this effect deals factors in the damage the user will deal or not
        /// </summary>
        public bool BasedOnEntityDamage = false;

        public EntityDamageEffect(string name, int damage, DamageTypes damageType, Elements element, bool basedOnEntityDmg) : base(name, null, 0f)
        {
            Damage = damage;
            DamageType = damageType;
            Element = element;

            BasedOnEntityDamage = basedOnEntityDmg;
        }

        public EntityDamageEffect(string name, int damage, DamageTypes damageType, Elements element, StatusEffect status, float statuspercentage, bool basedOnEntityDmg)
            : base(name, status, statuspercentage)
        {
            Damage = damage;
            DamageType = damageType;
            Element = element;

            BasedOnEntityDamage = basedOnEntityDmg;
        }

        public override void UseEffect(AffectableInfo affectableInfo, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                int entityDamage = 0;
                if (BasedOnEntityDamage == true)
                    entityDamage = affectableInfo.Affector.CalculateDamageDealt(DamageType, Element);

                //Calculate damage and add this Effect's damage
                Entities[i].TakeDamage(affectableInfo, entityDamage + Damage, DamageType, Element);

                //If no Status (only damage), don't bother inflicting
                if (Status != null)
                {
                    float percent = (float)Math.Round(Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        Entities[i].InflictStatus(affectableInfo, Status);
                    }
                }
            }
        }

        public override EntityEffect Copy()
        {
            return new EntityDamageEffect(Name, Damage, DamageType, Element, Status?.Copy(), StatusPercent, BasedOnEntityDamage);
        }
    }
}
