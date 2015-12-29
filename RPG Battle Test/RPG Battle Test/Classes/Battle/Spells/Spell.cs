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
    public abstract class Spell
    {
        /// <summary>
        /// Types of spells
        /// </summary>
        public enum SpellTypes
        {
            Neutral, Positive, Negative
        }

        /// <summary>
        /// The name of the spell
        /// </summary>
        public string Name { get; protected set; } = "Spell";

        /// <summary>
        /// Whether the spell is multi-target or not.
        /// If false only one target can be selected, otherwise all targets will be selected
        /// </summary>
        public bool MultiTarget { get; protected set; } = false;

        /// <summary>
        /// The type the spell is classified as
        /// </summary>
        public SpellTypes SpellType { get; protected set; } = SpellTypes.Neutral;

        /// <summary>
        /// The amount of MP it costs to cast the spell
        /// </summary>
        public int MPCost { get; protected set; } = 2;

        protected Spell(string name, int mpCost)
        {
            Name = name;
            MPCost = mpCost;
        }

        protected Spell(string name, int mpCost, bool multitarget) : this(name, mpCost)
        {
            MultiTarget = multitarget;
        }

        /// <summary>
        /// What happens to the entity when the Spell is used on it
        /// </summary>
        /// <param name="entities"></param>
        public abstract void OnUse(params BattleEntity[] entities);
    }
}
