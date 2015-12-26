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
        public string Name = "Spell";

        /// <summary>
        /// The type the spell is classified as
        /// </summary>
        public SpellTypes SpellType { get; protected set; } = SpellTypes.Neutral;

        protected Spell(string name)
        {
            Name = name;
        }

        /// <summary>
        /// What happens to the entity when the Spell is used on it
        /// </summary>
        /// <param name="entity"></param>
        public abstract void OnUse(BattleEntity entity);
    }
}
