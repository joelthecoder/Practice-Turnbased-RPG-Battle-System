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
    public class BattleEnemy : BattleEntity
    {
        protected const float WaitTime = 1f;
        protected float PrevWait = 0f;

        public BattleEnemy()
        {
            EntityType = EntityTypes.Enemy;

            IntRect rect = new IntRect(1456, 33, 31, 32);

            if (new Random().Next(0, 2) == 0)
            {
                Name = "Gnome";
                rect = new IntRect(1456, 33, 31, 32);
                Speed = 4;
            }
            else
            {
                Name = "Gargoyle";
                rect = new IntRect(990, 346, 62, 48);
                Speed = 2;
            }

            EntitySprite = Helper.CreateSprite(new Texture(Constants.ContentPath + "Enemies.png"), true, rect);
            EntitySprite.Scale *= 3f;
        }

        public override void StartTurn()
        {
            PrevWait = GameCore.ActiveSeconds + WaitTime;
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
            if (GameCore.ActiveSeconds >= PrevWait)
            {
                AttackEntity(BattleManager.Instance.SelectRandomEntity(EntityTypes.Player));
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
