using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// A special StatusEffect received when an entity performs the "Defend" command
    /// </summary>
    public sealed class BigDefenseStatus : StatusEffect
    {
        public BigDefenseStatus() : base(1)
        {
            Name = "BigDefense";
        }

        public override void OnInflict()
        {
            //Grant the entity 50% more defense
            Entity.AddStatModifier(StatModifiers.StatModTypes.Defense, 0, .5f);
            Entity.AddStatModifier(StatModifiers.StatModTypes.MagicDef, 0, .5f);
        }

        protected override void OnEnd()
        {
            Entity.RemoveStatModifierPercentage(StatModifiers.StatModTypes.Defense, .5f);
            Entity.RemoveStatModifierPercentage(StatModifiers.StatModTypes.MagicDef, .5f);
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
            return new BigDefenseStatus();
        }
    }
}
