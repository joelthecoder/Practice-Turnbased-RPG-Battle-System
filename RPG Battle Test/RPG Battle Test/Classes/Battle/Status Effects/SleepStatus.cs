using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Immobilizes an Entity for a specified number of turns.
    /// This ends an Entity's turn when it starts if the effect has not worn off yet
    /// </summary>
    public class SleepStatus : StatusEffect
    {
        public SleepStatus(int turns) : base(turns)
        {
            Name = "Sleep";
        }

        public override void OnInflict()
        {

        }

        protected override void OnEnd()
        {

        }

        protected override void OnTurnStart()
        {
            //End the turn because the Entity can't move while asleep
            Debug.Log($"{Entity.Name} is asleep and can't move! Turn: {TurnsPassed}");
            Entity.ModifyNumActions(0);
        }

        protected override void OnTurnEnd()
        {
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new SleepStatus(Turns);
        }
    }
}
