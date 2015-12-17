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
    //The base interface for commands available for use in battle
    //These include, but are not limited to, Attack, Defend, Magic, Item, Run
    public interface BattleCommand
    {
        //Perform a command
        void Perform();
    }
}
