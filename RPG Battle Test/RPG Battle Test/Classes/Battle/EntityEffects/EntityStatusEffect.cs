using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class EntityStatusEffect : EntityEffect
    {
        public StatusEffect Status = null;
        public float StatusPercent = 0f;

        public EntityStatusEffect(string name, StatusEffect status, float statuspercentage) : base(name)
        {
            Status = status;
            StatusPercent = Helper.Clamp(statuspercentage, 0f, 100f);
        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            if (Status != null)
            {
                for (int i = 0; i < Entities.Length; i++)
                {
                    float percent = (float)Math.Round(Globals.Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        Entities[i].InflictStatus(new Globals.AffectableInfo(User, this), Status);
                    }
                }
            }
            else
            {
                Debug.LogError($"Status for Spell {Name} by {User.Name} is null!");
            }
        }

        public override EntityEffect Copy()
        {
            return new EntityStatusEffect(Name, Status?.Copy(), StatusPercent);
        }
    }
}
