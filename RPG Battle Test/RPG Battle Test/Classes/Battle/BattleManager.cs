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
    //Handles turns and the state of a turn-based battles
    //This is a Singleton
    public sealed class BattleManager
    {
        public enum BattleStates
        {
            Init, Combat, TurnDone, Victory, GameOver
        }

        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }

                return instance;
            }
        }

        public static bool IsAwake => (instance != null);

        private static BattleManager instance = null;

        //Begin instance members
        //Determines turn order
        private List<BattleEntity> EntityOrder = null;
        private List<BattleEnemy> Enemies = new List<BattleEnemy>();
        private List<BattlePlayer> Players = new List<BattlePlayer>();
        private int CurEntityTurn = 0;

        public readonly TextBox HeaderBox = new TextBox(Helper.CreateText("Smell", "arial.ttf", new Vector2f(), Color.White), 40, 20,
                                          new Vector2f(GameCore.GameWindow.Size.X / 2, GameCore.GameWindow.Size.Y / 16));

        //The current state of the battle
        public BattleStates BattleState { get; private set; } = BattleStates.Init;

        private readonly List<Vector2f> EnemyPositions = null;
        private readonly List<Vector2f> PlayerPositions = null;
        private Sprite Background = null;

        private BattleManager()
        {
            EnemyPositions = new List<Vector2f>()
            {
                new Vector2f(325, 225),
                new Vector2f(200, 300),
                new Vector2f(300, 375)
            };

            PlayerPositions = new List<Vector2f>()
            {
                new Vector2f(550, 275),
                new Vector2f(550, 375)
            };
        }

        ~BattleManager()
        {
            //Clear instance and event
            instance = null;
        }

        public BattleEntity CurrentEntityTurn => EntityOrder[CurEntityTurn];

        public void Start(params BattleEntity[] entities)
        {
            EntityOrder = entities.ToList();
            EntityOrder.Sort((e1, e2) =>
            {
                if (e1.Speed > e2.Speed)
                    return -1;
                if (e1.Speed < e2.Speed)
                    return 1;
                return 0;
             });

            int enemyindex = 0, playerindex = 0;

            for (int i = 0; i < EntityOrder.Count; i++)
            {
                BattleEntity entity = EntityOrder[i];
                if (entity.IsEnemy)
                {
                    Enemies.Add((BattleEnemy)entity);
                    entity.EntitySprite.Position = EnemyPositions[enemyindex];
                    enemyindex++;
                }
                else
                {
                    Players.Add((BattlePlayer)entity);
                    entity.EntitySprite.Position = PlayerPositions[playerindex];
                    playerindex++;
                }
            }

            BattleState = BattleStates.TurnDone;
        }

        private void TurnStart()
        {
            BattleEntity entity = CurrentEntityTurn;

            HeaderBox.SetText(entity.Name + "'s turn!");

            entity.StartTurn();
            BattleState = BattleStates.Combat;
        }

        public void TurnEnd()
        {
            BattleState = BattleStates.TurnDone;

            UpdateBattleState();

            if (BattleState == BattleStates.Victory)
            {
                //Start victory here
                HeaderBox.SetText("Victory!");
                return;
            }
            else if (BattleState == BattleStates.GameOver)
            {
                //Start game over here
                HeaderBox.SetText("Game over...");
                return;
            }

            //Failsafe if all entities are dead
            int deadattempts = 0;

            //Check for the next entity that isn't dead
            do
            {
                CurEntityTurn = Helper.Wrap(CurEntityTurn + 1, 0, EntityOrder.Count - 1);
                deadattempts++;
            }
            while (CurrentEntityTurn.IsDead == true && deadattempts < EntityOrder.Count);

            //Everyone is dead, so update the battle state, which will make the players lose
            if (deadattempts >= EntityOrder.Count)
            {
                UpdateBattleState();
            }
        }

        //Update the battle state after each turn
        private void UpdateBattleState()
        {
            if (BattleState == BattleStates.Victory || BattleState == BattleStates.GameOver)
                return;

            bool allDead = true;

            //Check players first, since if both all players and all enemies are dead on the same turn, players get a game over
            for (int i = 0; i < Players.Count; i++)
            {
                //Check for Dead status
                if (Players[i].IsDead == false)
                {
                    allDead = false;
                    break;
                }
            }

            //All players are dead - GameOver
            if (allDead == true)
            {
                BattleState = BattleStates.GameOver;
                return;
            }

            allDead = true;

            for (int i = 0; i < Enemies.Count; i++)
            {
                //Check for Dead status
                if (Enemies[i].IsDead == false)
                {
                    allDead = false;
                }
            }

            //All enemies are dead - Victory
            if (allDead == true)
            {
                BattleState = BattleStates.Victory;
            }
        }

        public void Update()
        {
            if (BattleState == BattleStates.TurnDone)
                TurnStart();

            //This update is for the current entity's turn
            CurrentEntityTurn.TurnUpdate();

            //This update is for animations, effects, and etc.
            for (int i = 0; i < EntityOrder.Count; i++)
            {
                EntityOrder[i].Update();
            }
        }

        public void Draw()
        {
            Background?.Draw(GameCore.GameWindow, RenderStates.Default);
            HeaderBox?.Draw();

            for (int i = 0; i < EntityOrder.Count; i++)
            {
                EntityOrder[i].Draw();
            }
        }
    }
}
