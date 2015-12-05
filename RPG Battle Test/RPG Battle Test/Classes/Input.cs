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
using static SFML.Window.Keyboard;

namespace RPG_Battle_Test
{
    public static class Input
    {
        //States of each key press; we need two so we can track when a key was JUST pressed on this frame
        private static bool[] PrevPressedArray;
        private static bool[] PressedArray;

        //Used for tracking which keys have changed; this is efficient for changing the previous values
        //from pressed to not-pressed and vice versa
        private static Dictionary<Key, int> KeysChanged = new Dictionary<Key, int>();

        static Input()
        {
            PrevPressedArray = new bool[(int)Key.KeyCount];
            PressedArray = new bool[(int)Key.KeyCount];
        }

        /// <summary>
        /// Returns if a key is held
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyHeld(Key key)
        {
            if (key == Key.Unknown)
                return false;
            return IsKeyPressed(key);
        }

        /// <summary>
        /// Returns if a key was JUST pressed on this frame
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool PressedKey(Key key)
        {
            if (key == Key.Unknown)
                return false;
            return (PrevPressedArray[(int)key] == false && PressedArray[(int)key] == true);
        }

        /// <summary>
        /// KeyPressed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyeventargs"></param>
        public static void OnKeyPressed(object sender, KeyEventArgs keyeventargs)
        {
            if (keyeventargs.Code != Key.Unknown)
            {
                int index = (int)keyeventargs.Code;
                PrevPressedArray[index] = PressedArray[index];
                PressedArray[index] = true;
                if (KeysChanged.ContainsKey(keyeventargs.Code) == false)
                {
                    KeysChanged.Add(keyeventargs.Code, index);
                }

                //Debug.Log("Prev: " + PrevPressedArray[index] + " Current: " + PressedArray[index]);
            }
        }

        /// <summary>
        /// KeyReleased event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keyeventargs"></param>
        public static void OnKeyReleased(object sender, KeyEventArgs keyeventargs)
        {
            if (keyeventargs.Code != Key.Unknown)
            {
                int index = (int)keyeventargs.Code;
                PrevPressedArray[index] = PressedArray[index];
                PressedArray[index] = false;
                if (KeysChanged.ContainsKey(keyeventargs.Code) == false)
                {
                    KeysChanged.Add(keyeventargs.Code, index);
                }

                //Debug.Log("Prev: " + PrevPressedArray[index] + " Current: " + PressedArray[index]);
            }
        }

        /// <summary>
        /// Updates the state of the tracked key presses.
        /// If a key was just pressed on this frame, it will be registered as held.
        /// If a key was just released on this frame, it will be registered as not held
        /// </summary>
        public static void UpdateInput()
        {
            foreach (KeyValuePair<Key, int> pair in KeysChanged)
            {
                PrevPressedArray[pair.Value] = PressedArray[pair.Value];
            }

            if (KeysChanged.Count > 0)
                KeysChanged.Clear();
        }
    }
}
