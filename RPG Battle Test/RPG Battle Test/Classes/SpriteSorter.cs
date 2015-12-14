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
    /// Sorts the rendering of sprites and drawable components by depth
    /// </summary>
    public class SpriteSorter
    {
        private List<DepthObject> DepthBatch = null;

        private struct DepthObject
        {
            public Drawable DrawableObj;
            public int Depth;

            public DepthObject(Drawable drawableobj, int depth)
            {
                DrawableObj = drawableobj;
                Depth = depth;
            }
        }

        public SpriteSorter()
        {
            DepthBatch = new List<DepthObject>();
        }

        /// <summary>
        /// Sorts the depth objects by depth, rendering those with lower values first
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private int SortObjects(DepthObject first, DepthObject second)
        {
            if (first.Depth < second.Depth)
                return -1;
            else if (first.Depth > second.Depth)
                return 1;
            return 0;
        }

        public void Add(Drawable drawable, int depth)
        {
            DepthBatch.Add(new DepthObject(drawable, depth));
        }

        public void DrawAll()
        {
            if (DepthBatch.Count == 0)
                return;

            if (DepthBatch.Count > 1)
                DepthBatch.Sort(SortObjects);

            for (int i = 0; i < DepthBatch.Count; i++)
            {
                DepthBatch[i].DrawableObj.Draw(GameCore.GameWindow, RenderStates.Default);
            }

            DepthBatch.Clear();
        }
    }
}
