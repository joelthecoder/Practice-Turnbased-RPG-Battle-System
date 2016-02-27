using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The Haste status effect. An entity's Speed is increased by a specified amount
    /// </summary>
    public class HasteStatus : StatusEffect
    {
        private int AmountIncrease = 0;

        public HasteStatus(int turns, int amountIncrease) : base(turns)
        {
            Name = "Haste";
            AmountIncrease = amountIncrease;
        }

        public override void OnInflict()
        {
            Entity.AddStatModifier(StatModifiers.StatModTypes.Speed, AmountIncrease, 0f);
        }

        protected override void OnEnd()
        {
            Entity.RemoveStatModifierAmount(StatModifiers.StatModTypes.Speed, AmountIncrease);
        }

        protected override void OnTurnStart()
        {
            IncrementTurns();
        }

        protected override void OnTurnEnd()
        {
            
        }

        public override StatusEffect Copy()
        {
            return new HasteStatus(Turns, AmountIncrease);
        }
    }
}
