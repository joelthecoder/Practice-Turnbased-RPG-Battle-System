using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The Fast status. It allows a BattleEntity to move more than once per turn
    /// </summary>
    public sealed class FastStatus : StatusEffect
    {
        public uint NumActions = 2;

        public FastStatus(int turns, uint numActions) : base(turns)
        {
            Name = "Fast";
            NumActions = Helper.Clamp(numActions, Globals.MIN_ENTITY_TURNS, Globals.MAX_ENTITY_TURNS);
        }

        public override void OnInflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnTurnStart()
        {
            Entity.ModifyNumActions(NumActions);
        }

        protected override void OnTurnEnd()
        {
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new FastStatus(Turns, NumActions);
        }
    }
}
