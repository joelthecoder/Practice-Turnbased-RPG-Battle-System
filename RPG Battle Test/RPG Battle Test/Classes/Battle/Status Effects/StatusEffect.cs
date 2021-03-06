﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.UsableBase;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base, abstract class for Status Effects. Status Effects can range from poison to just about anything.
    /// <para>All derived classes should have the "Status" suffix at the end of their type names for clarity purposes</para>
    /// </summary>
    public abstract class StatusEffect : AffectableBase
    {
        /// <summary>
        /// Delegate for the StatusFinishedEvent
        /// </summary>
        /// <param name="status">The StatusEffect that has just finished</param>
        public delegate void StatusFinished(StatusEffect status);

        /// <summary>
        /// The event called whenever this StatusEffect is finished. This automatically subscribes to the BattleEntity afflicted
        /// with this StatusEffect
        /// </summary>
        public event StatusFinished StatusFinishedEvent = null;
        
        /// <summary>
        /// The afflicted entity
        /// </summary>
        public BattleEntity Entity { get; protected set; } = null;

        /// <summary>
        /// The entity that afflicted the StatusEffect
        /// </summary>
        public BattleEntity Afflicter { get; protected set; } = null;

        /// <summary>
        /// The number of turns the status effect is in effect for
        /// </summary>
        public int Turns { get; protected set; } = 1;

        /// <summary>
        /// The number of turns the status effect has been in effect for
        /// </summary>
        protected int TurnsPassed { get; private set; } = 0;

        /// <summary>
        /// States if the status effect is finished
        /// </summary>
        public bool IsFinished => (TurnsPassed >= Turns);

        protected StatusEffect(int turns)
        {
            Turns = turns;
            Name = "StatusEffect";
            AffectableType = AffectableTypes.Status;
        }

        /// <summary>
        /// Refreshes the status effect by resetting the number of turns passed to 0
        /// </summary>
        public virtual void Refresh()
        {
            TurnsPassed = 0;
        }

        /// <summary>
        /// Sets the afflicter of the status effect
        /// </summary>
        /// <param name="entity">The entity that afflicted the status effect</param>
        public void SetAfflicter(BattleEntity entity)
        {
            Afflicter = entity;
        }

        /// <summary>
        /// Sets the receiver of the status effect
        /// </summary>
        /// <param name="entity">The entity receiving the status effect</param>
        public void SetReceiver(BattleEntity entity)
        {
            Entity = entity;

            Entity.TurnStartEvent += OnTurnStart;
            Entity.TurnEndEvent += OnTurnEnd;
        }

        /// <summary>
        /// Increments the length the status effect has been in effect and ending it if the effect is finished
        /// </summary>
        protected void IncrementTurns()
        {
            //Ensure End() won't be called more than once
            if (IsFinished == false)
            {
                TurnsPassed++;
                if (IsFinished)
                {
                    End();
                    StatusFinishedEvent?.Invoke(this);
                }
            }
            else
            {
                Debug.LogError($"Attempted to call {nameof(IncrementTurns)} for StatusEffect: {Name} on Entity: {Entity.Name} after it already ended!");
            }
        }

        /// <summary>
        /// What the status effect does when it's first inflicted on the entity
        /// </summary>
        public abstract void OnInflict();

        /// <summary>
        /// Ends the StatusEffect
        /// </summary>
        public void End()
        {
            Entity.TurnStartEvent -= OnTurnStart;
            Entity.TurnEndEvent -= OnTurnEnd;

            OnEnd();
        }

        /// <summary>
        /// What the status effect does when it ends. Here's where you'd reset values on the entity
        /// </summary>
        protected abstract void OnEnd();

        /// <summary>
        /// What the status effect does at the start of the entity's turn
        /// </summary>
        protected abstract void OnTurnStart();

        /// <summary>
        /// What the status effect does at the end of the entity's turn
        /// </summary>
        protected abstract void OnTurnEnd();

        /// <summary>
        /// Copies the StatusEffect's properties and returns a new instance
        /// </summary>
        /// <returns>A new instance of the StatusEffect with the same properties. It does not copy the Afflicter and Receiver</returns>
        public abstract StatusEffect Copy();
    }
}
