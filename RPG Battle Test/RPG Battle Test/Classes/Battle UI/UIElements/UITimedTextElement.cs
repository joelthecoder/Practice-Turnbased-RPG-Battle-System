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
    public class UITimedTextElement : UITextElement
    {
        public float Time { get; private set; } = 0f;
        private float PrevTime = 0f;

        public UITimedTextElement(float time, string text, Vector2f position, Color color, float drawDepth) : base(text, position, color, drawDepth)
        {
            Time = time;
            PrevTime = GameCore.ActiveSeconds + Time;
        }

        public override void Update()
        {
            base.Update();
            if (GameCore.ActiveSeconds >= PrevTime)
            {
                IsFinished = true;
            }
        }
    }
}
