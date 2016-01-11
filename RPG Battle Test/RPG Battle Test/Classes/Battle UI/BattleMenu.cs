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

        public delegate void Open();
        public delegate void OptionSelect();
        public delegate void BackOut();

        public Open OnOpen = null;

        public BackOut OnBackOut = null;

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

        public bool Active = false;
        public bool HideArrow = false;
        public bool CanBackOut = true;

        public int CurOption = 0;

        protected Sprite Arrow = null;

        /// <summary>
        /// Menu option
        /// </summary>
        public class MenuOption
        {
            public Text TextString = null;
            public OptionSelect OnOptionSelect = null;

            public MenuOption(string text, OptionSelect onoptionselect)
            {
                TextString = Helper.CreateText(text, AssetManager.TextFont, new Vector2f(), Color.White);
                OnOptionSelect = onoptionselect;
            }

            public void Select()
            {
                OnOptionSelect?.Invoke();
            }
        }

        protected BattleMenu(Vector2f position, Vector2f spacing, MenuTypes menutype)
        {
            MenuType = menutype;
            Position = position;
            Spacing = spacing;
        }

        public BattleMenu(Vector2f position, Vector2f spacing, MenuTypes menutype, BackOut onbackout = null)
            : this(position, spacing, menutype)
        {
            Arrow = Helper.CreateSprite(AssetManager.SelectionArrow, false);
            Arrow.Rotation = 270f;
            Arrow.Origin = Helper.GetSpriteOrigin(Arrow, yPercent: 0f);

            Active = true;
            HideArrow = false;
            OnBackOut = onbackout;
        }

        public BattleMenu(Open onopen, Vector2f position, Vector2f spacing, MenuTypes menutype, BackOut onbackout = null)
        : this(position, spacing, menutype, onbackout)
        {
            OnOpen = onopen;
        }

        public void SetOptions(params MenuOption[] options)
        {
            Options = options.ToList();
            CurOption = 0;
        }

        public void SetOptions(List<MenuOption> options)
        {
            Options = options;
            CurOption = 0;
        }

        public void AddOptions(params MenuOption[] options)
        {
            Options.AddRange(options);
        }

        public void AddOptions(List<MenuOption> options)
        {
            Options.AddRange(options);
        }

        public void MoveCursor(bool forward)
        {
            if (forward) CurOption = Helper.Wrap(CurOption + 1, 0, Options.Count - 1);
            else CurOption = Helper.Wrap(CurOption - 1, 0, Options.Count - 1);
        }

        public virtual void Update()
        {
            //No input if the menu or arrow aren't active
            if (Active == false || HideArrow == true)
                return;

            if (MenuType == MenuTypes.Vertical)
            {
                if (Input.PressedKey(Keyboard.Key.Up))
                    MoveCursor(false);
                if (Input.PressedKey(Keyboard.Key.Down))
                    MoveCursor(true);
            }
            else if (MenuType == MenuTypes.Horizontal)
            {
                if (Input.PressedKey(Keyboard.Key.Left))
                    MoveCursor(false);
                if (Input.PressedKey(Keyboard.Key.Right))
                    MoveCursor(true);
            }

            //Select menu
            if (Input.PressedKey(Keyboard.Key.Z))
            {
                Options?[CurOption].Select();
            }
            //Back out of the menu if possible
            else if (CanBackOut == true && Input.PressedKey(Keyboard.Key.X))
            {
                Active = false;

                BattleUIManager.Instance.PopInputMenu();
                //OnBackOut?.Invoke();
            }
        }

        public void Draw()
        {
            if (Active == false)
                return;

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

                    GameCore.spriteSorter.Add(textstring, Constants.BASE_UI_LAYER + .3f);
                }
            }

            if (HideArrow == false)
            {
                Arrow.Position = new Vector2f(Options[CurOption].TextString.Position.X - 35, Options[CurOption].TextString.Position.Y + 18);
                GameCore.spriteSorter.Add(Arrow, Constants.BASE_UI_LAYER + .4f);
            }
        }
    }
}
