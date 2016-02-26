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
    public sealed class CecilP : BattlePlayer
    {
        public CecilP()
        {
            Name = "CecilP";
            Speed = 1;
            MaxHP = 25;
            MaxMP = 10;
            Attack = 5;
            Defense = 1;
            MagicAtk = 3;
            MagicDef = 2;

            CurHP = MaxHP;
            CurMP = MaxMP;

            EntitySprite = Helper.CreateSprite(new Texture(Globals.ContentPath + "CecilP.png"), false, new IntRect(7, 44, 16, 24));
            EntitySprite.Scale *= 3f;

            LearnSpell("Demi1");
            LearnSpell("Cure1");
            LearnSpell("Ultima");
            LearnSpell("Silence1");
            LearnSpell("Esuna");
        }
    }
}
