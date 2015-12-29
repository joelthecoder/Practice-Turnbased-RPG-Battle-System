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
    //The base class for commands available for use in battle
    //These include, but are not limited to, Attack, Defend, Magic, Item, Run
    public abstract class BattleCommand
    {
        /// <summary>
        /// Perform a command
        /// </summary>
        /// <param name="Attacker">The BattleEntity performing the action</param>
        /// <param name="Victims">The BattleEntities on the receiving end of the action</param>
        public void PerformAction(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            Perform(Attacker, Victims);
        }

        protected abstract void Perform(BattleEntity Attacker, params BattleEntity[] Victims);
    }
}
