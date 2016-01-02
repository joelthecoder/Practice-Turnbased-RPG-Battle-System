using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using static System.Console;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

namespace RPG_Battle_Test
{
    /// <summary>
    /// Debug class meant for aiding the diagnosis of issues in the game
    /// </summary>
    public static class Debug
    {
        public static bool EnableLogs { get; private set; } = true;

        public static void SetLogsEnabled(bool enabled)
        {
            EnableLogs = enabled;
        }

        public static void Log(object value)
        {
            if (EnableLogs == false)
                return;
            StackFrame trace = new StackFrame(1, true);
            int line = 0;
            string method = "";

            string[] file = trace.GetFileName().Split('\\');
            string fileName = file?[file.Length - 1];

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;

            WriteLine("Information: " + fileName + " -> " + method + ": (" + line + ") - " + value);
        }

        public static void LogWarning(object value)
        {
            if (EnableLogs == false)
                return;
            StackFrame trace = new StackFrame(1, true);
            int line = 0;
            string method = "";

            string[] file = trace.GetFileName().Split('\\');
            string fileName = file?[file.Length - 1];

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;

            WriteLine("Warning: " + fileName + " -> " + method + ": (" + line + ") - " + value);
        }

        public static void LogError(object value)
        {
            if (EnableLogs == false)
                return;
            StackFrame trace = new StackFrame(1, true);
            int line = 0;
            string method = "";

            string[] file = trace.GetFileName().Split('\\');
            string fileName = file?[file.Length - 1];

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;

            WriteLine("Error: " + fileName + " -> " + method + ": (" + line + ") - " + value);
        }
    }
}
