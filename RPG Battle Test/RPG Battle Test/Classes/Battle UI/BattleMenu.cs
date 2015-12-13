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
    //Menu for selecting options in battle; for players only
    public class BattleMenu
    {
        /// <summary>
        /// Menu types; horizontal or vertical
        /// </summary>
        public enum MenuTypes
        {
            Vertical, Horizontal
        }

        public List<MenuOption> Options = null;
        public MenuTypes MenuType = MenuTypes.Vertical;

        /// <summary>
        /// The position of the menu, starting from the top-left, where the first option is shown
        /// </summary>
        public Vector2f Position = new Vector2f(0f, 0f);

        /// <summary>
        /// Difference in spacing between each option
        /// </summary>
        public Vector2f Spacing = new Vector2f(50, 50);

        //How many options are allowed in each column (Horizontal) and each row (Vertical)
        public int MaxPerColumn = 4;
        public int MaxPerRow = 4;

        /// <summary>
        /// How much the menu scrolled vertically or horizontally
        /// </summary>
        public Vector2f Offset = new Vector2f(0, 0);

        public int CurOption = 0;

        public delegate void OptionSelect();

        /// <summary>
        /// Menu option
        /// </summary>
        public class MenuOption
        {
            public Text TextString = null;
            public OptionSelect OnOptionSelect = null;

            public MenuOption(string text, OptionSelect onoptionselect)
            {
                TextString = Helper.CreateText(text, "arial.ttf", new Vector2f(), Color.White);
                OnOptionSelect = onoptionselect;
            }

            public void Select()
            {
                OnOptionSelect?.Invoke();
            }
        }

        public BattleMenu(Vector2f position, Vector2f spacing, MenuTypes menutype, params MenuOption[] options)
        {
            MenuType = menutype;

            Position = position;
            Spacing = spacing;

            Options = options.ToList();
        }

        public void MoveCursor(bool forward)
        {
            if (forward) CurOption = Helper.Wrap(CurOption + 1, 0, Options.Count - 1);
            else CurOption = Helper.Wrap(CurOption - 1, 0, Options.Count - 1);
        }

        public void Draw()
        {
            for (int i = 0; i < Options.Count; i++)
            {
                Text textstring = Options[i].TextString;

                if (textstring != null)
                {
                    int xFactor = MenuType == MenuTypes.Horizontal ? (i % MaxPerColumn) : (i / MaxPerRow);
                    int yFactor = MenuType == MenuTypes.Vertical ? (i % MaxPerRow) : (i / MaxPerColumn);

                    textstring.Position = new Vector2f(Position.X + (xFactor * Spacing.X), Position.Y + (yFactor * Spacing.Y));

                    //Put the origin for the text on the top-left
                    textstring.Origin = new Vector2f(0, 0);

                    textstring.Draw(GameCore.GameWindow, RenderStates.Default);
                }
            }
        }
    }
}
