using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Temporarily prevents use of all Magic
    /// </summary>
    public sealed class SilenceStatus : StatusEffect
    {
        public SilenceStatus(int turns) : base(turns)
        {
            Name = "Silence";
        }

        public override void OnInflict()
        {
            //Disable the command for using magic
            Entity.ToggleCommand(BattleCommand.BattleActions.Magic, true);
        }

        protected override void OnEnd()
        {
            //Reenable the command for using magic
            Entity.ToggleCommand(BattleCommand.BattleActions.Magic, false);
        }

        protected override void OnTurnStart()
        {
            
        }

        protected override void OnTurnEnd()
        {
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new SilenceStatus(Turns);
        }
    }
}
