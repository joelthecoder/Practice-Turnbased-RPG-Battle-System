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

namespace RPG_Battle_Test
{
    public class HealingSpell : Spell
    {
        public uint HPRestored = 0;
        public uint MPRestored = 0;
        public string[] StatusesCured = null;

        public HealingSpell(string name, int mpCost, bool multitarget, uint hprestored, uint mprestored) : base(name, mpCost, multitarget)
        {
            HPRestored = hprestored;
            MPRestored = mprestored;

            Alignment = UsableAlignment.Positive;
        }

        public HealingSpell(string name, int mpCost, bool multitarget, uint hprestored, uint mprestored, params string[] statusescured)
            : this(name, mpCost, multitarget, hprestored, mprestored)
        {
            StatusesCured = statusescured;
        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].Restore(new Globals.AffectableInfo(User, this), HPRestored, MPRestored);

                //Cure StatusEffects if this Spell should cure any
                if (StatusesCured != null)
                {
                    Entities[i].CureStatuses(new Globals.AffectableInfo(User, this), StatusesCured);
                }
            }
        }

        public override Spell Copy()
        {
            return new HealingSpell(Name, MPCost, MultiTarget, HPRestored, MPRestored, StatusesCured);
        }
    }
}
