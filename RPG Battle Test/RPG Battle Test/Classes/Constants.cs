using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    //Holds all constant values
    public static class Constants
    {
        private static readonly string CurPath = Environment.CurrentDirectory + "\\";
        public static readonly string ContentPath = CurPath + "Content\\";

        public const float BASE_BACKGROUND_LAYER = 0f;
        public const float BASE_ENTITY_LAYER = 1f;
        public const float BASE_UI_LAYER = 2f;

        static Constants()
        {
            
        }
    }
}
