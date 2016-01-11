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
    public class DamageSpell : Spell
    {
        public int Damage = 0;
        public DamageTypes DamageType = DamageTypes.Magic;
        public Elements Element = Elements.Neutral;
        public StatusEffect Status = null;
        public float StatusPercent = 0f;

        public DamageSpell(string name, int mpCost, int damage, DamageTypes damageType, Elements element) : base(name, mpCost)
        {
            SpellType = SpellTypes.Negative;

            Damage = damage;
            DamageType = damageType;
            Element = element;
        }

        public DamageSpell(string name, int mpCost, int damage, DamageTypes damageType, Elements element, StatusEffect status, float statuspercentage)
            : this(name, mpCost, damage, damageType, element)
        {
            Status = status;
            StatusPercent = Helper.Clamp(statuspercentage, 0f, 100f);
        }

        public override void OnUse(BattleEntity Attacker, params BattleEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                //Calculate damage and add the Spell's damage
                entities[i].TakeDamage(Attacker.CalculateDamageDealt(DamageType, Element) + Damage, DamageType, Element);

                //If no Status (only damage), don't bother inflicting
                if (Status != null)
                {
                    float percent = (float)Math.Round(Randomizer.NextDouble() * 100f);
                    if (StatusPercent > percent)
                    {
                        entities[i].InflictStatus(Status);
                    }
                }
            }
        }

        public override Spell Copy()
        {
            return new DamageSpell(Name, MPCost, Damage, DamageType, Element, Status?.Copy(), StatusPercent);
        }
    }
}
