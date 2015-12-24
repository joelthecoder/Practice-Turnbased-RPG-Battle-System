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
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    public abstract class BattleEntity
    {
        public enum EntityTypes
        {
            None, Player, Enemy
        }

        public delegate void EntityHeal(BattleEntity entity);
        public delegate void EntityDamage(BattleEntity entity);
        public delegate void EntityStatus(BattleEntity entity);

        public static event EntityHeal OnEntityHeal = null;
        public static event EntityDamage OnEntityDamage = null;
        public static event EntityStatus OnEntityStatus = null;

        public Sprite EntitySprite = null;
        public int NumActions = 1;

        //Stats
        public string Name = "ERROR";
        public int CurHP { get; protected set; } = 10;
        public int CurMP { get; protected set; } = 0;
        public int MaxHP { get; protected set; } = 10;
        public int MaxMP { get; protected set; } = 0;
        public int Attack { get; protected set; } = 5;
        public int MagicAtk { get; protected set; } = 0;
        public int Defense { get; protected set; } = 0;
        public int MagicDef { get; protected set; } = 0;
        public int Speed { get; protected set; } = 1;

        protected readonly Dictionary<StatusEffect, StatusEffect> AfflictedStatuses = new Dictionary<StatusEffect, StatusEffect>();
        protected readonly Dictionary<Elements, Elements> Resistances = new Dictionary<Elements, Elements>();
        protected readonly Dictionary<Elements, Elements> Weaknesses = new Dictionary<Elements, Elements>();

        public readonly Color TurnColor = Color.Cyan;
        public EntityTypes EntityType { get; protected set; } = EntityTypes.None;

        public static bool IsEntityEnemy(BattleEntity entity) => entity.EntityType == EntityTypes.Enemy;
        public static bool IsEntityPlayer(BattleEntity entity) => entity.EntityType == EntityTypes.Player;

        public bool IsEnemy => EntityType == EntityTypes.Enemy;
        public bool IsPlayer => EntityType == EntityTypes.Player;

        public bool IsTurn => this == BattleManager.Instance.CurrentEntityTurn;
        public bool IsDead => CurHP <= 0;

        public BattleEntity()
        {
            
        }

        public Vector2f Position
        {
            get
            {
                return EntitySprite.Position;
            }
            set
            {
                EntitySprite.Position = value;
            }
        }

        //Battle-turn related methods
        public virtual void StartTurn()
        {
            StatusEffect[] statuses = AfflictedStatuses.Keys.ToArray();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].OnTurnStart();
            }
        }

        public virtual void OnTurnEnd()
        {
            StatusEffect[] statuses = AfflictedStatuses.Keys.ToArray();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].OnTurnEnd();
            }
        }

        public void EndTurn()
        {
            BattleManager.Instance.TurnEnd();
            OnTurnEnd();
        }

        /// <summary>
        /// Performs a battle command
        /// </summary>
        public void UseCommand(BattleCommand command)
        {
            command.Perform();
        }

        /// <summary>
        /// Calculates the total damage this entity will deal to another entity
        /// </summary>
        /// <returns>The amount of total damage this entity will deal</returns>
        private int CalculateDamageDealt()
        {
            return Helper.Clamp(Attack, 0, int.MaxValue);
        }

        /// <summary>
        /// Calculates total damage received based on this entity's defense, magic defense, and/or weaknesses and resistances, with the
        /// latter being applied after defense reduction
        /// </summary>
        /// <param name="damage">The damage value from the damage source</param>
        /// <param name ="damageType">The type of damage dealt</param>
        /// <param name="element">The elemental damage being dealt</param>
        /// <returns>The amount of total damage dealt to this entity</returns>
        private int CalculateDamageReceived(int damage, DamageTypes damageType, Elements element)
        {
            int totaldamage = damage;
            if (damageType == DamageTypes.Physical)
            {
                totaldamage -= Defense;
            }
            else if (damageType == DamageTypes.Magic)
            {
                totaldamage -= MagicDef;
            }

            if (element != Elements.Neutral)
            {
                //Apply weakness modifier if the entity is weak to this element
                if (Weaknesses.ContainsKey(element))
                {
                    totaldamage = (int)((float)totaldamage * Globals.WeaknessModifier);
                }

                //Apply resistance modifier if the entity resists this element
                //The damage will cancel out if the entity both resists and is weak to this element (possible with equipment, spells, etc.)
                if (Resistances.ContainsKey(element))
                {
                    totaldamage = (int)((float)totaldamage * Globals.ResistanceModifier);
                }
            }

            return Helper.Clamp(totaldamage, Globals.MinDamage, Globals.MaxDamage);
        }

        /// <summary>
        /// Deals damage to an entity, with modifiers
        /// </summary>
        /// <param name="entity">The entity to deal damage to</param>
        public void AttackEntity(BattleEntity entity)
        {
            Debug.Log(Name + " attacked " + entity.Name + "!");
            entity.TakeDamage(CalculateDamageDealt(), DamageTypes.Physical, Elements.Neutral);
        }

        public void TakeDamage(int damage, DamageTypes damagetype, Elements element)
        {
            int totaldamage = CalculateDamageReceived(damage, damagetype, element);
            CurHP = Helper.Clamp(CurHP - totaldamage, 0, MaxHP);

            Debug.Log($"{Name} received {totaldamage} damage!");

            OnEntityDamage?.Invoke(this);
        }

        //Battle-combat related methods
        /// <summary>
        /// Restores an Entity's HP and MP values by the given amount
        /// </summary>
        /// <param name="hp">The amount of HP to restore</param>
        /// <param name="mp">The amount of MP to restore</param>
        public void Restore(uint hp, uint mp)
        {
            CurHP = Helper.Clamp(CurHP + (int)hp, 0, MaxHP);
            CurMP = Helper.Clamp(CurMP + (int)mp, 0, MaxMP);

            OnEntityHeal?.Invoke(this);
        }

        /// <summary>
        /// Inflicts one or more StatusEffects on the entity
        /// </summary>
        /// <param name="statuses">The statuses to inflict on the entity</param>
        public void InflictStatus(params StatusEffect[] statuses)
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                StatusEffect status = statuses[i];

                if (AfflictedStatuses.ContainsKey(status))
                {
                    AfflictedStatuses[status].Refresh();
                    Debug.Log($"Refreshed status {AfflictedStatuses[status].Name} on {Name}!");
                }
                else
                {
                    status.OnStatusFinished += OnStatusFinished;
                    AfflictedStatuses.Add(status, status);
                    AfflictedStatuses[status].OnInflict();
                    Debug.Log($"Inflicted status {AfflictedStatuses[status].Name} on {Name}!");
                }
            }
            OnEntityStatus?.Invoke(this);
        }

        private void OnStatusFinished(StatusEffect status)
        {
            status.OnStatusFinished -= OnStatusFinished;

            if (AfflictedStatuses.ContainsKey(status))
            {
                AfflictedStatuses.Remove(status);
                Debug.Log($"{status.Name} ended on {Name}!");
            }
            else
            {
                Debug.LogError($"{Name} is not afflicted with the {status.Name} status!");
            }
        }

        public void CleanUp()
        {
            
        }

        /// <summary>
        /// The update called during the entity's turn; for input and other stuff
        /// </summary>
        public virtual void TurnUpdate()
        {

        }

        /// <summary>
        /// This update is for animations, status effects, and so on; nothing turn related
        /// </summary>
        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            if (IsDead == false)
            {
                GameCore.spriteSorter.Add(EntitySprite, Constants.BASE_ENTITY_LAYER + (Position.Y / 1000f));
            }
        }
    }
}
