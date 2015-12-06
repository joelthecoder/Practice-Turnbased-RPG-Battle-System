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
using static RPG_Battle_Test.BattlePlayer.Characters;

namespace RPG_Battle_Test
{
    public class BattlePlayer : BattleEntity
    {
        public enum Characters
        {
            None, CecilK, CecilP
        }

        public Characters Character { get; private set; } = Characters.None;

        public BattlePlayer(Characters character)
        {
            EntityType = EntityTypes.Player;

            IntRect rect = new IntRect(5, 55, 16, 24);

            switch (character)
            {
                case CecilK:
                    Name = "CecilK";
                    Speed = 3;
                    MaxHP = 20;
                    MaxMP = 0;
                    Attack = 6;
                    Defense = 2;
                    break;
                case CecilP:
                    Name = "CecilP";
                    Speed = 1;
                    MaxHP = 15;
                    MaxMP = 10;
                    Attack = 4;
                    MagicAtk = 3;
                    MagicDef = 2;
                    rect = new IntRect(7, 44, 16, 24);
                    break;
            }

            CurHP = MaxHP;
            CurMP = MaxMP;

            EntitySprite = Helper.CreateSprite(new Texture(Constants.ContentPath + Name + ".png"), false, rect);
            EntitySprite.Position = new Vector2f(GameCore.GameWindow.Size.X - GameCore.GameWindow.Size.X / 4f, GameCore.GameWindow.Size.Y / 2f);
            EntitySprite.Scale *= 3f;
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            if (Input.PressedKey(Keyboard.Key.A))
            {
                EndTurn();
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
