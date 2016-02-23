using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public class EntityPercentHealEffect : EntityHealEffect
    {
        private float PercentageHP = 0f;
        private float PercentageMP = 0f;

        public EntityPercentHealEffect(string name, float percentagehp, float percentagemp, params Type[] statusescured) : base(name, 0, 0, statusescured)
        {
            PercentageHP = Helper.Clamp(percentagehp, 0f, 1f);
            PercentageMP = Helper.Clamp(percentagemp, 0f, 1f);
        }

        public override void UseEffect(AffectableInfo affectableInfo, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                uint hpRestored = (uint)(Entities[i].MaxHP * PercentageHP);
                uint mpRestored = (uint)(Entities[i].MaxMP * PercentageMP);

                Entities[i].Restore(affectableInfo, hpRestored, mpRestored);

                Debug.Log($"Healed {Entities[i].Name} for {hpRestored} HP and {mpRestored} MP!");

                //Cure StatusEffects if this effect should cure any
                if (StatusesCured != null)
                {
                    Entities[i].CureStatuses(affectableInfo, StatusesCured);
                }
            }
        }

        public override EntityEffect Copy()
        {
            return new EntityPercentHealEffect(Name, PercentageHP, PercentageMP, StatusesCured);
        }
    }
}
