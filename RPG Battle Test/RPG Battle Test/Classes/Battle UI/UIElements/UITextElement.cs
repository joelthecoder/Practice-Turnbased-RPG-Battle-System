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
    public class UITextElement : UIElement
    {
        protected Text TextObject = null;

        public override sealed Color Color
        {
            get
            {
                return TextObject.Color;
            }
            set
            {
                TextObject.Color = value;
            }
        }

        public override sealed Vector2f Position
        {
            get
            {
                return TextObject.Position;
            }
            set
            {
                TextObject.Position = value;
            }
        }

        public override sealed Vector2f Scale
        {
            get
            {
                return TextObject.Scale;
            }
            set
            {
                TextObject.Scale = value;
            }
        }

        public override Vector2f Origin
        {
            get
            {
                return TextObject.Origin;
            }
            set
            {
                TextObject.Origin = value;
            }
        }

        public string TextString
        {
            get
            {
                return TextObject.DisplayedString;
            }
            set
            {
                TextObject.DisplayedString = value;
            }
        }

        public UITextElement(string text) : base(Globals.BASE_UI_LAYER)
        {
            TextObject = Helper.CreateText(text, AssetManager.TextFont, new Vector2f(0, 0), Color.White);
        }

        public UITextElement(string text, Vector2f position, Color color, float drawDepth) : base(drawDepth)
        {
            TextObject = Helper.CreateText(text, AssetManager.TextFont, position, color);
        }

        protected override sealed void DrawElement(RenderTarget target, RenderStates states)
        {
            GameCore.spriteSorter.Add(TextObject, DrawDepth);
        }
    }
}
