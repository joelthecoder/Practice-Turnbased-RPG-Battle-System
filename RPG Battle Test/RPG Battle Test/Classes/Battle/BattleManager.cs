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
    /// Handles turns and the state of turn-based battles
    /// This is a Singleton
    /// </summary>
    public sealed class BattleManager : IDisposable
    {
        public enum BattleStates
        {
            Init, Combat, TurnDone, Victory, GameOver
        }

        public enum EntityFilterStates
        {
            Both, Alive, Dead
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
        public List<BattleEntity> EntityOrder = null;
        public List<BattleEntity> Enemies = new List<BattleEntity>();
        public List<BattleEntity> Players = new List<BattleEntity>();
        private int CurEntityTurn = 0;

        public Inventory PartyInventory = new Inventory();

        //The current state of the battle
        public BattleStates BattleState { get; private set; } = BattleStates.Init;

        private readonly List<Vector2f> EnemyPositions = null;
        private readonly List<Vector2f> PlayerPositions = null;

        private BattleManager()
        {
            EnemyPositions = new List<Vector2f>()
            {
                new Vector2f(300, 175),
                new Vector2f(175, 250),
                new Vector2f(275, 325)
            };

            PlayerPositions = new List<Vector2f>()
            {
                new Vector2f(525, 225),
                new Vector2f(525, 325)
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
                    entity.Position = EnemyPositions[enemyindex];
                    enemyindex++;
                }
                else
                {
                    Players.Add((BattlePlayer)entity);
                    entity.Position = PlayerPositions[playerindex];
                    playerindex++;
                }
            }

            BattleState = BattleStates.TurnDone;

            BattleUIManager.Instance.Start();

            //Initialize static battle components
            BattlePlayer.OnBattleStart();
        }

        public void Dispose()
        {
            BattleUIManager.Instance.Dispose();
            PartyInventory.Dispose();

            for (int i = 0; i < EntityOrder.Count; i++)
            {
                EntityOrder[i].Dispose();
            }
        }

        private void TurnStart()
        {
            BattleEntity entity = CurrentEntityTurn;

            BattleUIManager.Instance.SetHeaderText(entity.Name + "'s turn!");

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
                BattleUIManager.Instance.SetHeaderText("Victory!");
                return;
            }
            else if (BattleState == BattleStates.GameOver)
            {
                //Start game over here
                BattleUIManager.Instance.SetHeaderText("Game over...");
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

        /// <summary>
        /// Returns a group of entities
        /// </summary>
        /// <param name="entityType">The type of entity </param>
        /// <param name="filterDead">If true, exclude all dead entities from the returned list</param>
        /// <returns>A list of the entities of the specified type</returns>
        public List<BattleEntity> GetEntityGroup(BattleEntity.EntityTypes entityType, bool filterDead)
        {
            List<BattleEntity> list = new List<BattleEntity>();

            if (entityType == BattleEntity.EntityTypes.Player)
            {
                list.AddRange(Players);
            }
            else if (entityType == BattleEntity.EntityTypes.Enemy)
            {
                list.AddRange(Enemies);
            }
            else
            {
                //Have it separated by enemies first, then players - more organized
                list.AddRange(Enemies);
                list.AddRange(Players);
            }

            //Filter out dead entities
            if (filterDead == true)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].IsDead == true)
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a random entity specified by entitytype
        /// </summary>
        /// <param name="entitytype">The type of entity to select. If None is passed in, choose from any entity in the battle</param>
        /// <param name="chooseDead">Specifies whether a dead entity should be chosen</param>
        public BattleEntity SelectRandomEntity(BattleEntity.EntityTypes entitytype, bool chooseDead)
        {
            List<BattleEntity> entitylist = GetEntityGroup(entitytype, chooseDead);

            return entitylist?[Globals.Randomizer.Next(0, entitylist.Count)];
        }

        public void Update()
        {
            if (BattleState == BattleStates.TurnDone)
                TurnStart();

            if (BattleState != BattleStates.GameOver && BattleState != BattleStates.Victory)
            {
                //This update is for the current entity's turn
                CurrentEntityTurn.TurnUpdate();
            }

            //This update is for animations, effects, and etc.
            for (int i = 0; i < EntityOrder.Count; i++)
            {
                EntityOrder[i].Update();
            }

            BattleUIManager.Instance.Update();
        }

        public void Draw()
        {
            for (int i = 0; i < EntityOrder.Count; i++)
            {
                EntityOrder[i].Draw();
            }

            BattleUIManager.Instance.Draw();
        }
    }
}
