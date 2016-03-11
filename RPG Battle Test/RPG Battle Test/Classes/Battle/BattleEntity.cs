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
    public abstract class BattleEntity : IDisposable
    {
        public enum EntityTypes
        {
            None, Player, Enemy
        }

        public delegate void TurnStart();
        public delegate void TurnEnd();

        public delegate void EntityDeath(BattleEntity entity);

        public event TurnStart TurnStartEvent = null;
        public event TurnEnd TurnEndEvent = null;
        public static event EntityDeath EntityDeathEvent = null;

        public Sprite EntitySprite = null;

        /// <summary>
        /// The number of actions the Entity can perform this turn.
        /// If modifying this value via a StatusEffect, put it in the StatusEffect's OnTurnStart() method.
        /// <para>Once this value is set to 0, it means that the Entity's turn should end and will ignore any value changes
        /// until the start of the Entity's next turn.</para>
        /// </summary>
        public uint NumActions { get; protected set; } = 1;

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

        /// <summary>
        /// The Speed of the entity after all StatModifiers are factored in
        /// </summary>
        public int TrueSpeed => CalculateTotalStat(StatModifiers.StatModTypes.Speed);

        /// <summary>
        /// The current BattleCommand the entity is about to perform.
        /// For players, once the command is set, the player selects the target(s)
        /// </summary>
        protected BattleCommand CurrentCommand = null;

        /// <summary>
        /// The command used on the previous turn
        /// </summary>
        protected BattleCommand PreviousCommand = null;

        /// <summary>
        /// The StatusEffects the entity is afflicted with. The key is the status' Type, which is used as a unique identifier.
        /// This allows for more flexibility; an enum or other identifier can be used but may require more maintenance
        /// </summary>
        protected readonly Dictionary<Type, StatusEffect> AfflictedStatuses = new Dictionary<Type, StatusEffect>();
        protected readonly Dictionary<string, Spell> LearnedSpells = new Dictionary<string, Spell>();
        protected readonly Dictionary<Elements, Elements> Resistances = new Dictionary<Elements, Elements>();
        protected readonly Dictionary<Elements, Elements> Weaknesses = new Dictionary<Elements, Elements>();
        //protected readonly Dictionary<string, BattleCommand> KnownCommands = new Dictionary<string, BattleCommand>();

        protected readonly StatModifiers StatModifications = new StatModifiers();

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

        public virtual void OnBattleStart()
        {
            
        }

        public virtual void OnBattleEnd()
        {
            ClearAllStatuses();
            ClearStatModifiers();
        }

        //Battle-turn related methods
        public void StartTurn()
        {
            //Default to one action. If a StatusEffect changes this, it'll happen in the TurnStartEvent
            NumActions = 1;

            TurnStartEvent?.Invoke();
            HandleTurnStart();

            /*NOTE: We check if NumActions is 0 here then end the turn if so. If NumActions is already 0, don't allow it
            to be set to any other value in ModifyNumActions().
            This fixes problems with Sleep, Fast, and other turn-modifying StatusEffects on an Entity at the same time*/
            if (NumActions == 0)
            {
                EndTurn();
            }
        }

        protected virtual void OnTurnStarted()
        {
            
        }

        public void EndTurn(bool forced = false)
        {
            //StatusEffects can have this at 0 before it gets here
            if (NumActions > 0)
                NumActions--;

            //If the entity is out of actions for this turn, end the turn
            if (forced == true || NumActions == 0)
            {
                TurnEndEvent?.Invoke();
                HandleTurnEnd();

                NumActions = 0;

                BattleManager.Instance.TurnEnd();
            }
            //Otherwise restart the turn
            else
            {
                RestartTurn();
            }
        }

        protected virtual void OnTurnEnded()
        {
            PreviousCommand = CurrentCommand;
            CurrentCommand = null;
        }

        /// <summary>
        /// Restarts the Entity's turn, allowing it to make another action
        /// </summary>
        protected void RestartTurn()
        {
            HandleTurnEnd();
            HandleTurnStart();
        }

        /// <summary>
        /// Calls OnTurnStarted() and performs anything else afterwards that should occur for all BattleEntities
        /// </summary>
        private void HandleTurnStart()
        {
            if (NumActions == 0)
                return;

            OnTurnStarted();
            if (PreviousCommand?.IsCommandFinished == false)
            {
                PreviousCommand.Reperform();
                EndTurn();
            }
        }

        /// <summary>
        /// Calls OnTurnEnded() and performs anything else afterwards that should occur for all BattleEntities
        /// </summary>
        private void HandleTurnEnd()
        {
            OnTurnEnded();
        }

        /// <summary>
        /// Modifies how many actions the Entity can perform this turn
        /// </summary>
        /// <param name="numActions">The number of actions to allow the Entity to perform this turn.
        /// This value is clamped by the min and max turns, defined in Globals, and if the Entity's actions are already at 
        /// 0, it won't change its value</param>
        /// <returns>false if the Entity's action count is 0 and true otherwise</returns>
        public bool ModifyNumActions(uint numActions)
        {
            if (NumActions == MIN_ENTITY_TURNS)
                return false;

            NumActions = Helper.Clamp(numActions, MIN_ENTITY_TURNS, MAX_ENTITY_TURNS);
            return true;
        }

        /// <summary>
        /// Interrupts the command the entity is using. For multi-turn commands like casting spells, this'll cancel the spell
        /// </summary>
        public void InterruptCommand()
        {
            PreviousCommand?.Interrupt();
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
        public int CalculateDamageDealt(DamageTypes damageType, Elements element)
        {
            int totalDamage = 0;

            //Calculate physical attack
            if (damageType == DamageTypes.Physical)
            {
                totalDamage = CalculateTotalStat(StatModifiers.StatModTypes.Attack);
            }
            //Calculate magic attack
            else if (damageType == DamageTypes.Magic)
            {
                totalDamage = CalculateTotalStat(StatModifiers.StatModTypes.MagicAtk);
            }
            //Use the higher of the total physical or magical attack if damageType is None
            else
            {
                int physical = CalculateTotalStat(StatModifiers.StatModTypes.Attack);
                int magic = CalculateTotalStat(StatModifiers.StatModTypes.MagicAtk);

                totalDamage = physical >= magic ? physical : magic;
            }

            return Helper.Clamp(totalDamage, Globals.MIN_DMG, Globals.MAX_DMG);
        }

        /// <summary>
        /// Calculates the total value of a stat, factoring in StatModifiers
        /// </summary>
        /// <param name="statModType">The type of the stat to apply StatModifiers to</param>
        /// <returns>The total value of the stat with all StatModifiers factored in</returns>
        protected int CalculateTotalStat(StatModifiers.StatModTypes statModType)
        {
            int stat = GetStatForStatMod(statModType);
            int total = (int)((stat + StatModifications.GetModifierAmount(statModType)) * StatModifications.GetModifierPercent(statModType));
            /*Debug.Log($"BASE STAT OF TYPE {statModType}: {stat}");
            Debug.Log($"AMOUNT MODIFIER: {StatModifications.GetModifierAmount(statModType)}");
            Debug.Log($"PERCENTAGE MODIFIER: {StatModifications.GetModifierPercent(statModType)}");
            Debug.Log($"TOTAL: {total}");*/
            return total;
        }

        /// <summary>
        /// Returns the value of the appropriate stat based on the StatModType passed in
        /// </summary>
        /// <param name="statModType">The type of the stat</param>
        /// <returns>The value of the stat that corresponds to the StatModType passed in</returns>
        protected int GetStatForStatMod(StatModifiers.StatModTypes statModType)
        {
            switch (statModType)
            {
                default:
                case StatModifiers.StatModTypes.Attack: return Attack;
                case StatModifiers.StatModTypes.Defense: return Defense;
                case StatModifiers.StatModTypes.MagicAtk: return MagicAtk;
                case StatModifiers.StatModTypes.MagicDef: return MagicDef;
                case StatModifiers.StatModTypes.Speed: return Speed;
            }
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
                totaldamage -= CalculateTotalStat(StatModifiers.StatModTypes.Defense);
            }
            else if (damageType == DamageTypes.Magic)
            {
                totaldamage -= CalculateTotalStat(StatModifiers.StatModTypes.MagicDef);
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

        protected void ModifyHP(int value)
        {
            CurHP = Helper.Clamp(value, 0, MaxHP);

            //Death; clear all statuses and status modifiers
            if (CurHP == 0)
            {
                Kill();
            }
        }

        protected void ModifyMP(int value)
        {
            CurMP = Helper.Clamp(value, 0, MaxMP);
        }

        /// <summary>
        /// Instantly kills the BattleEntity
        /// </summary>
        public void Kill()
        {
            CurHP = 0;

            ClearAllStatuses();
            ClearStatModifiers();
            InterruptCommand();

            EntityDeathEvent?.Invoke(this);

            Debug.Log($"{Name} has fallen in battle!");
        }

        /// <summary>
        /// Deals damage to the Entity, subtracting from its HP
        /// </summary
        /// <param name="affectableInfo">Contains the Entity and what it dealt damage with</param>
        /// <param name="damage">The amount of total damage dealt</param>
        /// <param name="damagetype">The type of damage dealt</param>
        /// <param name="element">The element of the damage</param>
        public void TakeDamage(AffectableInfo affectableInfo, int damage, DamageTypes damagetype, Elements element)
        {
            int totaldamage = CalculateDamageReceived(damage, damagetype, element);
            ModifyHP(CurHP - totaldamage);

            BattleUIManager.Instance.AddElement(new UIDamageTextDisplay(damagetype, element, .75f, totaldamage.ToString(),
            new Vector2f(Position.X, Position.Y - 50f), Globals.BASE_UI_LAYER + .6f));

            OnDamageReceived(affectableInfo, totaldamage, damagetype, element);
        }

        /// <summary>
        /// Drains MP from the Entity
        /// </summary>
        /// <param name="affectableInfo">Contains the Entity and what it drained MP with</param>
        /// <param name="mp">The total amount of MP to drain</param>
        public void DrainMP(AffectableInfo affectableInfo, int mp)
        {
            ModifyMP(CurMP - mp);

            Debug.Log($"{Name} was drained of {mp} MP!");
        }

        //Battle-combat related methods
        /// <summary>
        /// Restores an Entity's HP and MP values by the given amount
        /// </summary>
        /// <param name="affectableInfo">Contains the Entity and what it restored HP/MP with</param>
        /// <param name="hp">The amount of HP to restore</param>
        /// <param name="mp">The amount of MP to restore</param>
        public void Restore(AffectableInfo affectableInfo, uint hp, uint mp)
        {
            ModifyHP(CurHP + (int)hp);
            ModifyMP(CurMP + (int)mp);

            if (hp != 0)
            {
                //Emerald green
                BattleUIManager.Instance.AddElement(new UIDamageTextDisplay(.75f, hp.ToString(),
                new Vector2f(Position.X, Position.Y - 50f), new Color(85, 212, 63, 255), Globals.BASE_UI_LAYER + .6f));
            }
            if (mp != 0)
            {
                //Turquoise-ish
                BattleUIManager.Instance.AddElement(new UIDamageTextDisplay(.75f, mp.ToString(),
                new Vector2f(Position.X, Position.Y - 50f), new Color(72, 241, 241, 255), Globals.BASE_UI_LAYER + .6f));
            }
        }

        /// <summary>
        /// What occurs when the Entity receives damage
        /// </summary
        /// <param name="affectableInfo">Contains the Entity and what it dealt damage with</param>
        /// <param name="totaldamage">The amount of total damage received</param>
        /// <param name="damagetype">The type of damage dealt</param>
        /// <param name="element">The element of the damage</param>
        public virtual void OnDamageReceived(AffectableInfo affectableInfo, int totaldamage, DamageTypes damagetype, Elements element)
        {
            Debug.Log($"{Name} received {totaldamage} damage!");
        }

        /// <summary>
        /// Gets all the Spells the entity knows. This returns a copy of the LearnedSpells dictionary
        /// </summary>
        public Dictionary<string, Spell> GetAllSpells()
        {
            return new Dictionary<string, Spell>(LearnedSpells);
        }

        /// <summary>
        /// Makes the entity learn a Spell
        /// </summary>
        /// <param name="spellName">The name of the Spell to learn</param>
        public void LearnSpell(string spellName)
        {
            if (KnowsSpell(spellName) == true)
            {
                Debug.LogError($"{Name} already knows {spellName}!");
                return;
            }

            Spell learnedSpell = Spell.GetSpell(spellName);

            //Don't add if the spell doesn't exist
            if (learnedSpell != null)
            {
                LearnedSpells.Add(spellName, learnedSpell);
                Debug.Log($"{Name} learned {spellName}!");
            }
            else
            {
                Debug.LogError($"Spell with name \"{spellName}\" does not exist in the global Spell table!");
            }
        }

        /// <summary>
        /// Tells if the entity knows a particular Spell
        /// </summary>
        /// <param name="spellName">The name of the Spell to check that the entity knows</param>
        /// <returns>true if the entity knows the Spell, otherwise false</returns>
        public bool KnowsSpell(string spellName)
        {
            return LearnedSpells.ContainsKey(spellName);
        }

        /// <summary>
        /// Makes the entity forget a Spell
        /// </summary>
        /// <param name="spellNames">The name of the Spell to forget</param>
        public void ForgetSpell(params string[] spellNames)
        {
            for (int i = 0; i < spellNames.Length; i++)
            {
                string spellName = spellNames[i];    

                if (KnowsSpell(spellName) == false)
                {
                    Debug.LogError($"{Name} doesn't know {spellName}!");
                    return;
                }

                LearnedSpells.Remove(spellName);
                Debug.Log($"{Name} forgot {spellName}!");
            }
        }

        /// <summary>
        /// Makes the entity forget all learned Spells
        /// </summary>
        public void ForgetAllSpells()
        {
            ForgetSpell(LearnedSpells.Keys.ToArray());
        }

        /// <summary>
        /// Inflicts one or more StatusEffects on the entity
        /// </summary>
        /// <param name="affectableInfo">Contains the Entity and what it inflicted the statuses with</param>
        /// <param name="statuses">The StatusEffects to inflict on the entity</param>
        public void InflictStatus(AffectableInfo affectableInfo, params StatusEffect[] statuses)
        {
            //Don't inflict statuses if dead
            if (IsDead == true)
            {
                Debug.LogWarning($"Entity: {Name} is dead and cannot be inflicted with any StatusEffects!");
                return;
            }

            for (int i = 0; i < statuses.Length; i++)
            {
                StatusEffect status = statuses[i];

                if (status == null)
                {
                    Debug.LogError($"Status being inflicted on {Name} at index {i} is null!");
                    continue;
                }

                //Copy the status for a new reference
                status = statuses[i].Copy();

                Type statusType = status.GetType();

                if (AfflictedStatuses.ContainsKey(statusType))
                {
                    AfflictedStatuses[statusType].Refresh();
                    Debug.Log($"Refreshed status {AfflictedStatuses[statusType].Name} on {Name}!");
                }
                else
                {
                    status.StatusFinishedEvent += OnStatusFinished;
                    AfflictedStatuses.Add(statusType, status);
                    AfflictedStatuses[statusType].SetAfflicter(affectableInfo.Affector);
                    AfflictedStatuses[statusType].SetReceiver(this);
                    AfflictedStatuses[statusType].OnInflict();
                    Debug.Log($"Inflicted status {AfflictedStatuses[statusType].Name} on {Name}!");
                }
            }
        }

        /// <summary>
        /// Removes one or more StatusEffects on an entity.
        /// This method is the public one for use in curing StatusEffects via Items, Spells, and etc.
        /// </summary>
        /// <param name="affectableInfo">Contains the Entity and what it cured the StatusEffects with</param>
        /// <param name="statuses">The names of the StatusEffects to cure</param>
        public void CureStatuses(AffectableInfo affectableInfo, params Type[] statuses)
        {
            string curedstatuses = "Status Effects";
            if (statuses != null && statuses.Length > 0)
            {
                curedstatuses = string.Empty;
                if (statuses.Length == 1) curedstatuses = statuses[0].Name;
                else
                {
                    for (int i = 0; i < statuses.Length; i++)
                        curedstatuses += (i == (statuses.Length - 1) ? "and " + statuses[i].Name : statuses[i].Name + ", ");
                }
            }

            Debug.Log($"{affectableInfo.Affector?.Name} cured {curedstatuses} on {Name} with {affectableInfo.AffectableObj?.Name}!");

            RemoveStatus(statuses);
        }

        /// <summary>
        /// Removes a set of StatusEffects affecting an entity.
        /// Each StatusEffect's OnEnd() method is called and the entity unsubscribes from the status finish events
        /// </summary>
        /// <param name="statuses">The names of the StatusEffects to remove</param>
        protected void RemoveStatus(params Type[] statuses)
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                Type statusType = statuses[i];
                if (AfflictedStatuses.ContainsKey(statusType))
                {
                    StatusEffect status = AfflictedStatuses[statusType];
                    status.End();

                    OnStatusFinished(status);
                }
            }
        }

        /// <summary>
        /// Removes all StatusEffects affecting an entity.
        /// Functions the same as RemoveStatus
        /// </summary>
        public void ClearAllStatuses()
        {
            RemoveStatus(AfflictedStatuses.Keys.ToArray());
        }

        public void AddStatModifier(StatModifiers.StatModTypes statModType, int amount, float percentage)
        {
            StatModifications.AddModifier(statModType, amount, percentage);
        }

        public bool RemoveStatModifierAmount(StatModifiers.StatModTypes statModType, int amount)
        {
            return StatModifications.RemoveModifierWithAmount(statModType, amount);
        }

        public bool RemoveStatModifierPercentage(StatModifiers.StatModTypes statModType, float percentage)
        {
            return StatModifications.RemoveModifierWithPercentage(statModType, percentage);
        }

        public void ClearStatModifiers()
        {
            StatModifications.ClearModifiers();
        }

        private void OnStatusFinished(StatusEffect status)
        {
            status.StatusFinishedEvent -= OnStatusFinished;

            Type statusType = status.GetType();

            if (AfflictedStatuses.ContainsKey(statusType))
            {
                AfflictedStatuses.Remove(statusType);
                Debug.Log($"{status.Name} ended on {Name}!");
            }
            else
            {
                Debug.LogError($"{Name} is not afflicted with the {status.Name} status!");
            }
        }

        public virtual void Dispose()
        {
            ClearAllStatuses();
            ClearStatModifiers();

            TurnStartEvent = null;
            TurnEndEvent = null;
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
                GameCore.spriteSorter.Add(EntitySprite, Globals.BASE_ENTITY_LAYER + (Position.Y / 1000f));
            }
        }
    }
}
