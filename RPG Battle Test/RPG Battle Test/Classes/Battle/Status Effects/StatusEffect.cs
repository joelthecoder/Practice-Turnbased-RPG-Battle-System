using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base, abstract class for Status Effects. Status Effects can range from poison to just about anything
    /// </summary>
    public abstract class StatusEffect
    {
        //The afflicted entity
        public BattleEntity Entity = null;

        /// <summary>
        /// The number of turns the status effect is in effect for
        /// </summary>
        public int Turns = 1;

        /// <summary>
        /// The number of turns the status effect has been in effect for
        /// </summary>
        public int TurnsPassed = 0;

        /// <summary>
        /// What the status effect does when it's first inflicted on the entity
        /// </summary>
        public abstract void OnInflict();

        /// <summary>
        /// What the status effect does when it ends. Here's where you'd reset values on the entity
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// What the status effect does at the start of the entity's turn
        /// </summary>
        public abstract void OnTurnStart();

        /// <summary>
        /// What the status effect does at the end of the entity's turn
        /// </summary>
        public abstract void OnTurnEnd();
    }
}
