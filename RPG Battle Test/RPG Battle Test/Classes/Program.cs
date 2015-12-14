using System;
using System.Windows;
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
    internal sealed class Program
    {
        static void Main()
        {
            RenderWindow GameWindow = new RenderWindow(new VideoMode(800, 600), "SFML Works!");
            
            GameCore gameCore = new GameCore(GameWindow);
            gameCore.Initialize();

            Color windowColor = new Color(0, 192, 255);

            Clock clock = new Clock();

            //Start the game loop
            while (GameWindow.IsOpen)
            {
                //Process events
                GameWindow.DispatchEvents();

                //Update game logic here
                gameCore.Update(clock.Restart());
                gameCore.PostUpdate();

                //Clear screen
                GameWindow.Clear(windowColor);

                //Draw game logic here
                gameCore.Draw();
                gameCore.PostDraw();

                //Update the window
                GameWindow.Display();
            }
        }
    }
}
