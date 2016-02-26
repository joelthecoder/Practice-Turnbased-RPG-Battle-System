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
    //A message box with text
    public sealed class TextBox : MessageBox
    {
        private Text TextMessage = null;

        //How much larger the message box is than the text (to not have the text look cramped)
        private uint SpacingX = 20, SpacingY = 20;

        public TextBox(Text text, uint spaceX, uint spaceY, Vector2f position)
        : base((uint)text.GetLocalBounds().Width + spaceX, (uint)text.GetLocalBounds().Height + spaceY, position)
        {
            TextMessage = text;

            SpacingX = spaceX;
            SpacingY = spaceY;

            TextMessage.Position = position;
        }

        public override Vector2f Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                base.Position = value;
                if (TextMessage != null)
                    TextMessage.Position = value;
            }
        }

        public void SetText(string message)
        {
            if (TextMessage.DisplayedString == message)
                return;

            TextMessage.DisplayedString = message;
            TextMessage.UpdateOrigin();
            FloatRect rect = TextMessage.GetLocalBounds();
            Resize((uint)rect.Width + SpacingX, (uint)rect.Height + SpacingY);
        }

        public void SetSpacing(uint spaceX, uint spaceY)
        {
            if (SpacingX == spaceX && SpacingY == spaceY)
                return;

            Vector2f diff = new Vector2f(spaceX - SpacingX, spaceY - SpacingY);

            SpacingX = spaceX;
            SpacingY = spaceY;

            Resize(diff);
        }

        public override void Draw()
        {
            base.Draw();
            GameCore.spriteSorter.Add(TextMessage, Globals.BASE_UI_LAYER + .2f);
        }
    }
}
