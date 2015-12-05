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
    internal class GameCore
    {
        public static RenderWindow GameWindow { get; private set; } = null;
        public static bool TimeEnabled { get; private set; } = true;
        private static Time GameTime = default(Time);
        //public static 
        protected Text text = null;

        /// <summary>
        /// Constructor - called before any GameCore methods
        /// </summary>
        public GameCore(RenderWindow renderWindow)
        {
            GameWindow = renderWindow;
        }
        
        public void OnGameClose(object sender, EventArgs e)
        {
            UnloadAssets();

            //Close the window when OnClose event is received and clean up events
            GameWindow.Close();
            GameWindow = null;
        }
        
        public static float ActiveSeconds
        {
            get { return GameTime.AsSeconds(); }
        }

        /// <summary>
        /// Initialize anything after the constructor
        /// </summary>
        public void Initialize()
        {
            /*Font font = new Font(Constants.ContentPath + "arial.ttf");
            text = new Text(@"Press 'Enter' to play", font);
            FloatRect rect = text.GetLocalBounds();
            text.Origin = new Vector2f(rect.Left + (rect.Width / 2f), rect.Top + (rect.Height/2f));
            text.Position = new Vector2f(GameWindow.Size.X / 2f, GameWindow.Size.Y / 2f);*/

            Debug.Log("Initialized GameCore!");

            GameWindow.SetFramerateLimit(60);
            GameWindow.SetKeyRepeatEnabled(false);

            GameWindow.Closed += OnGameClose;
            GameWindow.KeyPressed += Input.OnKeyPressed;
            GameWindow.KeyReleased += Input.OnKeyReleased;

            BattleManager.Instance.Start(new BattlePlayer(BattlePlayer.Characters.CecilK), new BattlePlayer(BattlePlayer.Characters.CecilP),
                              new BattleEnemy(), new BattleEnemy(), new BattleEnemy());
        }

        /// <summary>
        /// Called on cleanup
        /// </summary>
        protected void UnloadAssets()
        {
            GameWindow.Closed -= OnGameClose;
            GameWindow.KeyPressed -= Input.OnKeyPressed;
            GameWindow.KeyReleased -= Input.OnKeyReleased;
        }

        /// <summary>
        /// Called every frame for game world updates
        /// </summary>
        public void Update(Time time)
        {
            //Update game time if it's enabled
            if (TimeEnabled == true)
                GameTime += time;

            //We would put this here in the "Battle" game state
            BattleManager.Instance.Update();
        }

        /// <summary>
        /// Called every frame immediately after Update(). Here's where updates for input states and other things would go
        /// </summary>
        public void PostUpdate()
        {
            Input.UpdateInput();
        }

        /// <summary>
        /// Called every frame for game world rendering
        /// </summary>
        public void Draw()
        {
            text?.Draw(GameWindow, new RenderStates(BlendMode.Add));

            //We would put this here in the "Battle" game state
            BattleManager.Instance.Draw();
        }
    }
}
