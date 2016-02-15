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
    public class DamageSpell : StatusSpell
    {
        public int Damage = 0;
        public DamageTypes DamageType = DamageTypes.Magic;
        public Elements Element = Elements.Neutral;

        public DamageSpell(string name, int mpCost, int damage, bool multiTarget, DamageTypes damageType, Elements element)
            : base(name, mpCost, multiTarget, null, 0f)
        {
            Alignment = UsableAlignment.Negative;

            Damage = damage;
            DamageType = damageType;
            Element = element;
        }

        public DamageSpell(string name, int mpCost, int damage, bool multiTarget, DamageTypes damageType, Elements element, StatusEffect status, float statuspercentage)
            : this(name, mpCost, damage, multiTarget, damageType, element)
        {
            Status = status;
            StatusPercent = Helper.Clamp(statuspercentage, 0f, 100f);
        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                //Calculate damage and add the Spell's damage
                Entities[i].TakeDamage(new AffectableInfo(User, this), User.CalculateDamageDealt(DamageType, Element) + Damage, DamageType, Element);

                //If no Status (only damage), don't bother inflicting
                if (Status != null)
                {
                    float percent = (float)Math.Round(Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        Entities[i].InflictStatus(User, Status);
                    }
                }
            }
        }

        public override Spell Copy()
        {
            return new DamageSpell(Name, MPCost, Damage, MultiTarget, DamageType, Element, Status?.Copy(), StatusPercent);
        }
    }
}
