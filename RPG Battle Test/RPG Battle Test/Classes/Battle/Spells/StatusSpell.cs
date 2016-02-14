using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public class StatusSpell : Spell
    {
        public StatusEffect Status = null;
        public float StatusPercent = 0f;

        public StatusSpell(string name, int mpCost, bool multiTarget, StatusEffect status, float statuspercentage)
            : base(name, mpCost, multiTarget)
        {
            Status = status;
            StatusPercent = Helper.Clamp(statuspercentage, 0f, 100f);

            if (Status != null)
            {
                Alignment = (UsableAlignment)Status.StatusType;
            }
        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            if (Status != null)
            {
                for (int i = 0; i < Entities.Length; i++)
                {
                    float percent = (float)Math.Round(Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        Entities[i].InflictStatus(User, Status);
                    }
                }
            }
            else
            {
                Debug.LogError($"Status for Spell {Name} by {User.Name} is null!");
            }
        }

        public override Spell Copy()
        {
            return new StatusSpell(Name, MPCost, MultiTarget, Status?.Copy(), StatusPercent);
        }
    }
}
