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

        protected readonly Dictionary<string, StatusEffect> AfflictedStatuses = new Dictionary<string, StatusEffect>();
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
            StatusEffect[] statuses = AfflictedStatuses.Values.ToArray();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].OnTurnStart();
            }
        }

        public virtual void OnTurnEnd()
        {
            StatusEffect[] statuses = AfflictedStatuses.Values.ToArray();
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i].OnTurnEnd();
            }
        }

        public void EndTurn()
        {
            OnTurnEnd();
            BattleManager.Instance.TurnEnd();
        }

        /// <summary>
        /// Gets the DamageType of the Entity's attack action
        /// </summary>
        /// <returns></returns>
        public virtual DamageTypes GetAttackDamageType()
        {
            return DamageTypes.Physical;
        }

        /// <summary>
        /// Gets the Element of the Entity's attack action
        /// </summary>
        /// <returns></returns>
        public virtual Elements GetAttackElement()
        {
            return Elements.Neutral;
        }

        /// <summary>
        /// Calculates the total damage this entity will deal to another entity
        /// </summary>
        /// <returns>The amount of total damage this entity will deal</returns>
        public int CalculateDamageDealt(/*DamageTypes damageType, Elements element*/)
        {
            //NOTE: Figure out how to handle None damage here. Default to Attack for now
            return Helper.Clamp(/*damageType == DamageTypes.Magic ? MagicAtk : */Attack, Globals.MIN_DMG, Globals.MAX_DMG);
        }

        /// <summary>
        /// Calculates total damage received based on this entity's defense, magic defense, and/or weaknesses and resistances, with the
        /// latter being applied after defense reduction
        /// </summary>
        /// <param name="damage">The damage value from the damage source</param>
        /// <param name ="damageType">The type of damage dealt</param>
        /// <param name="element">The elemental damage being dealt</param>
        /// <returns>The amount of total damage dealt to this entity</returns>
        public int CalculateDamageReceived(int damage, DamageTypes damageType, Elements element)
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
                    totaldamage = (int)((float)totaldamage * Globals.WEAKNESS_MOD);
                }

                //Apply resistance modifier if the entity resists this element
                //The damage will cancel out if the entity both resists and is weak to this element (possible with equipment, spells, etc.)
                if (Resistances.ContainsKey(element))
                {
                    totaldamage = (int)((float)totaldamage * Globals.RESISTANCE_MOD);
                }
            }

            return Helper.Clamp(totaldamage, Globals.MIN_DMG, Globals.MAX_DMG);
        }

        /// <summary>
        /// Deals damage to one or more entities, with modifiers
        /// </summary>
        /// <param name="entities">The entities to deal damage to</param>
        public void AttackEntity(params BattleEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Debug.Log(Name + " attacked " + entities[i].Name + "!");
                entities[i].TakeDamage(CalculateDamageDealt(), GetAttackDamageType(), GetAttackElement());
            }
        }

        public void ModifyHP(int value)
        {
            CurHP = Helper.Clamp(value, 0, MaxHP);
        }

        public void ModifyMP(int value)
        {
            CurMP = Helper.Clamp(value, 0, MaxMP);
        }

        public void TakeDamage(int damage, DamageTypes damagetype, Elements element)
        {
            int totaldamage = CalculateDamageReceived(damage, damagetype, element);
            ModifyHP(CurHP - totaldamage);

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
            ModifyHP(CurHP + (int)hp);
            ModifyMP(CurMP + (int)mp);

            OnEntityHeal?.Invoke(this);
        }

        /// <summary>
        /// Inflicts one or more StatusEffects on the entity
        /// </summary>
        /// <param name="statuses">The StatusEffets to inflict on the entity</param>
        public void InflictStatus(params StatusEffect[] statuses)
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                StatusEffect status = statuses[i];

                if (status == null)
                {
                    Debug.LogError($"Status being inflicted on {Name} at index {i} is null!");
                    continue;
                }

                string statusName = status.Name;

                if (AfflictedStatuses.ContainsKey(statusName))
                {
                    AfflictedStatuses[statusName].Refresh();
                    Debug.Log($"Refreshed status {AfflictedStatuses[statusName].Name} on {Name}!");
                }
                else
                {
                    status.OnStatusFinished += OnStatusFinished;
                    AfflictedStatuses.Add(statusName, status);
                    AfflictedStatuses[statusName].SetReceiver(this);
                    AfflictedStatuses[statusName].OnInflict();
                    Debug.Log($"Inflicted status {AfflictedStatuses[statusName].Name} on {Name}!");
                }
            }
            OnEntityStatus?.Invoke(this);
        }

        /// <summary>
        /// Removes all StatusEffects affecting an entity.
        /// Functions the same as RemoveStatus
        /// </summary>
        public void ClearAllStatuses()
        {
            RemoveStatus(AfflictedStatuses.Keys.ToArray());
        }

        /// <summary>
        /// Removes a set of StatusEffects affecting an entity.
        /// Each StatusEffect's OnEnd() method is called and the entity unsubscribes from the status finish events
        /// </summary>
        /// <param name="statuses">The names of the StatusEffets to remove</param>
        public void RemoveStatus(params string[] statuses)
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                string statusName = statuses[i];
                if (AfflictedStatuses.ContainsKey(statusName))
                {
                    StatusEffect status = AfflictedStatuses[statusName];
                    status.OnEnd();

                    OnStatusFinished(status);
                }
            }
        }

        private void OnStatusFinished(StatusEffect status)
        {
            status.OnStatusFinished -= OnStatusFinished;

            if (AfflictedStatuses.ContainsKey(status.Name))
            {
                AfflictedStatuses.Remove(status.Name);
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
