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
    /// Base UIElement class, wrapping around SFML graphics
    /// This is primarily used for temporary text and visual effects during battle and is handled by the BattleUIManager
    /// </summary>
    public abstract class UIElement : Drawable
    {
        public float DrawDepth { get; set; } = Constants.BASE_UI_LAYER;

        public bool IsFinished = false;

        public abstract Color Color
        {
            get;
            set;
        }

        public virtual Vector2f Position
        {
            get;
            set;
        }

        public abstract Vector2f Scale
        {
            get;
            set;
        }

        public abstract Vector2f Origin
        {
            get;
            set;
        }

        protected UIElement(float drawDepth)
        {
            DrawDepth = drawDepth;
        }

        public virtual void Update()
        {
            
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //Don't draw if the element is ready to be cleaned up
            if (IsFinished == false)
            {
                DrawElement(target, states);
            }
        }

        protected abstract void DrawElement(RenderTarget target, RenderStates states);
    }
}
