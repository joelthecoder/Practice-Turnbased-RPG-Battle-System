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
        public AttackCommand() : base("Attack")
        {
            
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            for (int i = 0; i < Victims.Length; i++)
            {
                Debug.Log(Attacker.Name + " attacked " + Victims[i].Name + "!");
                Victims[i].TakeDamage(new Globals.AffectableInfo(Attacker, null), Attacker.CalculateDamageDealt(Attacker.GetAttackDamageType(), Attacker.GetAttackElement())
                , Attacker.GetAttackDamageType(), Attacker.GetAttackElement());
            }
        }
    }
}
