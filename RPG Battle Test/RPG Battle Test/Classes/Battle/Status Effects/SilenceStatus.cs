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
    /*NOTE: This isn't the same as indicating that Magic is disabled; it's just a simple entity-independent solution that doesn't
      require any changes in BattleEntity*/
    public sealed class SilenceStatus : StatusEffect
    {
        private Dictionary<string, Spell> EntitySpells = null;

        public SilenceStatus(int turns) : base(turns)
        {
            Name = "Silence";
        }

        public override void OnInflict()
        {
            //Store all the entity's spells and make it forget them all
            /*NOTE: Remember that this is battle-only, so if an entire game were implemented, this would NOT permanently
            remove the Entity's spells if something went wrong; they would just be removed for this battle*/
            EntitySpells = Entity.GetAllSpells();
            Entity.ForgetAllSpells();
        }

        protected override void OnEnd()
        {
            //Relearn all the Spells when the status is finished
            foreach (KeyValuePair<string, Spell> spell in EntitySpells)
            {
                Entity.LearnSpell(spell.Key);
            }
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
