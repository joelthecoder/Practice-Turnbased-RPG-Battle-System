using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace RPG_Battle_Test
{
    public class SpellCommand : BattleCommand
    {
        protected Spell SpellCast = null;

        public SpellCommand() : base("Magic")
        {
            
        }

        private void PopulateSpellList(BattlePlayer player)
        {
            List<BattleMenu.MenuOption> options = new List<BattleMenu.MenuOption>();

            Dictionary<string, Spell> entityspells = player.GetAllSpells();

            if (entityspells.Count == 0)
            {
                options.Add(new BattleMenu.MenuOption("No spells - Exit", BattleUIManager.Instance.PopInputMenu));
            }
            else
            {
                foreach (KeyValuePair<string, Spell> spell in entityspells)
                {
                    options.Add(new BattleMenu.MenuOption($"{spell.Value.Name}", () => SelectSpell(player, spell.Value)));
                }
            }

            BattleUIManager.Instance.GetInputMenu().SetElements(options);
        }

        private void SelectSpell(BattlePlayer player, Spell spell)
        {
            if (player.CurMP < spell.MPCost)
            {
                Debug.Log($"{player.Name} has {player.CurMP}MP but requires {spell.MPCost}MP to cast {spell.Name}!");
                return;
            }

            SpellCast = spell;
            TurnsRequired = SpellCast.CastTurns;

            BattleEntity.EntityTypes entityType = spell.GetEntityTypeBasedOnAlignment(player.EntityType);
            bool multiTarget = spell.MultiTarget;
            BattleManager.EntityFilterStates filterState = spell.FilterState;

            BattleUIManager.Instance.StartTargetSelection(BattleManager.Instance.GetEntityGroup(entityType, filterState), multiTarget);
        }

        protected override void OnCommandSelected(BattlePlayer player)
        {
            BattleMenu spellMenu = new BattleMenu(new Vector2f(40f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 38),
                                                  BattleMenu.GridTypes.Vertical);
            spellMenu.OnOpen = () => PopulateSpellList(player);

            BattleUIManager.Instance.PushInputMenu(spellMenu);
        }

        protected override void Perform(BattleEntity Attacker, params BattleEntity[] Victims)
        {
            string usedOn = string.Empty;
            if (Victims.Length == 1)
                usedOn = $" on {Victims[0].Name}";
            Debug.Log($"{Attacker.Name} cast {SpellCast.Name}{usedOn}!");

            Attacker.DrainMP(new Globals.AffectableInfo(Attacker, SpellCast), SpellCast.MPCost);
            SpellCast.OnUse(Attacker, Victims);
        }
    }
}
