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
    /// The base class for anything that can affect a BattleEntity.
    /// This includes, but isn't limited to, attacks, items, spells, and status effects
    /// </summary>
    public abstract class AffectableBase
    {
        public enum AffectableTypes
        {
            None, Attack, Item, Spell, Status
        }

        /// <summary>
        /// The name of the Affectable
        /// </summary>
        public string Name { get; protected set; } = "Affectable";

        /// <summary>
        /// The type of Affectable object this is
        /// </summary>
        public AffectableTypes AffectableType { get; protected set; } = AffectableTypes.None;

        protected AffectableBase()
        {
        
        }

        protected AffectableBase(string name)
        {
            Name = name;
        }
    }
}
