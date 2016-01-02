using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public class ItemCommand : BattleCommand
    {
        protected Item ItemUsed = null;

        public ItemCommand(Item itemUsed)
        {
            ItemUsed = itemUsed;
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            string usedOn = string.Empty;
            if (Victims.Length == 1)
                usedOn = $" on {Victims[0].Name}";
            Debug.Log($"{Attacker.Name} used {ItemUsed.Name}{usedOn}!");
            ItemUsed.Use(Victims);
        }
    }
}
