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
        public static SpriteSorter spriteSorter { get; private set; } = null;
        public static bool TimeEnabled { get; private set; } = true;
        private static Time GameTime = default(Time);
        //public static 
        protected Text text = null;

        public const uint FPS = 60;

        /// <summary>
        /// Constructor - called before any GameCore methods
        /// </summary>
        public GameCore(RenderWindow renderWindow)
        {
            GameWindow = renderWindow;
            spriteSorter = new SpriteSorter();
        }
        
        public void OnGameClose(object sender, EventArgs e)
        {
            UnloadAssets();

            //Clean up the BattleManager
            BattleManager.Instance.Dispose();

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
            Debug.Log("Initialized GameCore!");

            GameWindow.SetFramerateLimit(FPS);
            GameWindow.SetKeyRepeatEnabled(false);

            GameWindow.Closed += OnGameClose;
            GameWindow.KeyPressed += Input.OnKeyPressed;
            GameWindow.KeyReleased += Input.OnKeyReleased;

            AssetManager.LoadAssets();

            BattleManager.Instance.Start(new CecilK(), new CecilP(), new BattleEnemy(), new BattleEnemy(), new BattleEnemy());
        }

        /// <summary>
        /// Called on Dispose
        /// </summary>
        protected void UnloadAssets()
        {
            GameWindow.Closed -= OnGameClose;
            GameWindow.KeyPressed -= Input.OnKeyPressed;
            GameWindow.KeyReleased -= Input.OnKeyReleased;

            AssetManager.Dispose();
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
            //We would put this here in the "Battle" game state
            BattleManager.Instance.Draw();
        }

        /// <summary>
        /// Called every frame immediately after Draw(). Here's where a SpriteBatch would output everything to the GPU
        /// </summary>
        public void PostDraw()
        {
            spriteSorter.DrawAll();
        }
    }
}
