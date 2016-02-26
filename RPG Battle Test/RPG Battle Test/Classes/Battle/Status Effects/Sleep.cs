using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Immobilizes an Entity for a specified number of turns.
    /// <para>IMPORTANT NOTE: This automatically ends an Entity's turn if the effect has not worn off yet.
    /// It's not recommended to have this and similar StatusEffects (Petrify, etc.) on the same entity at the same time,
    /// as it can cause OnTurnEnd() of some inflicted statuses to occur before OnTurnStart()</para>
    /// </summary>
    public class Sleep : StatusEffect
    {
        public Sleep(int turns) : base(turns)
        {
            Name = "Sleep";
            StatusAlignment = UsableBase.UsableAlignment.Negative;
        }

        public override void OnInflict()
        {

        }

        protected override void OnEnd()
        {

        }

        protected override void OnTurnStart()
        {
            IncrementTurns();

            //End the turn because the Entity can't move while asleep
            if (IsFinished == false)
            {
                Debug.Log($"{Entity.Name} is asleep and can't move! Turn: {TurnsPassed - 1}");
                Entity.EndTurn();
            }
        }

        protected override void OnTurnEnd()
        {

        }

        public override StatusEffect Copy()
        {
            return new Sleep(Turns);
        }
    }
}
