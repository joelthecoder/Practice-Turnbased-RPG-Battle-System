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

        /// <summary>
        /// Possible BattleActions for players to have.
        /// Every character has Attack, Item, Magic, and Run, but more can be obtained or specific to characters (Ex. Steal).
        /// </summary>
        public enum BattleActions
        {
            Attack, Item, Magic, Steal, Run
        }

        public delegate void SelectEntity(BattleEntity entity);

        public SelectEntity OnSelectEntity = null;

        /// <summary>
        /// The main battle menu
        /// </summary>
        public static BattleMenu MainBattleMenu = null;
        public static BattleMenu ItemMenu = null;

        public static Stack<BattleMenu> Menus = null;

        public static BattleMenu CurrentMenu => Menus.Peek();

        protected const float ArrowVerticalDist = 100f;

        public Characters Character { get; private set; } = Characters.None;
        protected Sprite Arrow = null;
        protected int? CurSelection = null;

        private List<BattleEntity> TargetList = null;

        public BattlePlayer(Characters character)
        {
            EntityType = EntityTypes.Player;
            
            IntRect rect = new IntRect(5, 55, 16, 24);

            switch (character)
            {
                case CecilK:
                    Name = "CecilK";
                    Speed = 3;
                    MaxHP = 30;
                    MaxMP = 0;
                    Attack = 7;
                    Defense = 3;
                    break;
                case CecilP:
                    Name = "CecilP";
                    Speed = 1;
                    MaxHP = 25;
                    MaxMP = 10;
                    Attack = 5;
                    Defense = 1;
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

            Arrow = Helper.CreateSprite(new Texture(Constants.ContentPath + "Arrow.png"), false);
        }

        /// <summary>
        /// Called when the battle is started
        /// </summary>
        public static void OnBattleStart()
        {
            Menus = new Stack<BattleMenu>();
            MainBattleMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), BattleMenu.MenuTypes.Vertical);
            ItemMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), BattleMenu.MenuTypes.Vertical, () => Menus.Pop());

            Menus.Push(MainBattleMenu);
        }

        public override void StartTurn()
        {
            base.StartTurn();
            BattleMenu.Active = true;

            //Set the basic options
            MainBattleMenu.SetOptions(new BattleMenu.MenuOption("Attack", AttackSelect), new BattleMenu.MenuOption("Item", ItemSelect));
            ItemMenu.OnOpen = PopulateItemList;
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleMenu.Active = false;
            while (Menus.Count > 1)
            {
                Menus.Pop();
            }
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();

            if (CurSelection.HasValue)
            {
                //Cancel selection
                if (Input.PressedKey(Keyboard.Key.X))
                {
                    CurSelection = null;
                    BattleManager.Instance.HeaderBox.SetText(Name + "'s turn!");
                    return;
                }
                else
                {
                    //Move up a selection
                    if (Input.PressedKey(Keyboard.Key.Up))
                    {
                        do CurSelection = Helper.Wrap(CurSelection.Value - 1, 0, TargetList.Count - 1);
                        while (TargetList[CurSelection.Value].IsDead == true);
                        Arrow.Position = new Vector2f(TargetList[CurSelection.Value].Position.X, TargetList[CurSelection.Value].Position.Y - ArrowVerticalDist);
                        BattleManager.Instance.HeaderBox.SetText("Attack " + TargetList[CurSelection.Value].Name + "?");
                    }
                    //Move down a selection
                    if (Input.PressedKey(Keyboard.Key.Down))
                    {
                        do CurSelection = Helper.Wrap(CurSelection.Value + 1, 0, TargetList.Count - 1);
                        while (TargetList[CurSelection.Value].IsDead == true);
                        Arrow.Position = new Vector2f(TargetList[CurSelection.Value].Position.X, TargetList[CurSelection.Value].Position.Y - ArrowVerticalDist);
                        BattleManager.Instance.HeaderBox.SetText("Attack " + TargetList[CurSelection.Value].Name + "?");
                    }
                }
            }

            if (Input.PressedKey(Keyboard.Key.Z) && CurSelection.HasValue == true)
            {
                OnSelectEntity?.Invoke(TargetList[CurSelection.Value]);
                CurSelection = null;
                TargetList = null;
                EndTurn();
                return;
            }

            if (CurSelection.HasValue == false)
                CurrentMenu.Update();
        }

        protected void AttackSelect()
        {
            TargetList = BattleManager.Instance.Enemies.ConvertAll<BattleEntity>(entity => (BattleEntity)entity);

            for (int i = 0; i < TargetList.Count; i++)
            {
                BattleEntity target = TargetList[i];
                if (target.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(target.Position.X, target.Position.Y - ArrowVerticalDist);
                    BattleManager.Instance.HeaderBox.SetText("Attack " + TargetList[i].Name + "?");
                    break;
                }
            }

            OnSelectEntity = AttackEntity;
        }
        
        protected void ItemSelection(Item item)
        {
            if (item.IsOfType(Item.ItemTypes.Damage))
            {
                TargetList = BattleManager.Instance.Enemies.ConvertAll<BattleEntity>(entity => (BattleEntity)entity);
            }
            else if (item.IsOfType(Item.ItemTypes.Heal))
            {
                TargetList = BattleManager.Instance.Players.ConvertAll<BattleEntity>(entity => (BattleEntity)entity);
            }

            for (int i = 0; i < TargetList.Count; i++)
            {
                BattleEntity target = TargetList[i];
                if (target.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(target.Position.X, target.Position.Y - ArrowVerticalDist);
                    BattleManager.Instance.HeaderBox.SetText("Attack " + TargetList[i].Name + "?");
                    break;
                }
            }

            OnSelectEntity = item.Use;
        }

        protected void ItemSelect()
        {
            Menus.Push(ItemMenu);
            if (Menus.Peek().OnOpen != null)
                Menus.Peek().OnOpen();
        }

        protected void PopulateItemList()
        {
            Dictionary<Item, int> items = BattleManager.Instance.PartyInventory.GetInventory();
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();
            
            foreach (KeyValuePair<Item, int> pair in items)
            {
                options.Add(new BattleMenu.MenuOption($"{pair.Key.Name} x{pair.Value}", () => ItemSelection(pair.Key)));
            }

            ItemMenu.SetOptions(options);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (CurSelection.HasValue)
            {
                GameCore.spriteSorter.Add(Arrow, Constants.BASE_UI_LAYER + .03f);
                //Arrow.Draw(GameCore.GameWindow, RenderStates.Default);
            }
        }
    }
}
