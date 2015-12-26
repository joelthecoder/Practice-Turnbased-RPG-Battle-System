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
    /// Sprite animations that can loop
    /// </summary>
    public class LoopAnimation : Animation
    {
        public const int CONTINOUS_LOOP = -1;

        private int NumLoops = 1;
        private int CurLoops = 0;

        public LoopAnimation(int numLoops, Texture texture, float frameRate, params IntRect[] frameRects)
            : base(texture, frameRate, frameRects)
        {
            AnimationType = AnimationTypes.Looping;

            NumLoops = numLoops;
        }

        protected override void Progress()
        {
            if (CurFrame >= MaxFrameIndex)
            {
                if (NumLoops <= CONTINOUS_LOOP)
                {
                    CurFrame = 0;
                }
                else
                {
                    CurLoops++;
                    if (CurLoops >= NumLoops)
                    {
                        AnimDone = true;
                    }
                    else
                    {
                        CurFrame = 0;
                    }
                }
            }
            else
            {
                CurFrame++;
            }

            ResetFrameDur();
        }

        public override void Reset()
        {
            base.Reset();
            CurLoops = 0;
        }
    }
}
