using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public sealed class DefendCommand : BattleCommand
    {
        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            //Grant the user 50% damage reduction until their next turn
            Attacker.AddStatModifier(StatModifiers.StatModTypes.Defense, 1, 0);//0, .5f);
            Attacker.AddStatModifier(StatModifiers.StatModTypes.MagicDef, 1, 0);//0, .5f);

            Debug.Log($"{Attacker.Name} has defended this turn!");
        }
    }
}
