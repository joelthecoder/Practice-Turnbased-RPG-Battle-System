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
    public sealed class PartyInfoMenu : UIGrid<UITextElement>
    {
        public PartyInfoMenu(Vector2f position, Vector2f spacing) : base(position, spacing, GridTypes.Horizontal)
        {
            Active = true;
            MaxPerColumn = 3;
        }

        public void SetUpPartyInfo()
        {
            ObjList = new List<UITextElement>();

            for (int i = 0; i < BattleManager.Instance.Players.Count; i++)
            {
                BattleEntity player = BattleManager.Instance.Players[i];

                //Show name, HP, and MP
                ObjList.Add(new UITextElement(player.Name));
                ObjList.Add(new UITextElement(player.CurHP + "/" + player.MaxHP));
                ObjList.Add(new UITextElement(player.CurMP + "/" + player.MaxMP));
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

                ObjList[reali].TextString = player.Name;
                ObjList[reali].Color = optioncolor;
                ObjList[reali + 1].TextString = player.CurHP + "/" + player.MaxHP;
                ObjList[reali + 1].Color = optioncolor;
                ObjList[reali + 2].TextString = player.CurMP + "/" + player.MaxMP;
                ObjList[reali + 2].Color = optioncolor;
            }
        }
    }
}
