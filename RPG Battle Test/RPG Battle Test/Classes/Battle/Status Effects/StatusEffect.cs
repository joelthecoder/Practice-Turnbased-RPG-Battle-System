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
        public enum StatusTypes
        {
            Neutral, Positive, Negative
        }

        public delegate void StatusFinished(StatusEffect status);

        public event StatusFinished OnStatusFinished = null;

        //The afflicted entity
        public BattleEntity Entity { get; protected set; } = null;

        /// <summary>
        /// The number of turns the status effect is in effect for
        /// </summary>
        public int Turns { get; protected set; } = 1;

        /// <summary>
        /// The name of the status effect
        /// </summary>
        public string Name { get; protected set; } = "StatusEffect";

        /// <summary>
        /// The type of status this is - Neutral, Positive, or Negative
        /// </summary>
        public StatusTypes StatusType { get; protected set; } = StatusTypes.Neutral;

        /// <summary>
        /// The number of turns the status effect has been in effect for
        /// </summary>
        protected int TurnsPassed = 0;

        /// <summary>
        /// States if the status effect is finished
        /// </summary>
        public bool IsFinished => (TurnsPassed >= Turns);

        protected StatusEffect(int turns)
        {
            Turns = turns;
        }

        /// <summary>
        /// Refreshes the status effect by resetting the number of turns passed to 0
        /// </summary>
        public virtual void Refresh()
        {
            TurnsPassed = 0;
        }

        /// <summary>
        /// Sets the receiver of the status effect
        /// </summary>
        /// <param name="entity"></param>
        public void SetReceiver(BattleEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Increments the length the status effect has been in effect and ending it if the effect is finished
        /// </summary>
        protected void IncrementTurns()
        {
            TurnsPassed++;
            if (IsFinished)
            {
                OnEnd();
                OnStatusFinished?.Invoke(this);
            }
        }

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
