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
    /// A grid of UIElements
    /// </summary>
    public class UIGrid<T> where T : UIElement
    {
        /// <summary>
        /// Grid types; horizontal or vertical
        /// </summary>
        public enum GridTypes
        {
            Vertical, Horizontal
        }

        public GridTypes GridType = GridTypes.Vertical;

        protected List<T> ObjList = null;

        /// <summary>
        /// The position of the grid, starting from the top-left, where the first entry is shown
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
        /// How much the grid scrolled vertically or horizontally
        /// </summary>
        public Vector2f Offset = new Vector2f(0, 0);

        /// <summary>
        /// Whether the grid is active or not
        /// </summary>
        public bool Active = false;

        public UIGrid(Vector2f position, Vector2f spacing, GridTypes gridtype)
        {
            GridType = gridtype;
            Position = position;
            Spacing = spacing;
        }

        public virtual void SetElements(List<T> elements)
        {
            ObjList = elements;
        }

        public virtual void SetElements(params T[] elements)
        {
            ObjList = elements.ToList();
        }

        public virtual void AddElements(params T[] elements)
        {
            ObjList.AddRange(elements);
        }

        public virtual void Update()
        {
            
        }

        public void Draw()
        {
            if (Active == false)
                return;

            DrawElements();
        }

        protected virtual void DrawElements()
        {
            //Position each object in its proper spot
            for (int i = 0; i < ObjList.Count; i++)
            {
                T gridItem = ObjList[i];

                if (gridItem != null)
                {
                    int xFactor = GridType == GridTypes.Horizontal ? (i % MaxPerColumn) : (i / MaxPerRow);
                    int yFactor = GridType == GridTypes.Vertical ? (i % MaxPerRow) : (i / MaxPerColumn);

                    gridItem.Position = new Vector2f(Position.X + (xFactor * Spacing.X), Position.Y + (yFactor * Spacing.Y));

                    //Put the origin for the text on the top-left
                    gridItem.Origin = new Vector2f(0, 0);

                    GameCore.spriteSorter.Add(gridItem, Globals.BASE_UI_LAYER + .3f);
                }
            }
        }
    }
}
