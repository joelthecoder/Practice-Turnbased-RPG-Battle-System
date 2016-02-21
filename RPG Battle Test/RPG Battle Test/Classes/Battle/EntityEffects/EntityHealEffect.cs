using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public class EntityHealEffect : EntityEffect
    {
        public uint HPRestored = 0;
        public uint MPRestored = 0;
        public string[] StatusesCured = null;

        public EntityHealEffect(string name, uint hprestored, uint mprestored, params string[] statusescured) : base(name)
        {
            HPRestored = hprestored;
            MPRestored = mprestored;
            StatusesCured = statusescured;
        }

        public override void UseEffect(AffectableInfo affectableInfo, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].Restore(affectableInfo, HPRestored, MPRestored);

                Debug.Log($"Healed {Entities[i].Name} for {HPRestored} HP and {MPRestored} MP!");

                //Cure StatusEffects if this effect should cure any
                if (StatusesCured != null)
                {
                    Entities[i].CureStatuses(affectableInfo, StatusesCured);
                }
            }
        }

        public override EntityEffect Copy()
        {
            return new EntityHealEffect(Name, HPRestored, MPRestored, StatusesCured);
        }
    }
}
