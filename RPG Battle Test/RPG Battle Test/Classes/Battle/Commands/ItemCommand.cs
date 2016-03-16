using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace RPG_Battle_Test
{
    public class ItemCommand : BattleCommand
    {
        protected Item ItemUsed = null;

        public ItemCommand() : base("Item")
        {
            
        }

        private void PopulateItemList(BattlePlayer player)
        {
            Dictionary<Item, int> items = BattleManager.Instance.PartyInventory.GetInventory();
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();

            if (items.Count == 0)
            {
                options.Add(new BattleMenu.MenuOption("Empty Inventory - Exit", BattleUIManager.Instance.PopInputMenu));
            }
            else
            {
                foreach (KeyValuePair<Item, int> pair in items)
                {
                    options.Add(new BattleMenu.MenuOption($"{pair.Key.Name} x{pair.Value}", () => SelectItem(player, pair.Key)));
                }
            }

            BattleUIManager.Instance.GetInputMenu().SetElements(options);
        }

        private void SelectItem(BattlePlayer player, Item item)
        {
            ItemUsed = item;

            BattleEntity.EntityTypes entityType = item.GetEntityTypeBasedOnAlignment(player.EntityType);
            bool multiTarget = item.MultiTarget;
            BattleManager.EntityFilterStates filterState = item.FilterState;

            BattleUIManager.Instance.StartTargetSelection(BattleManager.Instance.GetEntityGroup(entityType, filterState), multiTarget);
        }

        protected override void OnCommandSelected(BattlePlayer player)
        {
            BattleMenu itemMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 38),
                                                 BattleMenu.GridTypes.Vertical);
            itemMenu.OnOpen = () => PopulateItemList(player);
            BattleUIManager.Instance.PushInputMenu(itemMenu);
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            string usedOn = string.Empty;
            if (Victims.Length == 1)
                usedOn = $" on {Victims[0].Name}";
            Debug.Log($"{Attacker.Name} used {ItemUsed.Name}{usedOn}!");
            ItemUsed.OnUse(Attacker, Victims);
        }
    }
}
