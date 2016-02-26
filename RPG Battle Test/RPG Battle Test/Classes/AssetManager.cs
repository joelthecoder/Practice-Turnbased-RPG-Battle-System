using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Holds important, commonly used assets, and aids in loading/unloading assets
    /// </summary>
    public static class AssetManager
    {
        public static Font TextFont = null;

        public static Texture MsgBoxM = null;
        public static Texture MsgBoxL = null;
        public static Texture MsgBoxT = null;
        public static Texture MsgBoxTL = null;
        public static Texture SelectionArrow = null;

        public static void LoadAssets()
        {
            TextFont = new Font(Globals.ContentPath + "arial.ttf");

            MsgBoxM = new Texture(Globals.ContentPath + "MessageBox\\MsgBoxM.png");
            MsgBoxL = new Texture(Globals.ContentPath + "MessageBox\\MsgBoxL.png");
            MsgBoxT = new Texture(Globals.ContentPath + "MessageBox\\MsgBoxT.png");
            MsgBoxTL = new Texture(Globals.ContentPath + "MessageBox\\MsgBoxTL.png");
            SelectionArrow = new Texture(Globals.ContentPath + "Arrow.png");
        }

        private static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void Dispose()
        {
            TextFont.Dispose();

            MsgBoxM.Dispose();
            MsgBoxL.Dispose();
            MsgBoxT.Dispose();
            MsgBoxTL.Dispose();
            SelectionArrow.Dispose();
        }
    }
}
