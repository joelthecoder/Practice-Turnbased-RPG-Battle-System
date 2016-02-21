using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base class for EntityEffects, which affect BattleEntities in some way.
    /// They are attached to Items and Spells, which act as containers for EntityEffects
    /// </summary>
    public abstract class EntityEffect : AffectableBase
    {
        protected EntityEffect(string name) : base(name)
        {
            
        }

        /// <summary>
        /// What happens to the entities when the EntityEffect is used on them
        /// </summary>
        /// <param name="affectableInfo">Contains the Entity and how it used the EntityEffect</param>
        /// <param name="Entities">The entities to use the EntityEffect on</param>
        public abstract void UseEffect(AffectableInfo affectableInfo, params BattleEntity[] Entities);

        /// <summary>
        /// Copies the EntityEffect's values and returns a new instance
        /// </summary>
        /// <returns>A new instance of the EntityEffect with the same values</returns>
        public abstract EntityEffect Copy();
    }
}
