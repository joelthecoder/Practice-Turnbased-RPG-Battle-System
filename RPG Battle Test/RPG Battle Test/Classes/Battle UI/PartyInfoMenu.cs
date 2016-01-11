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
    /// <summary>
    /// A BattleMenu specifically for displaying party information.
    /// It automatically updates player info
    /// </summary>
    public sealed class PartyInfoMenu : BattleMenu
    {
        public PartyInfoMenu(Vector2f position, Vector2f spacing) : base(position, spacing, MenuTypes.Horizontal)
        {
            Active = HideArrow = true;
            MaxPerColumn = 3;
        }

        public void SetUpPartyInfo()
        {
            Options = new List<MenuOption>();

            for (int i = 0; i < BattleManager.Instance.Players.Count; i++)
            {
                BattleEntity player = BattleManager.Instance.Players[i];

                //Show name, HP, and MP
                Options.Add(new MenuOption(player.Name, null));
                Options.Add(new MenuOption(player.CurHP + "/" + player.MaxHP, null));
                Options.Add(new MenuOption(player.CurMP + "/" + player.MaxMP, null));
            }
        }

        public override void Update()
        {
            for (int i = 0; i < BattleManager.Instance.Players.Count; i++)
            {
                BattleEntity player = BattleManager.Instance.Players[i];
                Color optioncolor = Color.White;

                //Get color here, which corresponds to the player's current status
                if (player.IsDead == true)
                    optioncolor = Color.Red;

                int reali = i * MaxPerColumn;

                Options[reali].TextString.DisplayedString = player.Name;
                Options[reali].TextString.Color = optioncolor;
                Options[reali + 1].TextString.DisplayedString = player.CurHP + "/" + player.MaxHP;
                Options[reali + 1].TextString.Color = optioncolor;
                Options[reali + 2].TextString.DisplayedString = player.CurMP + "/" + player.MaxMP;
                Options[reali + 2].TextString.Color = optioncolor;
            }
        }
    }
}
