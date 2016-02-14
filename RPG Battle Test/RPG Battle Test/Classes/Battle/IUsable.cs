using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Interface for objects that implement the OnUse() method
    /// </summary>
    public interface IUsable
    {
        void OnUse(BattleEntity User, params BattleEntity[] Entities);
    }
}
