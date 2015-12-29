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
    public class AttackCommand : BattleCommand
    {
        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            base.PerformAction(Attacker, Victims);

            for (int i = 0; i < Victims.Length; i++)
            {
                Debug.Log(Attacker.Name + " attacked " + Victims[i].Name + "!");
                Victims[i].TakeDamage(Attacker.CalculateDamageDealt(), Attacker.GetAttackDamageType(), Attacker.GetAttackElement());
            }
        }
    }
}
