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
    /// Various extension methods
    /// </summary>
    public static class Extensions
    {
        public static void UpdateOrigin(this Text text)
        {
            text.Origin = Helper.GetTextOrigin(text);
        }
    }
}
