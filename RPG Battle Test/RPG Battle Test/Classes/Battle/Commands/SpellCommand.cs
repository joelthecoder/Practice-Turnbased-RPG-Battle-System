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
            string usedOn = string.Empty;
            if (Victims.Length == 1)
                usedOn = $" on {Victims[0].Name}";
            Debug.Log($"{Attacker.Name} cast {SpellCast.Name}{usedOn}!");

            Attacker.DrainMP(new Globals.AffectableInfo(Attacker, SpellCast), SpellCast.MPCost);
            SpellCast.OnUse(Attacker, Victims);
        }
    }
}
