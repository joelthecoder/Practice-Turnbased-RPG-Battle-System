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
    public sealed class CecilK : BattlePlayer
    {
        public CecilK()
        {
            Name = "CecilK";
            Speed = 3;
            MaxHP = 30;
            MaxMP = 9;
            Attack = 7;
            Defense = 3;

            CurHP = MaxHP;
            CurMP = MaxMP;

            EntitySprite = Helper.CreateSprite(new Texture(Globals.ContentPath + "CecilK.png"), false, new IntRect(5, 55, 16, 24));
            EntitySprite.Scale *= 3f;

            LearnSpell("Haste1");
            LearnSpell("Sleep1");
            LearnSpell("Fast1");
        }
    }
}
