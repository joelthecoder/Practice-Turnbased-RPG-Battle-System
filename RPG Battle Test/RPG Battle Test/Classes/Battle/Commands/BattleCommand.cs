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
    /// <summary>
    /// The base class for commands available for use in battle.
    /// These include, but are not limited to, Attack, Defend, Magic, Item, Run
    /// <para>This is a layer on top of the actions themselves, allowing for more flexibility.
    /// For example, you can have a DoubleSpell command that uses the same Spell twice while subtracting the MP cost only once.</para>
    /// </summary>
    public abstract class BattleCommand
    {
        /// <summary>
        /// The name of the command.
        /// For Players, this will show up in the selection menu.
        /// </summary>
        public string Name { get; protected set; } = "BattleCommand";

        /// <summary>
        /// Returns whether the Command is finished or not.
        /// This is used for keeping a BattleEntity locked into a certain Command until it ends (Ex. casting time for a spell)
        /// </summary>
        public virtual bool IsCommandFinished => true;

        protected BattleCommand(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Perform a command
        /// </summary>
        /// <param name="Attacker">The BattleEntity performing the action</param>
        /// <param name="Victims">The BattleEntities on the receiving end of the action</param>
        public void PerformAction(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            Perform(Attacker, Victims);
        }

        /// <summary>
        /// What happens when the command is performed
        /// </summary>
        /// <param name="Attacker">The BattleEntity performing the action</param>
        /// <param name="Victims">The BattleEntities on the receiving end of the action</param>
        protected abstract void Perform(BattleEntity Attacker, params BattleEntity[] Victims);
    }
}
