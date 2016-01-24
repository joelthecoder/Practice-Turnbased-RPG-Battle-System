﻿using System;
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

        private List<BattleEntity> TargetList = null;

        public Animation AttackAnim = null;

        public BattlePlayer()
        {
            EntityType = EntityTypes.Player;

            //AttackAnim = new LoopAnimation(LoopAnimation.CONTINOUS_LOOP, new Texture(Constants.ContentPath + "CecilK.png"), 5f, 
            //new IntRect(5, 83, 16, 23), new IntRect(25, 82, 16, 24), new IntRect(45, 82, 16, 24));
        }

        /// <summary>
        /// Called when the battle is started
        /// </summary>
        public static void OnBattleStart()
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

            if (PreviousCommand is DefendCommand)
            {
                StatModifications.RemoveModifierWithAmount(StatModifiers.StatModTypes.Defense, 1);//RemoveModifierWithPercentage(StatModifiers.StatModTypes.Defense, .5f);
                StatModifications.RemoveModifierWithAmount(StatModifiers.StatModTypes.MagicDef, 1);//RemoveModifierWithPercentage(StatModifiers.StatModTypes.MagicDef, .5f);
            }

            BattleUIManager.Instance.TargetMenu.TargetSelectionEvent += UseCommand;

            BattleUIManager.Instance.GetInputMenu().Active = true;

            //Set the basic options
            BattleUIManager.Instance.GetInputMenu().SetElements(new BattleMenu.MenuOption("Attack", AttackSelect), new BattleMenu.MenuOption("Defend", DefendSelect),
            new BattleMenu.MenuOption("Item", ItemSelect), new BattleMenu.MenuOption("Magic", SpellSelect));
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

            /*if (CurSelection.HasValue)
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
            }*/

            if (BattleUIManager.Instance.TargetMenu.Active == false)
                BattleUIManager.Instance.GetInputMenu().Update();
            else BattleUIManager.Instance.TargetMenu.Update();
        }

        protected void AttackSelect()
        {
            BattleUIManager.Instance.StartTargetSelection(BattleManager.Instance.GetEntityGroup(EntityTypes.Enemy, true), false);

            CurrentCommand = new AttackCommand();
        }

        protected void ItemSelection(Item item)
        {
            EntityTypes entityType = EntityTypes.None;
            bool multiTarget = false;
            bool filterDead = true;

            if (item.IsOfType(Item.ItemTypes.Damage) || item.IsOfType(Item.ItemTypes.NegativeStatus))
            {
                entityType = EntityTypes.Enemy;
            }
            else if (item.IsOfType(Item.ItemTypes.Heal) || item.IsOfType(Item.ItemTypes.PositiveStatus))
            {
                entityType = EntityTypes.Player;
            }

            BattleUIManager.Instance.StartTargetSelection(BattleManager.Instance.GetEntityGroup(entityType, filterDead), multiTarget);

            CurrentCommand = new ItemCommand(item);
        }

        protected void SpellSelection(Spell spell)
        {
            if (CurMP < spell.MPCost)
            {
                Debug.Log($"{Name} has {CurMP}MP but requires {spell.MPCost}MP to cast {spell.Name}!");
                return;
            }

            EntityTypes entityType = EntityTypes.None;
            bool multiTarget = spell.MultiTarget;
            bool filterDead = true;

            if (spell.SpellType == Spell.SpellTypes.Negative)
            {
                entityType = EntityTypes.Enemy;
            }
            else if (spell.SpellType == Spell.SpellTypes.Positive)
            {
                entityType = EntityTypes.Player;
            }

            BattleUIManager.Instance.StartTargetSelection(BattleManager.Instance.GetEntityGroup(entityType, filterDead), multiTarget);

            CurrentCommand = new SpellCommand(spell);
        }

        protected void DefendSelect()
        {
            BattleUIManager.Instance.StartTargetSelection(new List<BattleEntity>() { this }, false);

            CurrentCommand = new DefendCommand();
        }

        protected void ItemSelect()
        {
            BattleMenu itemMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40),
                                                 BattleMenu.GridTypes.Vertical);
            itemMenu.OnOpen = PopulateItemList;
            BattleUIManager.Instance.PushInputMenu(itemMenu);
        }

        protected void SpellSelect()
        {
            BattleMenu spellMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40), 
                                                  BattleMenu.GridTypes.Vertical);
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

            BattleUIManager.Instance.GetInputMenu().SetElements(options);
        }

        protected void PopulateSpellList()
        {
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();

            DamageSpell Poison = new DamageSpell("Demi1", 3, 3, false, Globals.DamageTypes.Magic, Globals.Elements.Poison, new Poison(2), 50f);
            HealingSpell Cure = new HealingSpell("Cure1", 2, false, 10, 0);
            DamageSpell Ultima = new DamageSpell("Ultima", 4, 5, true, Globals.DamageTypes.Magic, Globals.Elements.Neutral, null, 0f);

            options.Add(new BattleMenu.MenuOption($"{Poison.Name}", () => SpellSelection(Poison)));
            options.Add(new BattleMenu.MenuOption($"{Cure.Name}", () => SpellSelection(Cure)));
            options.Add(new BattleMenu.MenuOption($"{Ultima.Name}", () => SpellSelection(Ultima)));

            BattleUIManager.Instance.GetInputMenu().SetElements(options);
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
