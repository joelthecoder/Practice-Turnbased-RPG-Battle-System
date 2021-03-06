﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public sealed class DefendCommand : BattleCommand
    {
        public DefendCommand() : base("Defend")
        {
            
        }

        protected override void OnCommandSelected(BattlePlayer player)
        {
            BattleUIManager.Instance.StartTargetSelection(new List<BattleEntity>() { player }, false);
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            //Grant the entity 50% more defense until its next turn
            Attacker.InflictStatus(new Globals.AffectableInfo(Attacker, null), new BigDefenseStatus());

            Debug.Log($"{Attacker.Name} has defended this turn!");
        }
    }
}
