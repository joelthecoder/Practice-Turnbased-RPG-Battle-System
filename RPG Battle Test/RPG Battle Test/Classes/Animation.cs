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
    /// Sprite animation
    /// </summary>
    public sealed class Animation
    {
        public Texture SpriteSheet = null;
        private Frame[] Frames = null;
        private int MaxFrames = 1;
        private int CurFrame = 0;

        public Animation(Texture texture, params IntRect[] frameRects)
        {
            SpriteSheet = texture;
            MaxFrames = frameRects.Length;
            Frames = new Frame[MaxFrames];
            for (int i = 0; i < Frames.Length; i++)
            {
                Frames[i] = new Frame(SpriteSheet, frameRects[i]);
            }
        }

        private void Progress()
        {
            CurFrame++;
            if (CurFrame >= MaxFrames)
            {
                CurFrame = MaxFrames - 1;

                //Animation done
            }
        }

        /// <summary>
        /// Animation frame
        /// </summary>
        private sealed class Frame
        {
            public IntRect TextureRect = new IntRect();
            public Sprite FrameSprite = null;

            public Frame(Texture texture, IntRect texrect)
            {
                TextureRect = texrect;

                FrameSprite = Helper.CreateSprite(texture, true, TextureRect);
            }
        }
    }
}
