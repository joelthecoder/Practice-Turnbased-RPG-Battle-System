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
    public sealed class BattleUIManager
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

        //List of UIElements
        private List<UIElement> Elements = new List<UIElement>();

        private BattleUIManager()
        {
            
        }

        ~BattleUIManager()
        {
            
        }

        public void Start()
        {
            
        }

        public void CleanUp()
        {
            Elements.Clear();
            Elements = null;
        }

        public void AddElement(UIElement uiElement)
        {
            Elements.Add(uiElement);
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
        }
    }
}
