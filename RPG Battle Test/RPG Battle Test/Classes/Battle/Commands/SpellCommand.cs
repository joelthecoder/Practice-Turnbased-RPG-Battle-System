using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class SpellCommand : BattleCommand
    {
        protected Spell SpellCast = null;    

        public SpellCommand(Spell spellCast)
        {
            SpellCast = spellCast;
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            Attacker.ModifyMP(Attacker.CurMP - SpellCast.MPCost);
            SpellCast.OnUse(Victims);
        }
    }
}
