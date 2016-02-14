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

        public HealingSpell(string name, int mpCost, bool multitarget, uint hprestored, uint mprestored) : base(name, mpCost, multitarget)
        {
            HPRestored = hprestored;
            MPRestored = mprestored;

            Alignment = UsableAlignment.Positive;
        }

        public override void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].Restore(HPRestored, MPRestored);
            }
        }

        public override Spell Copy()
        {
            return new HealingSpell(Name, MPCost, MultiTarget, HPRestored, MPRestored);
        }
    }
}
