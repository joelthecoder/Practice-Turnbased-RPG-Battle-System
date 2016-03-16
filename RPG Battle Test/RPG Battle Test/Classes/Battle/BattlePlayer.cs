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
    public abstract class BattlePlayer : BattleEntity
    {
        public enum Characters
        {
            None, CecilK, CecilP
        }

        protected const float ArrowVerticalDist = 100f;

        public Characters Character { get; private set; } = Characters.None;

        private List<BattleEntity> TargetList = null;

        public Animation AttackAnim = null;

        public BattlePlayer()
        {
            EntityType = EntityTypes.Player;

            KnownCommands.Add(BattleCommand.BattleActions.Attack, new AttackCommand());
            KnownCommands.Add(BattleCommand.BattleActions.Defend, new DefendCommand());
            KnownCommands.Add(BattleCommand.BattleActions.Item, new ItemCommand());
            KnownCommands.Add(BattleCommand.BattleActions.Magic, new SpellCommand());
            //AttackAnim = new LoopAnimation(LoopAnimation.CONTINOUS_LOOP, new Texture(Constants.ContentPath + "CecilK.png"), 5f, 
            //new IntRect(5, 83, 16, 23), new IntRect(25, 82, 16, 24), new IntRect(45, 82, 16, 24));
        }

        /// <summary>
        /// Called when the battle is started
        /// </summary>
        public static void BattleStart()
        {
            BattleMenu mainBattleMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(150, 35), 
                                                       BattleMenu.GridTypes.Vertical);
            mainBattleMenu.CanBackOut = false;
            mainBattleMenu.Active = false;

            BattleUIManager.Instance.PushInputMenu(mainBattleMenu);
        }

        protected override void OnTurnStarted()
        {
            base.OnTurnStarted();

            BattleUIManager.Instance.TargetMenu.TargetSelectionEvent += UseCommand;

            BattleUIManager.Instance.GetInputMenu().Active = true;

            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();

            //Set the basic options
            foreach (KeyValuePair<BattleCommand.BattleActions, BattleCommand> command in KnownCommands)
            {
                Color optioncolor = Color.Black;
                BattleMenu.MenuOption.OptionSelect commandselect = null;

                if (command.Value.Disabled == false)
                {
                    optioncolor = Color.White;
                    commandselect = () => command.Value.OnSelect(this);
                }

                options.Add(new BattleMenu.MenuOption(command.Value.Name, commandselect, optioncolor));
            }

            BattleUIManager.Instance.GetInputMenu().SetElements(options);
        }

        protected override void OnTurnEnded()
        {
            base.OnTurnEnded();

            BattleUIManager.Instance.TargetMenu.TargetSelectionEvent -= UseCommand;

            while (BattleUIManager.Instance.InputMenuCount() > 1)
            {
                BattleUIManager.Instance.PopInputMenu();
            }

            BattleUIManager.Instance.GetInputMenu().Active = false;
        }

        public override void Dispose()
        {
            base.Dispose();
            BattleUIManager.Instance.TargetMenu.TargetSelectionEvent -= UseCommand;
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();

            if (BattleUIManager.Instance.TargetMenu.Active == false)
                BattleUIManager.Instance.GetInputMenu().Update();
            else BattleUIManager.Instance.TargetMenu.Update();
        }

        public void SetCommand(BattleCommand command)
        {
            CurrentCommand = command;
        }

        public void UseCommand(params BattleEntity[] victims)
        {
            CurrentCommand.PerformAction(this, victims);
            EndTurn();
        }

        public override void Update()
        {
            base.Update();
            AttackAnim?.Update();
        }

        public override void Draw()
        {
            base.Draw();

            //AttackAnim.Position = new Vector2f(500f, 400f);
            AttackAnim?.Draw(40f);
        }
    }
}
