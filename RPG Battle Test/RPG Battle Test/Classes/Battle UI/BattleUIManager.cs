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
    /// Handles the UI during turn-based battles, including menus, numerical damage displays, and more
    /// This is a Singleton
    /// </summary>
    public sealed class BattleUIManager : IDisposable
    {
        public static BattleUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleUIManager();
                }

                return instance;
            }
        }

        public static bool IsAwake => (instance != null);

        private static BattleUIManager instance = null;

        //Input menus used by BattlePlayers
        private readonly Stack<BattleMenu> InputMenus = new Stack<BattleMenu>();

        //The main box at the bottom of the screen, serving as a container for all the battle menus and player information
        private readonly MessageBox ActionBox = null;

        //Header box describing the details of battle
        private readonly TextBox HeaderBox = null;

        //Party info display
        private readonly PartyInfoMenu PartyInfo = null;

        //List of UIElements
        private List<UIElement> Elements = new List<UIElement>();

        public TargetSelectionMenu TargetMenu = new TargetSelectionMenu();

        private BattleUIManager()
        {
            //Initialize necessary menus here
            ActionBox = new MessageBox(796, 150, new Vector2f(GameCore.GameWindow.Size.X / 2f, GameCore.GameWindow.Size.Y - 77));
            HeaderBox = new TextBox(Helper.CreateText("Smell", AssetManager.TextFont, new Vector2f(), Color.White), 40, 20, 
                new Vector2f(GameCore.GameWindow.Size.X / 2, GameCore.GameWindow.Size.Y / 16));

            PartyInfo = new PartyInfoMenu(new Vector2f(440f, GameCore.GameWindow.Size.Y - 150), new Vector2f(100, 40));
        }

        public void Start()
        {
            PartyInfo.SetUpPartyInfo();
        }

        public void Dispose()
        {
            Elements.Clear();
            Elements = null;

            InputMenus.Clear();
            TargetMenu.Dispose();

            instance = null;
        }

        public void PushInputMenu(BattleMenu menu)
        {
            if (menu == null)
            {
                Debug.LogError($"Refusing to push {nameof(menu)} because it is null!");
                return;
            }

            InputMenus.Push(menu);
            menu.OnOpen?.Invoke();
        }

        public BattleMenu GetInputMenu()
        {
            if (InputMenus.Count > 0)
                return InputMenus.Peek();

            return null;
        }

        public int InputMenuCount() => InputMenus.Count;

        public void PopInputMenu()
        {
            if (InputMenus.Count == 0)
            {
                Debug.LogError($"Cannot pop from {nameof(InputMenus)} because it is empty!");
                return;
            }

            BattleMenu menu = InputMenus.Pop();
            menu.OnBackOut?.Invoke();
        }

        public void SetHeaderText(string message)
        {
            HeaderBox.SetText(message);
        }

        public void AddElement(UIElement uiElement)
        {
            Elements.Add(uiElement);
        }

        public void StartTargetSelection(List<BattleEntity> targetList, bool multiTarget)
        {
            if (targetList == null || targetList.Count == 0)
            {
                Debug.LogError($"Cannot set target selection because {nameof(targetList)} is null or empty!");
                return;
            }

            TargetMenu.Start(targetList, multiTarget);
        }

        public void Update()
        {
            if (Elements == null)
            {
                Debug.LogError($"{nameof(Elements)} list is null!");
                return;
            }
            
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Update();

                //Remove element if finished
                if (Elements[i].IsFinished == true)
                {
                    Elements.RemoveAt(i);
                    i--;
                }
            }

            //Update main UI elements
            PartyInfo.Update();
        }

        public void Draw()
        {
            if (Elements == null)
            {
                Debug.LogError($"{nameof(Elements)} list is null!");
                return;
            }

            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Draw(GameCore.GameWindow, RenderStates.Default);
            }

            //Draw main UI elements
            HeaderBox.Draw();
            ActionBox.Draw();
            PartyInfo.Draw();
            if (InputMenus.Count > 0)
                InputMenus.Peek().Draw();
            TargetMenu.Draw();
        }
    }
}
