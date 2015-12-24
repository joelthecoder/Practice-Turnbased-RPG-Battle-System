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
    /// The Poison status effect. Entities lose 10% of their HP, rounding down, each time they end their turn
    /// </summary>
    public sealed class Poison : StatusEffect
    {
        public Poison(BattleEntity entity, int turns) : base(entity, turns)
        {
            Name = "Poison";
            StatusType = StatusTypes.Negative;
        }

        public override void OnInflict()
        {
            
        }

        public override void OnEnd()
        {
            
        }

        public override void OnTurnStart()
        {
            
        }

        public override void OnTurnEnd()
        {
            IncrementTurns();
            Entity.TakeDamage((int)(Entity.MaxHP / 10f), Globals.DamageTypes.None, Globals.Elements.Poison);
        }
    }
}
