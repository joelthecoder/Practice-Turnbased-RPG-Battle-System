using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The base class for EntityEffects, which affect BattleEntities in some way.
    /// They are attached to Items and Spells, which act as containers for EntityEffects
    /// </summary>
    public abstract class EntityEffect : AffectableBase, IUsable
    {
        protected EntityEffect(string name) : base(name)
        {
            
        }

        public abstract void OnUse(BattleEntity User, params BattleEntity[] Entities);

        public abstract EntityEffect Copy();
    }
}
