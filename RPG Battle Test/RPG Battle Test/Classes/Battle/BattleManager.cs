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
            All, Alive, Dead
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
        public List<BattleEntity> Entities = new List<BattleEntity>();
        public List<BattleEntity> Enemies = new List<BattleEntity>();
        public List<BattleEntity> Players = new List<BattleEntity>();
        private List<BattleEntity> TurnOrder = null;

        public Inventory PartyInventory = new Inventory();

        //The current state of the battle
        public BattleStates BattleState { get; private set; } = BattleStates.Init;

        private readonly List<Vector2f> EnemyPositions = null;
        private readonly List<Vector2f> PlayerPositions = null;

        public bool IsBattleOver => BattleState == BattleStates.Victory || BattleState == BattleStates.GameOver;

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

        public BattleEntity CurrentEntityTurn => TurnOrder[0];

        public void Start(params BattleEntity[] entities)
        {
            //Unsubscribe first then subscribe, in case the battle was started again before being disposed
            BattleEntity.EntityDeathEvent -= OnEntityDeath;
            BattleEntity.EntityDeathEvent += OnEntityDeath;

            BattleState = BattleStates.Init;

            AddEntities(entities);

            TurnOrder = new List<BattleEntity>(Entities);
            TurnOrder.Sort(SortBySpeed);

            BattleState = BattleStates.TurnDone;

            BattleUIManager.Instance.Start();

            //Initialize static battle components
            BattlePlayer.BattleStart();
        }

        /// <summary>
        /// Ends the combat part of the battle - Victory and results if 
        /// </summary>
        public void EndBattle(bool victory)
        {
            BattleState = victory ? BattleStates.Victory : BattleStates.GameOver;

            Debug.Log($"The battle has ended in {BattleState}!");

            CurrentEntityTurn?.EndTurn(true);

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].OnBattleEnd();
            }
        }

        public void Dispose()
        {
            BattleUIManager.Instance.Dispose();
            PartyInventory.Dispose();

            RemoveEntities(Entities);

            //Clear instance and event
            BattleEntity.EntityDeathEvent -= OnEntityDeath;

            instance = null;
        }
        
        /// <summary>
        /// Adds BattleEntities to the battle. Any new entities added will not be able to move until the next turn cycle
        /// </summary>
        /// <param name="entities">The BattleEntities to add to the battle</param>
        public void AddEntities(params BattleEntity[] entities)
        {
            //Add all entities to the global list
            Entities.AddRange(entities);

            //Go through all the entities and add them to the proper Player or Enemy lists and initialize them
            for (int i = 0; i < entities.Length; i++)
            {
                BattleEntity entity = Entities[i];
                if (entity.IsEnemy)
                {
                    Enemies.Add(entity);
                    int enemyIndex = (Enemies.Count - 1) % EnemyPositions.Count;
                    entity.Position = EnemyPositions[enemyIndex];
                }
                else
                {
                    Players.Add(entity);
                    int playerIndex = (Players.Count - 1) % PlayerPositions.Count;
                    entity.Position = PlayerPositions[playerIndex];
                }

                entity.OnBattleStart();
            }
        }

        /// <summary>
        /// Removes BattleEntities from the battle.
        /// </summary>
        /// <param name="entities">A list of BattleEntities to remove from the battle</param>
        private void RemoveEntities(List<BattleEntity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];
                if (Entities.Remove(entity) == false)
                {
                    Debug.LogError($"CRITICAL ERROR! {entity.Name} is not in the {nameof(Entities)} list!");
                    continue;
                }

                //Remove from turn order
                TurnOrder.Remove(entity);

                //Remove from the respective Enemy or Player list
                if (entity.IsEnemy) Enemies.Remove(entity);
                else Players.Remove(entity);

                //Dispose the entity
                entity.Dispose();
            }
        }

        /// <summary>
        /// Removes BattleEntities from the battle.
        /// </summary>
        /// <param name="entities">The BattleEntities to remove from the battle</param>
        public void RemoveEntities(params BattleEntity[] entities)
        {
            RemoveEntities(new List<BattleEntity>(entities));
        }

        private void TurnStart()
        {
            if (IsBattleOver == true)
            {
                Debug.LogError("Trying to start a turn when the battle is over!");
                return;
            }

            BattleEntity entity = CurrentEntityTurn;

            Debug.LogWarning($"Started {entity.Name}'s Turn!");

            BattleState = BattleStates.Combat;

            entity.StartTurn();

            if (IsBattleOver == false && entity.IsTurn == true)
            {
                BattleUIManager.Instance.SetHeaderText(entity.Name + "'s turn!");
            }
        }

        public void TurnEnd()
        {
            if (IsBattleOver == true)
            {
                Debug.LogError("Trying to end a turn when the battle is over!");
                return;
            }

            Debug.LogWarning($"Ended {CurrentEntityTurn.Name}'s Turn!");

            //Update the next turn cycle
            TurnOrder.RemoveAt(0);

            //Start the cycle over
            if (TurnOrder.Count == 0)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    //Add only alive entities
                    if (Entities[i].IsDead == false)
                    {
                        TurnOrder.Add(Entities[i]);
                    }
                }
            }

            TurnOrder.Sort(SortBySpeed);

            BattleState = BattleStates.TurnDone;

            UpdateBattleState();
        }

        //Update the battle state after each turn
        private void UpdateBattleState()
        {
            if (IsBattleOver == true)
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
                EndBattle(false);
                BattleUIManager.Instance.SetHeaderText("Game over...");
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
                EndBattle(true);
                BattleUIManager.Instance.SetHeaderText("Victory!");
            }
        }

        private int SortBySpeed(BattleEntity entity1, BattleEntity entity2)
        {
            if (entity1.TrueSpeed > entity2.TrueSpeed)
                return -1;
            if (entity1.TrueSpeed < entity2.TrueSpeed)
                return 1;
            return 0;
        }

        /// <summary>
        /// Removes the Entity that died from the turn order
        /// </summary>
        /// <param name="entity">The entity that died</param>
        private void OnEntityDeath(BattleEntity entity)
        {
            //Prevent removing if it is the Entity's turn
            //It would mess up the turn order because the next Entity would be removed from the list instead of the current one
            //It's possible for an Entity to die on its own turn through Poison or other Status Effects
            if (entity.IsTurn == false)
            {
                TurnOrder.Remove(entity);
            }

            //Update the battle state as the current entity could have more than one action per turn
            UpdateBattleState();
        }

        /// <summary>
        /// Returns a group of entities
        /// </summary>
        /// <param name="entityType">The type of entity </param>
        /// <param name="filterState">Include all entities only from this filter in the returned list</param>
        /// <returns>A list of the entities of the specified type</returns>
        public List<BattleEntity> GetEntityGroup(BattleEntity.EntityTypes entityType, EntityFilterStates filterState)
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
            if (filterState == EntityFilterStates.Alive)
            {
                list.RemoveAll((entity) => entity.IsDead == true);
            }
            //Filter out alive entities
            else if (filterState == EntityFilterStates.Dead)
            {
                list.RemoveAll((entity) => entity.IsDead == false);
            }

            return list;
        }

        /// <summary>
        /// Returns a random entity specified by entitytype
        /// </summary>
        /// <param name="entitytype">The type of entity to select. If None is passed in, choose from any entity in the battle</param>
        /// <param name="filterState">The filter to apply to the random selection</param>
        public BattleEntity SelectRandomEntity(BattleEntity.EntityTypes entitytype, EntityFilterStates filterState)
        {
            List<BattleEntity> entitylist = GetEntityGroup(entitytype, filterState);

            return entitylist?[Globals.Randomizer.Next(0, entitylist.Count)];
        }

        public void Update()
        {
            if (BattleState == BattleStates.TurnDone)
                TurnStart();
            
            //If an entity's turn ended immediately after it started via a StatusEffect, don't update the next entity yet
            if (BattleState != BattleStates.TurnDone && IsBattleOver == false)
            {
                //This update is for the current entity's turn
                CurrentEntityTurn.TurnUpdate();
            }

            //This update is for animations, effects, and etc.
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update();
            }

            BattleUIManager.Instance.Update();
        }

        public void Draw()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Draw();
            }

            BattleUIManager.Instance.Draw();
        }
    }
}
