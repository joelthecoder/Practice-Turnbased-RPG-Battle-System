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

        static Spell()
        {
            SpellTable = new Dictionary<string, Spell>()
            {
                { "Demi1", new Spell("Demi1", 3, false, UsableAlignment.Negative, new EntityDamageEffect("Demi1", 3, Globals.DamageTypes.Magic, Globals.Elements.Poison, new Poison(2), 50f, true)) },
                { "Cure1", new Spell("Cure1", 2, false, UsableAlignment.Positive, new EntityHealEffect("Cure1", 10, 0)) },
                { "Ultima", new Spell("Ultima", 4, true, UsableAlignment.Negative, new EntityDamageEffect("Ultima", 4, Globals.DamageTypes.Magic, Globals.Elements.Neutral, true)) },
                { "Silence1", new Spell("Silence", 2, false, UsableAlignment.Negative, new EntityStatusEffect("Silence1", new Silence(2), 100f)) },
                { "Haste1", new Spell("Haste1", 3, false, UsableAlignment.Positive, new EntityStatusEffect("Haste1", new Haste(3, 10), 100f)) },
                { "Sleep1", new Spell("Sleep1", 2, false, UsableAlignment.Negative, new EntityStatusEffect("Sleep1", new Sleep(3), 100f)) },
                { "Esuna", new Spell("Esuna", 4, false, UsableAlignment.Positive, new EntityHealEffect("Esuna", 0, 0, typeof(Poison))) }
            };
        }

        public static bool SpellExists(string spellName) => SpellTable.ContainsKey(spellName);

        public static Spell GetSpell(string spellName)
        {
            if (SpellExists(spellName) == true)
            {
                return SpellTable[spellName];
            }
            
            return null;
        }

        public Spell(string name, int mpCost, bool multitarget, UsableAlignment alignment, EntityEffect entityeffect)
            : base(name, multitarget, alignment, BattleManager.EntityFilterStates.Alive, entityeffect)
        {
            MPCost = mpCost;
        }

        public Spell(string name, int mpCost, bool multitarget, UsableAlignment alignment, BattleManager.EntityFilterStates filterState, EntityEffect entityeffect)
            : base(name, multitarget, alignment, filterState, entityeffect)
        {
            MPCost = mpCost;
        }

        /// <summary>
        /// What happens to the entities when the Spell is used on them
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Entities"></param>
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
        /// Copies the Spell's properties and returns a new instance
        /// </summary>
        /// <returns>A new instance of the Spell with the same properties</returns>
        public Spell Copy()
        {
            return new Spell(Name, MPCost, MultiTarget, Alignment, FilterState, Entityeffect?.Copy());
        }
    }
}
