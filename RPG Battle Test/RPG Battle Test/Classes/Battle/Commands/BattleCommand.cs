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
        public bool IsCommandFinished => Performed;

        /// <summary>
        /// The number of turns required to complete this command
        /// </summary>
        protected uint TurnsRequired = 0;

        /// <summary>
        /// The number of turns used to complete this command
        /// </summary>
        protected uint TurnsUsed = 0;

        /// <summary>
        /// Tells whether the command has been successfully performed or not
        /// </summary>
        private bool Performed = false;

        /// <summary>
        /// The previous BattleEntity that used this command
        /// </summary>
        private BattleEntity PrevAttacker = null;

        /// <summary>
        /// The previous set of BattleEntities targeted with this command
        /// </summary>
        private BattleEntity[] PrevVictims = null;

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
            PrevAttacker = Attacker;
            PrevVictims = Victims;

            //Carry out the command if it should be used
            if (TurnsUsed >= TurnsRequired)
            {
                Perform(Attacker, Victims);

                Interrupt();
            }
            //Otherwise increment the turns
            else
            {
                Debug.Log($"Performing {Name}! Current turn: {TurnsUsed} out of {TurnsRequired}");
                TurnsUsed++;
                Performed = false;
            }
        }

        /// <summary>
        /// Performs the command with the same Attacker and Victims it was performed with previously
        /// </summary>
        public void Reperform()
        {
            PerformAction(PrevAttacker, PrevVictims);
        }

        /// <summary>
        /// Interrupts the BattleCommand, setting the number of turns it's been in use to 0 and stating that it has been performed.
        /// This is used by StatusEffects, like Sleep, that interrupt multi-turn actions such as casting spells
        /// </summary>
        public void Interrupt()
        {
            TurnsUsed = 0;
            Performed = true;
        }

        /// <summary>
        /// What happens when the command is performed
        /// </summary>
        /// <param name="Attacker">The BattleEntity performing the action</param>
        /// <param name="Victims">The BattleEntities on the receiving end of the action</param>
        protected abstract void Perform(BattleEntity Attacker, params BattleEntity[] Victims);
    }
}
