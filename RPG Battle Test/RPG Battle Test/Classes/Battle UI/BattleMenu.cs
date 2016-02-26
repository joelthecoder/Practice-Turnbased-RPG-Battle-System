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
    public class BattleMenu : UIGrid<BattleMenu.MenuOption>
    {
        public delegate void Open();
        public delegate void BackOut();

        /// <summary>
        /// Delegate called when the menu is opened
        /// </summary>
        public Open OnOpen = null;

        /// <summary>
        /// Delegate called when the menu is backed out of
        /// </summary>
        public BackOut OnBackOut = null;

        /// <summary>
        /// Determines whether you can back out of the menu or not
        /// </summary>
        public bool CanBackOut = true;

        /// <summary>
        /// Whether options wrap around to the other end when moving the cursor or not
        /// </summary>
        public bool WrapOptions = false;

        protected int CurOption = 0;
        protected Sprite Arrow = null;

        /// <summary>
        /// Menu option
        /// </summary>
        public class MenuOption : UITextElement
        {
            public delegate void OptionSelect();
            public OptionSelect OnOptionSelect = null;

            public MenuOption(string text, OptionSelect onoptionselect) : base(text)
            {
                OnOptionSelect = onoptionselect;
            }

            public void Select()
            {
                OnOptionSelect?.Invoke();
            }
        }

        protected BattleMenu(Vector2f position, Vector2f spacing, GridTypes gridtype) : base(position, spacing, gridtype)
        {
            GridType = gridtype;
            Position = position;
            Spacing = spacing;
        }

        public BattleMenu(Vector2f position, Vector2f spacing, GridTypes gridtype, BackOut onbackout = null)
            : this(position, spacing, gridtype)
        {
            Arrow = Helper.CreateSprite(AssetManager.SelectionArrow, false);
            Arrow.Rotation = 270f;
            Arrow.Origin = Helper.GetSpriteOrigin(Arrow, yPercent: 0f);

            Active = true;
            OnBackOut = onbackout;
        }

        public BattleMenu(Open onopen, Vector2f position, Vector2f spacing, GridTypes gridtype, BackOut onbackout = null)
        : this(position, spacing, gridtype, onbackout)
        {
            OnOpen = onopen;
        }

        public override void SetElements(params MenuOption[] options)
        {
            base.SetElements(options);
            CurOption = 0;
        }

        public override void SetElements(List<MenuOption> options)
        {
            base.SetElements(options);
            CurOption = 0;
        }

        public void MoveCursor(bool forward, int amount = 1)
        {
            if (forward)
            {
                if (WrapOptions == true)
                    CurOption = Helper.Wrap(CurOption + amount, 0, ObjList.Count - 1);
                else CurOption = Helper.Clamp(CurOption + amount, 0, ObjList.Count - 1);
            }
            else
            {
                if (WrapOptions == true)
                    CurOption = Helper.Wrap(CurOption - amount, 0, ObjList.Count - 1);
                else CurOption = Helper.Clamp(CurOption - amount, 0, ObjList.Count - 1);
            }
        }

        public override void Update()
        {
            //No input if the menu isn't active
            if (Active == false)
                return;

            if (GridType == GridTypes.Vertical)
            {
                if (Input.PressedKey(Keyboard.Key.Up))
                    MoveCursor(false);
                if (Input.PressedKey(Keyboard.Key.Down))
                    MoveCursor(true);
                if (Input.PressedKey(Keyboard.Key.Left))
                    MoveCursor(false, MaxPerRow);
                if (Input.PressedKey(Keyboard.Key.Right))
                    MoveCursor(true, MaxPerRow);
            }
            else if (GridType == GridTypes.Horizontal)
            {
                if (Input.PressedKey(Keyboard.Key.Left))
                    MoveCursor(false);
                if (Input.PressedKey(Keyboard.Key.Right))
                    MoveCursor(true);
                if (Input.PressedKey(Keyboard.Key.Up))
                    MoveCursor(false, MaxPerColumn);
                if (Input.PressedKey(Keyboard.Key.Down))
                    MoveCursor(true, MaxPerColumn);
            }

            //Select menu
            if (Input.PressedKey(Keyboard.Key.Z))
            {
                ObjList?[CurOption].Select();
            }
            //Back out of the menu if possible
            else if (CanBackOut == true && Input.PressedKey(Keyboard.Key.X))
            {
                Active = false;

                BattleUIManager.Instance.PopInputMenu();
                //OnBackOut?.Invoke();
            }
        }

        protected override void DrawElements()
        {
            base.DrawElements();

            Arrow.Position = new Vector2f(ObjList[CurOption].Position.X - 35, ObjList[CurOption].Position.Y + 18);
            GameCore.spriteSorter.Add(Arrow, Globals.BASE_UI_LAYER + .4f);
        }
    }
}
