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

        /// <summary>
        /// Possible BattleActions for players to have.
        /// Every character has Attack, Item, Magic, and Run, but more can be obtained or specific to characters (Ex. Steal).
        /// </summary>
        public enum BattleActions
        {
            Attack, Item, Magic, Steal, Run
        }

        protected const float ArrowVerticalDist = 100f;

        public Characters Character { get; private set; } = Characters.None;
        protected Sprite Arrow = null;
        protected int? CurSelection = null;

        private List<BattleEntity> TargetList = null;

        public Animation AttackAnim = null;

        public BattlePlayer()
        {
            EntityType = EntityTypes.Player;

            Arrow = Helper.CreateSprite(AssetManager.SelectionArrow, false);
            //AttackAnim = new LoopAnimation(LoopAnimation.CONTINOUS_LOOP, new Texture(Constants.ContentPath + "CecilK.png"), 5f, 
            //new IntRect(5, 83, 16, 23), new IntRect(25, 82, 16, 24), new IntRect(45, 82, 16, 24));
        }

        /// <summary>
        /// Called when the battle is started
        /// </summary>
        public static void OnBattleStart()
        {
            BattleMenu mainBattleMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(150, 35), 
                                                       BattleMenu.MenuTypes.Vertical);
            mainBattleMenu.CanBackOut = false;
            mainBattleMenu.Active = false;

            BattleUIManager.Instance.PushInputMenu(mainBattleMenu);
        }

        protected override void OnTurnStarted()
        {
            base.OnTurnStarted();

            if (PreviousCommand is DefendCommand)
            {
                StatModifications.RemoveModifierWithAmount(StatModifiers.StatModTypes.Defense, 1);//RemoveModifierWithPercentage(StatModifiers.StatModTypes.Defense, .5f);
                StatModifications.RemoveModifierWithAmount(StatModifiers.StatModTypes.MagicDef, 1);//RemoveModifierWithPercentage(StatModifiers.StatModTypes.MagicDef, .5f);
            }

            BattleUIManager.Instance.GetInputMenu().Active = true;

            //Set the basic options
            BattleUIManager.Instance.GetInputMenu().SetOptions(new BattleMenu.MenuOption("Attack", AttackSelect), new BattleMenu.MenuOption("Defend", DefendSelect),
            new BattleMenu.MenuOption("Item", ItemSelect), new BattleMenu.MenuOption("Magic", SpellSelect));
        }

        protected override void OnTurnEnded()
        {
            base.OnTurnEnded();
            CurSelection = null;
            TargetList = null;
            while (BattleUIManager.Instance.InputMenuCount() > 1)
            {
                BattleUIManager.Instance.PopInputMenu();
            }

            BattleUIManager.Instance.GetInputMenu().Active = false;
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
                    BattleUIManager.Instance.SetHeaderText(Name + "'s turn!");
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
                        BattleUIManager.Instance.SetHeaderText("Attack " + TargetList[CurSelection.Value].Name + "?");
                    }
                    //Move down a selection
                    if (Input.PressedKey(Keyboard.Key.Down))
                    {
                        do CurSelection = Helper.Wrap(CurSelection.Value + 1, 0, TargetList.Count - 1);
                        while (TargetList[CurSelection.Value].IsDead == true);
                        Arrow.Position = new Vector2f(TargetList[CurSelection.Value].Position.X, TargetList[CurSelection.Value].Position.Y - ArrowVerticalDist);
                        BattleUIManager.Instance.SetHeaderText("Attack " + TargetList[CurSelection.Value].Name + "?");
                    }
                }
            }

            if (Input.PressedKey(Keyboard.Key.Z) && CurSelection.HasValue == true)
            {
                CurrentCommand.PerformAction(this, TargetList[CurSelection.Value]);
                EndTurn();
                return;
            }

            if (CurSelection.HasValue == false)
                BattleUIManager.Instance.GetInputMenu().Update();
        }

        protected void AttackSelect()
        {
            TargetList = BattleManager.Instance.Enemies;

            for (int i = 0; i < TargetList.Count; i++)
            {
                BattleEntity target = TargetList[i];
                if (target.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(target.Position.X, target.Position.Y - ArrowVerticalDist);
                    BattleUIManager.Instance.SetHeaderText("Attack " + TargetList[i].Name + "?");
                    break;
                }
            }

            CurrentCommand = new AttackCommand();
        }
        
        protected void ItemSelection(Item item)
        {
            if (item.IsOfType(Item.ItemTypes.Damage) || item.IsOfType(Item.ItemTypes.NegativeStatus))
            {
                TargetList = BattleManager.Instance.Enemies;
            }
            else if (item.IsOfType(Item.ItemTypes.Heal) || item.IsOfType(Item.ItemTypes.PositiveStatus))
            {
                TargetList = BattleManager.Instance.Players;
            }

            for (int i = 0; i < TargetList.Count; i++)
            {
                BattleEntity target = TargetList[i];
                if (target.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(target.Position.X, target.Position.Y - ArrowVerticalDist);
                    BattleUIManager.Instance.SetHeaderText("Attack " + TargetList[i].Name + "?");
                    break;
                }
            }

            CurrentCommand = new ItemCommand(item);
        }

        protected void SpellSelection(Spell spell)
        {
            if (CurMP < spell.MPCost)
            {
                Debug.Log($"{Name} has {CurMP}MP but requires {spell.MPCost}MP to cast {spell.Name}!");
                return;
            }
            
            if (spell.SpellType == Spell.SpellTypes.Negative)
            {
                TargetList = BattleManager.Instance.Enemies;
            }
            else if (spell.SpellType == Spell.SpellTypes.Positive)
            {
                TargetList = BattleManager.Instance.Players;
            }
            else
            {
                TargetList = BattleManager.Instance.EntityOrder;
            }

            for (int i = 0; i < TargetList.Count; i++)
            {
                BattleEntity target = TargetList[i];
                if (target.IsDead == false)
                {
                    CurSelection = i;
                    Arrow.Position = new Vector2f(target.Position.X, target.Position.Y - ArrowVerticalDist);
                    BattleUIManager.Instance.SetHeaderText($"Use {spell.Name} on " + TargetList[i].Name + "?");
                    break;
                }
            }

            CurrentCommand = new SpellCommand(spell);
        }

        protected void DefendSelect()
        {
            CurrentCommand = new DefendCommand();
            CurrentCommand.PerformAction(this, null);
            EndTurn();
        }

        protected void ItemSelect()
        {
            BattleMenu itemMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40),
                                                 BattleMenu.MenuTypes.Vertical);
            itemMenu.OnOpen = PopulateItemList;
            BattleUIManager.Instance.PushInputMenu(itemMenu);
        }

        protected void SpellSelect()
        {
            BattleMenu spellMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), 
                                                  BattleMenu.MenuTypes.Vertical);
            spellMenu.OnOpen = PopulateSpellList;
            BattleUIManager.Instance.PushInputMenu(spellMenu);
        }

        protected void PopulateItemList()
        {
            Dictionary<Item, int> items = BattleManager.Instance.PartyInventory.GetInventory();
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();
            
            foreach (KeyValuePair<Item, int> pair in items)
            {
                options.Add(new BattleMenu.MenuOption($"{pair.Key.Name} x{pair.Value}", () => ItemSelection(pair.Key)));
            }

            BattleUIManager.Instance.GetInputMenu().SetOptions(options);
        }

        protected void PopulateSpellList()
        {
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();

            DamageSpell Poison = new DamageSpell("Demi1", 3, 3, Globals.DamageTypes.Magic, Globals.Elements.Poison, new Poison(2), 50f);
            HealingSpell Cure = new HealingSpell("Cure1", 2, false, 10, 0);

            options.Add(new BattleMenu.MenuOption($"{Poison.Name}", () => SpellSelection(Poison)));
            options.Add(new BattleMenu.MenuOption($"{Cure.Name}", () => SpellSelection(Cure)));

            BattleUIManager.Instance.GetInputMenu().SetOptions(options);
        }

        public override void Update()
        {
            base.Update();
            AttackAnim?.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (CurSelection.HasValue)
            {
                GameCore.spriteSorter.Add(Arrow, Constants.BASE_UI_LAYER + .03f);
            }

            //AttackAnim.Position = new Vector2f(500f, 400f);
            AttackAnim?.Draw(40f);
        }
    }
}
