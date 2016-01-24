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
    /// A special menu used for selecting targets. It does not derive from any other menu at present.
    /// This menu modifies the text of the BattleUIManager's HeaderBox
    /// </summary>
    public class TargetSelectionMenu : IDisposable
    {
        public delegate void TargetSelection(params BattleEntity[] targets);

        public event TargetSelection TargetSelectionEvent = null;

        public bool Active { get; private set; } = false;

        /// <summary>
        /// The distance above the target the arrow hovers
        /// </summary>
        private const float ArrowVerticalDist = 100f;

        /// <summary>
        /// The list of available targetes
        /// </summary>
        private List<BattleEntity> TargetList = null;

        /// <summary>
        /// Determines whether all targets are selected or not
        /// </summary>
        private bool MultiTarget = false;

        /// <summary>
        /// Current selection
        /// </summary>
        private int CurSelection = 0;

        /// <summary>
        /// The arrows above each entity. If all targets are selected, this list will contain more than one
        /// </summary>
        private List<Sprite> Arrows = new List<Sprite>();

        /// <summary>
        /// Constructor
        /// </summary>
        public TargetSelectionMenu()
        {
            
        }

        public void Dispose()
        {
            TargetSelectionEvent = null;
        }

        /// <summary>
        /// Initialization function
        /// </summary>
        /// <param name="targetList">The list of targets</param>
        /// <param name="multiTarget">Whether to select all targets or not</param>
        public void Start(List<BattleEntity> targetList, bool multiTarget)
        {
            Active = true;

            TargetList = targetList;
            MultiTarget = multiTarget;
            CurSelection = 0;
            Arrows.Clear();

            if (MultiTarget == true)
            {
                for (int i = 0; i < TargetList.Count; i++)
                {
                    Arrows.Add(Helper.CreateSprite(AssetManager.SelectionArrow, false));
                    Arrows[i].Position = new Vector2f(TargetList[i].Position.X, TargetList[i].Position.Y - ArrowVerticalDist);
                }
            }
            else
            {
                Arrows.Add(Helper.CreateSprite(AssetManager.SelectionArrow, false));
                SetTarget(0);
            }
        }

        private void SetTarget(int index)
        {
            if (MultiTarget == true)
            {
                Debug.LogError($"{nameof(MultiTarget)} is true, so all targets are automatically selected");
                return;
            }

            if (index < 0 || index >= TargetList.Count)
            {
                Debug.LogError($"Invalid index of {index} is out of the target list range!");
                return;
            }

            Arrows[0].Position = new Vector2f(TargetList[index].Position.X, TargetList[index].Position.Y - ArrowVerticalDist);
        }

        public void Update()
        {
            if (MultiTarget == false)
            {
                //Move up a selection
                if (Input.PressedKey(Keyboard.Key.Up))
                {
                    CurSelection = Helper.Wrap(CurSelection - 1, 0, TargetList.Count - 1);
                    SetTarget(CurSelection);
                }
                //Move down a selection
                if (Input.PressedKey(Keyboard.Key.Down))
                {
                    CurSelection = Helper.Wrap(CurSelection + 1, 0, TargetList.Count - 1);
                    SetTarget(CurSelection);
                }
            }

            //Cancel
            if (Input.PressedKey(Keyboard.Key.X))
            {
                Active = false;
                return;
            }

            //Confirm
            if (Input.PressedKey(Keyboard.Key.Z))
            {
                if (MultiTarget == true)
                {
                    TargetSelectionEvent?.Invoke(TargetList.ToArray());
                }
                else
                {
                    TargetSelectionEvent?.Invoke(TargetList[CurSelection]);
                }

                Active = false;
            }
        }

        public void Draw()
        {
            if (Active == false)
                return;

            for (int i = 0; i < Arrows.Count; i++)
            {
                GameCore.spriteSorter.Add(Arrows[i], Constants.BASE_UI_LAYER + .03f);
            }
        }
    }
}
