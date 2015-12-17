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
        private BattleEntity Target = null;
        private BattleEntity Attacker = null;

        public AttackCommand(BattleEntity target, BattleEntity attacker)
        {
            Target = target;
            Attacker = attacker;
        }

        public void Perform()
        {
            Attacker.AttackEntity(Target);
        }
    }
}
