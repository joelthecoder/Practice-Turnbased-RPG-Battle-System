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
    //Helper class for various tasks
    public static class Helper
    {
        /// <summary>
        /// Creates a text object tinted a specific color at a particular position with its origin centered
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Text CreateText(string text, Font font, Vector2f position, Color color)
        {
            Text textobj = new Text(text, font);
            textobj.Origin = GetTextOrigin(textobj);
            textobj.Position = position;
            textobj.Color = color;
            return textobj;
        }

        /// <summary>
        /// Creates a text object tinted a specific color at a particular position with its origin centered.
        /// This method loads the Font reference as well
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontName">The direct name of the font</param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Text CreateText(string text, string fontName, Vector2f position, Color color)
        {
            return CreateText(text, new Font(Globals.ContentPath + fontName), position, color);
        }

        public static Vector2f GetTextOrigin(Text textobj, float xPercent = .5f, float yPercent = .5f)
        {
            FloatRect rect = textobj.GetLocalBounds();
            return new Vector2f(rect.Left + (rect.Width * xPercent), rect.Top + (rect.Height * yPercent));
        }

        /// <summary>
        /// Creates a Sprite object with its origin centered
        /// </summary>
        /// <returns></returns>
        public static Sprite CreateSprite(Texture tex, bool facingRight)
        {
            Sprite spriteobj = new Sprite(tex);
            FloatRect rect = spriteobj.GetLocalBounds();
            spriteobj.Origin = new Vector2f(rect.Left + (rect.Width / 2f), rect.Top + (rect.Height / 2f));
            if (facingRight == true)
            {
                spriteobj.Scale = new Vector2f(-spriteobj.Scale.X, spriteobj.Scale.Y);
            }
            
            return spriteobj;
        }

        public static Vector2f GetSpriteOrigin(Sprite sprite, float xPercent = .5f, float yPercent = .5f)
        {
            FloatRect rect = sprite.GetLocalBounds();
            return new Vector2f(rect.Left + (rect.Width * xPercent), rect.Top + (rect.Height * yPercent));
        }

        /// <summary>
        /// Creates a Sprite object using a portion of a texture with its origin centered
        /// </summary>
        /// <returns></returns>
        public static Sprite CreateSprite(Texture tex, bool facingRight, IntRect textureRect)
        {
            Sprite spriteobj = new Sprite(tex, textureRect);
            FloatRect rect = spriteobj.GetLocalBounds();
            spriteobj.Origin = new Vector2f(rect.Left + (rect.Width / 2f), rect.Top + (rect.Height / 2f));
            if (facingRight == true)
            {
                spriteobj.Scale = new Vector2f(-spriteobj.Scale.X, spriteobj.Scale.Y);
            }

            return spriteobj;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static uint Clamp(uint value, uint min, uint max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Wraps a value around a range
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="min">The inclusive min value; if value is less, it gets set to max</param>
        /// <param name="max">The inclusive max value; if value is greater, it gets set to min</param>
        public static int Wrap(int value, int min, int max)
        {
            return (value < min) ? max : (value > max) ? min : value;
        }

        /// <summary>
        /// Wraps a value around a range
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="min">The inclusive min value; if value is less, it gets set to max</param>
        /// <param name="max">The inclusive max value; if value is greater, it gets set to min</param>
        public static float Wrap(float value, float min, float max)
        {
            return (value < min) ? max : (value > max) ? min : value;
        }
    }
}
