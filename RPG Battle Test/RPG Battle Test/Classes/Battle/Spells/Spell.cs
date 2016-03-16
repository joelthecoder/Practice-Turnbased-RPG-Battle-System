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
    /// Magic spells used by entities. They are enhanced by an entity's MagicAtk and resisted by an entity's MagicDef.
    /// Spells can have a variety of primary or secondary effects, including healing, status, increased resistance, or more
    /// </summary>
    public class Spell : UsableBase, IUsable
    {
        /// <summary>
        /// The global Spell table
        /// </summary>
        private static readonly Dictionary<string, Spell> SpellTable = null;

        /// <summary>
        /// The amount of MP it costs to cast the spell
        /// </summary>
        public int MPCost { get; protected set; } = 2;

        /// <summary>
        /// The number of turns it takes to cast this spell. A value of 0 means the spell is cast on the same turn it is used
        /// </summary>
        public uint CastTurns { get; protected set; } = 0;

        static Spell()
        {
            SpellTable = new Dictionary<string, Spell>()
            {
                { "Demi1", new Spell("Demi1", 3, false, 0, UsableAlignment.Negative, new EntityDamageEffect("Demi1", 3, Globals.DamageTypes.Magic, Globals.Elements.Poison, new PoisonStatus(2), 50f, true)) },
                { "Cure1", new Spell("Cure1", 2, false, 0, UsableAlignment.Positive, new EntityHealEffect("Cure1", 10, 0)) },
                { "Ultima", new Spell("Ultima", 4, true, 1, UsableAlignment.Negative, new EntityDamageEffect("Ultima", 4, Globals.DamageTypes.Magic, Globals.Elements.Neutral, true)) },
                { "Silence1", new Spell("Silence", 2, false, 0, UsableAlignment.Negative, new EntityStatusEffect("Silence1", new SilenceStatus(2), 100f)) },
                { "Haste1", new Spell("Haste1", 3, false, 0, UsableAlignment.Positive, new EntityStatusEffect("Haste1", new HasteStatus(3, 10), 100f)) },
                { "Sleep1", new Spell("Sleep1", 2, false, 0, UsableAlignment.Negative, new EntityStatusEffect("Sleep1", new SleepStatus(3), 100f)) },
                { "Esuna", new Spell("Esuna", 4, false, 0, UsableAlignment.Positive, new EntityHealEffect("Esuna", 0, 0, typeof(PoisonStatus), typeof(SilenceStatus))) },
                { "Fast1", new Spell("Fast1", 2, false, 0, UsableAlignment.Positive, new EntityStatusEffect("Fast1", new FastStatus(2, 2), 100f)) }
            };
        }

        /// <summary>
        /// Tells whether a Spell with a particular name exists in the global Spell table
        /// </summary>
        /// <param name="spellName">The name of the Spell to search for</param>
        /// <returns>true if a Spell with spellName exists, otherwise false</returns>
        public static bool SpellExists(string spellName) => SpellTable.ContainsKey(spellName);

        /// <summary>
        /// Gets a Spell in the global Spell table by name
        /// </summary>
        /// <param name="spellName">The name of the Spell to get</param>
        /// <returns>A deep copy of the Spell if found, otherwise null</returns>
        public static Spell GetSpell(string spellName)
        {
            if (SpellExists(spellName) == true)
            {
                return SpellTable[spellName].Copy();
            }
            
            return null;
        }

        public Spell(string name, int mpCost, bool multitarget, uint castTurns, UsableAlignment alignment, EntityEffect entityeffect)
            : this(name, mpCost, multitarget, castTurns, alignment, BattleManager.EntityFilterStates.Alive, entityeffect)
        {
            
        }

        public Spell(string name, int mpCost, bool multitarget, uint castTurns, UsableAlignment alignment, BattleManager.EntityFilterStates filterState, EntityEffect entityeffect)
            : base(name, multitarget, alignment, filterState, entityeffect)
        {
            MPCost = mpCost;
            AffectableType = AffectableTypes.Spell;

            CastTurns = castTurns;
        }

        /// <summary>
        /// What happens to the entities when the Spell is used on them
        /// </summary>
        /// <param name="User">The BattleEntity that used this Spell</param>
        /// <param name="Entities">The BattleEntities affected by the Spell</param>
        public void OnUse(BattleEntity User, params BattleEntity[] Entities)
        {
            if (Entityeffect == null)
            {
                Debug.LogError($"Spell {Name}'s EntityEffect is null!");
                return;
            }
            
            Entityeffect.UseEffect(new Globals.AffectableInfo(User, this), Entities);
        }

        /// <summary>
        /// Returns a new instance of this Spell with the same properties
        /// </summary>
        /// <returns>A deep copy of the Spell</returns>
        public Spell Copy()
        {
            return new Spell(Name, MPCost, MultiTarget, CastTurns, Alignment, FilterState, Entityeffect?.Copy());
        }
    }
}
