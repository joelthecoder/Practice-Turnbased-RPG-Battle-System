﻿using System;
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
    public sealed class PoisonStatus : StatusEffect
    {
        public PoisonStatus(int turns) : base(turns)
        {
            Name = "Poison";
        }

        public override void OnInflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnTurnStart()
        {
            
        }

        protected override void OnTurnEnd()
        {
            Entity.TakeDamage(new Globals.AffectableInfo(Afflicter, this), (int)(Entity.MaxHP / 10f), Globals.DamageTypes.None, Globals.Elements.Poison);
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new PoisonStatus(Turns);
        }
    }
}
