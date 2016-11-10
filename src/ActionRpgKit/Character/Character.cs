﻿using System;
using System.Collections.Generic;
using ActionRpgKit.Core;
using ActionRpgKit.Item;
using ActionRpgKit.Character.Skill;
using ActionRpgKit.Character.Stats;
using ActionRpgKit.Character.Attribute;
using System.Runtime.Serialization;

namespace ActionRpgKit.Character
{
    #region Interfaces

    /// <summary>
    /// Characters populate the game world. 
    /// They are defined by Stats.</summary>
    public interface ICharacter
    {

        /// <summary>
        /// The active state of this Character.</summary>
        IState CurrentState { get; set; }

        /// <summary>
        /// Change the State to the given state if the given state differs
        /// from the current state.</summary>
        void ChangeState(IState state);
        
        /// <summary>
        /// Event is fired when a character changes state.</summary>
        event StateChangedHandler OnStateChanged;
    }

    /// <summary>
    /// Handler operates whenever an Character's state changes.</summary>
    /// <param name="sender">The sending ICharacter</param>
    /// <param name="previousState">The previos state</param>
    /// <param name="newState">The new state</param>
    public delegate void StateChangedHandler(ICharacter sender, IState previousState, IState newState);

    /// <summary>
    /// Character can use Magic.</summary>  
    public interface IMagicUser
    {
        /// <summary>
        /// MagicSkills available for this Character.</summary>
        List<int> MagicSkills { get; set; }

        /// <summary>
        /// Add a new MagicSkill.</summary>
        void LearnMagicSkill (int skillId);

        /// <summary>
        /// Trigger the given MagicSkill.</summary>
        bool TriggerMagicSkill (int skillId);

        /// <summary>
        /// Use the MagicSkill.</summary>
        void UseMagicSkill(int skillId);

        /// <summary>
        /// Event is fired when a new MagicSkill is learned.</summary>
        event MagicSkillLearnedHandler OnMagicSkillLearned;
        
        /// <summary>
        /// Event is fired when an MagicSkill is triggered.</summary>
        event MagicSkillTriggeredHandler OnMagicSkillTriggered;
    }
    
    /// <summary>
    /// Handler operates whenever an IMagicUser learns a new MagicSkill.</summary>
    /// <param name="sender">The sending IMagicUser</param>
    /// <param name="skill">The learned MagicSkill</param>
    public delegate void MagicSkillLearnedHandler(IMagicUser sender, int skillId);
    
    /// <summary>
    /// Handler operates whenever an IMagicUser triggers an MagicSkill.</summary>
    /// <param name="sender">The sending IMagicUser</param>
    /// <param name="skill">The triggered MagicSkill</param>
    public delegate void MagicSkillTriggeredHandler(IMagicUser sender, int skillId);
    
    /// <summary>
    /// Character can fight.</summary>  
    public interface IFighter
    {
        /// <summary>
        /// World space position of the IFighter.</summary>
        Position Position { get; set; }

        /// <summary>
        /// The remaining life.</summary>
        float Life { get; set; }

        /// <summary>
        /// Whether the Fighter is active or defeated.</summary>
        bool IsDead{ get; set; }

        /// <summary>
        /// Targeted enemies of the fighter.</summary>
        List<IFighter> Enemies { get; }

        /// <summary>
        /// All enemies currently in Attack Range.</summary>
        List<IFighter> EnemiesInAttackRange { get; set; } 

        /// <summary>
        /// A timestamp representing the next possible moment for an attack.</summary>
        float TimeUntilNextAttack { get; set; }

        /// <summary>
        /// The current Skill that is to be used for an Attack.</summary>
        int CurrentAttackSkill { get; set; }

        /// <summary>
        /// The Attack Range.</summary>
        float AttackRange { get; }
        
        /// <summary>
        /// The currently equipped Weapon.</summary>
        int EquippedWeapon { get; set; }

        /// <summary>
        /// Add a new enemy.</summary>
        void AddEnemy(IFighter enemy, int index);

        /// <summary>
        /// Remove an enemy.</summary>
        void RemoveEnemy(IFighter enemy);

        /// <summary>
        /// Whether the Character can attack depends on the time since the
        /// last attack.</summary>
        bool CanAttack();

        /// <summary>
        /// Attack the given IFighter.</summary>
        void Attack(IFighter enemy);

        /// <summary>
        /// CombatSkills available for this Character.</summary>
        List<int> CombatSkills { get; set; }

        /// <summary>
        /// Add a new CombatSkill.</summary>
        void LearnCombatSkill(int skillId);

        /// <summary>
        /// Attack with the given CombatSkill.</summary>
        bool TriggerCombatSkill(int skillId);

        /// <summary>
        /// Use the CombatSkill.</summary>
        void UseCombatSkill(int skillId);

        /// <summary>
        /// The Fighter is being attacked.</summary>
        void OnAttacked(IFighter attacker, float damage);

        /// <summary>
        /// Event is fired when a new CombatSkill is learned.</summary>
        event CombatSkillLearnedHandler OnCombatSkillLearned;
        
        /// <summary>
        /// Event is fired when an ICombatSkill is triggered.</summary>
        event CombatSkillTriggeredHandler OnCombatSkillTriggered;

        /// <summary>
        /// Event is fired when an Enemy enters the alertness range.</summary>
        event EnemyEnteredAltertnessRangeHandler OnEnemyEnteredAltertnessRange;

        /// <summary>
        /// Event is fired when an Enemy leaves the alterness range.</summary>
        event EnemyLeftAltertnessRangeHandler OnEnemyLeftAltertnessRange;
    }

    /// <summary>
    /// Handler operates whenever a IFighter learns a new CombatSkill.</summary>
    /// <param name="sender">The sending IFighter</param>
    /// <param name="skill">The learned CombatSkill</param>
    public delegate void CombatSkillLearnedHandler(IFighter sender, int skillId);
    
    /// <summary>
    /// Handler operates whenever a Character's state changes.</summary>
    /// <param name="sender">The sending IFighter</param>
    /// <param name="skill">The triggered CombatSkill</param>
    public delegate void CombatSkillTriggeredHandler(IFighter sender, int skillId);

    /// <summary>
    /// An enemy has entered the alterness range.</summary>
    /// <param name="enemy">The Enemy</param>
    public delegate void EnemyEnteredAltertnessRangeHandler(IFighter enemy);

    /// <summary>
    /// An enemy has left the alterness range.</summary>
    /// <param name="enemy">The Enemy</param>
    public delegate void EnemyLeftAltertnessRangeHandler(IFighter enemy);

    #endregion

    #region Abstracts

    /// <summary>
    /// Base implementation of a Character.</summary>
    [Serializable]
    public abstract class BaseCharacter : IGameObject, ICharacter, IMagicUser, IFighter
    {
        /// <summary>
        /// Id of the Charcter.</summary>
        public int Id;

        /// <summary>
        /// Name of the character.</summary>
        public string Name;

        /// <summary>
        /// Stats describing the Character.</summary>
        public BaseStats Stats;

        /// <summary>
        /// Inventory of the Character.</summary>
        public BaseInventory Inventory;

        /// <summary>
        /// Event handler.</summary>
        [field: NonSerialized] public event StateChangedHandler OnStateChanged;
        [field: NonSerialized] public event MagicSkillLearnedHandler OnMagicSkillLearned;   
        [field: NonSerialized] public event MagicSkillTriggeredHandler OnMagicSkillTriggered; 
        [field: NonSerialized] public event CombatSkillLearnedHandler OnCombatSkillLearned;
        [field: NonSerialized] public event CombatSkillTriggeredHandler OnCombatSkillTriggered;
        [field: NonSerialized] public event EnemyEnteredAltertnessRangeHandler OnEnemyEnteredAltertnessRange;    
        [field: NonSerialized] public event EnemyLeftAltertnessRangeHandler OnEnemyLeftAltertnessRange;

        #region States

        /// <summary>
        /// The level of alertness of the Character.</summary>
        public float AlertnessLevel
        {
            get
            {
                return Stats.BaseAlertnessLevel.Value;
            }
        }

        /// <summary>
        /// The time in seconds until the Character detects the Enemy in his 
        /// AlternessRange.</summary>
        [NonSerialized]
        public float AlertnessEndTime;

        /// <summary>
        /// Whether the Character is currently moving.</summary>
        [NonSerialized]
        public bool IsMoving;

        /// <summary>
        /// The Character is idling.</summary>
        [NonSerialized]
        public IdleState IdleState = IdleState.Instance;

        /// <summary>
        /// The Character is moving from one point to another.</summary>
        [NonSerialized]
        public MoveState MoveState = MoveState.Instance;

        /// <summary>
        /// The Character is aware of enemies somewhere around him.</summary>
        [NonSerialized]
        public AlertState AlertState = AlertState.Instance;

        /// <summary>
        /// The Character is running after an enemy.</summary>
        [NonSerialized]
        public ChaseState ChaseState = ChaseState.Instance;

        /// <summary>
        /// The Character is close enough to attack an enemy.</summary>
        [NonSerialized]
        public AttackState AttackState = AttackState.Instance;

        /// <summary>
        /// The Character has been defeated and is about to be removed.</summary>
        [NonSerialized]
        public DyingState DyingState = DyingState.Instance;

        #endregion

        /// <summary>
        /// Determines whether the Charaxter is active or not.</summary>
        [NonSerialized]
        private bool _isDead = false;

        /// <summary>
        /// A list of absolute end times for the cooldown of magic skills.</summary>
        public List<float> MagicSkillEndTimes = new List<float>();

        /// <summary>
        /// A list of absolute end times for the cooldown of combat skills.</summary>
        public List<float> CombatSkillEndTimes = new List<float>() {};

        /// <summary>
        /// Initialize the Stats, Inventory, State and connect basic signals.</summary>
        /// <param name="stats">The Stats</param>
        /// <param name="inventory">The Inventory</param>
        public BaseCharacter(BaseStats stats, BaseInventory inventory)
        {
            Stats = stats;
            Inventory = inventory;
            CurrentState = IdleState;
            
            // Connect internal signals
            Stats.Life.OnMinReached += new MinReachedHandler(OnDeath);
        }

        /// <summary>
        /// Format the Character into a nice string reperesentation.</summary>
        /// <returns>Nice text representation</returns>
        public override string ToString()
        {
            string repr = string.Format("### CHARACTER: {0} ###\n" +
                                         "{1}\n", Name, Stats.ToString());
            repr += "--- Combat Skills ------------\n";
            for (int i = 0; i < CombatSkills.Count; i++)
            {
                repr += string.Format("{0}\n", CombatSkills[i].ToString());
            }
            repr += "--- Magic Skills ------------";
            for (int i = 0; i < MagicSkills.Count; i++)
            {
                repr += string.Format("\n{0}", MagicSkills[i].ToString());
            }
            repr += "\n";
            repr += Inventory.ToString();
            return repr;
        }

        /// <summary>
        /// Reset the combat skill end times values.</summary>
        /// <param name="context">The context</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            CombatSkillEndTimes.Clear();
            for (int i = 0; i < CombatSkills.Count; i++)
            {
                CombatSkillEndTimes.Add(-1);
            }
        }

        #region IGameObject Implementations

        /// <summary>
        /// 2D world space position.</summary>
        public Position Position { get; set; } = new Position();

        /// <summary>
        /// Update the current state of the Character.</summary>
        public void Update ()
        {
            CurrentState.UpdateState(this);
        }

        #endregion

        #region ICharacter Implementations

        /// <summary>
        /// The current state of the Character.</summary>
        public IState CurrentState { get; set; }

        /// <summary>
        /// Change the State to the given State if the given State differs
        /// from the current State.</summary>
        public void ChangeState(IState state)
        {
            if (state != CurrentState)
            {
                var previousState = CurrentState;
                CurrentState.ExitState(this);
                CurrentState = state;
                CurrentState.EnterState(this);
                EmitOnStateChanged(previousState, CurrentState);
            }
        }

        /// <summary>
        /// Emit the state changes signal if any handlers are attached.</summary>
        protected void EmitOnStateChanged(IState previousState, IState newState)
        {
            var handler = OnStateChanged;
            if (handler != null)
            {
                handler(this, previousState, newState);
            }
        }

        /// <summary>
        /// Emit the magic learned signal if any handlers are attached.</summary>
        protected void EmitOnMagicSkillLearned(int skillId)
        {
            var handler = OnMagicSkillLearned;
            if (handler != null)
            {
                handler(this, skillId);
            }
        }

        /// <summary>
        /// Emit the magic skill triggered signal if any handlers are attached.</summary>
        protected void EmitOnMagicSkillTriggered(int skillId)
        {
            var handler = OnMagicSkillTriggered;
            if (handler != null)
            {
                handler(this, skillId);
            }
        }

        /// <summary>
        /// Emit the combat learned signal if any handlers are attached.</summary>
        protected void EmitOnCombatSkillLearned(int skillId)
        {
            var handler = OnCombatSkillLearned;
            if (handler != null)
            {
                handler(this, skillId);
            }
        }

        /// <summary>
        /// Emit the combat skill triggered signal if any handlers are attached.</summary>
        protected void EmitOnCombatSkillTriggered(int skillId)
        {
            var handler = OnCombatSkillTriggered;
            if (handler != null)
            {
                handler(this, skillId);
            }
        }

        /// <summary>
        /// Emit the enemy entered alertness range signal if any handlers are attached.</summary>
        protected void EmitOnEnemyEnteredAltertnessRange()
        {
            var handler = OnEnemyEnteredAltertnessRange;
            if (handler != null)
            {
                handler(this);
            }
        }

        /// <summary>
        /// Emit the enemy left alertness range signal if any handlers are attached.</summary>
        protected void EmitOnEnemyLeftAltertnessRange()
        {
            var handler = OnEnemyLeftAltertnessRange;
            if (handler != null)
            {
                handler(this);
            }
        }

        #endregion

        #region IMagicUser Implementations

        /// <summary>
        /// The magic value.</summary>
        public float Magic
        {
            get
            {
                return Stats.Magic.Value;
            }
            set
            {
                Stats.Magic.Value = value;
            }
        }

        /// <summary>
        /// All available magic skills.</summary>
        public List<int> MagicSkills { get; set; } = new List<int>();

        /// <summary>
        /// Add a new magic skill.</summary>
        /// <param name="skillId">The id of the magic skill to learn.</param>
        public void LearnMagicSkill(int skillId)
        {
            if (!MagicSkills.Contains(skillId))
            {
                MagicSkills.Add(skillId);
                MagicSkillEndTimes.Add(-1);
                EmitOnMagicSkillLearned(skillId);
            }
        }

        /// <summary>
        /// Trigger the given magic skill.</summary>
        /// <param name="skillId">The id of the magic skill.</param>
        /// <returns>Whether the magic skill was triggered.</returns>
        public bool TriggerMagicSkill(int skillId)
        {
            if (!MagicSkillCanBeUsed(skillId))
            {
                return false;
            }
            var magicSkill = SkillDatabase.GetMagicSkillById(skillId);
            Magic -= magicSkill.Cost;
            MagicSkillEndTimes[MagicSkills.IndexOf(skillId)] = GameTime.time + magicSkill.CooldownTime;
            EmitOnMagicSkillTriggered(skillId);
            return true;
        }

        /// <summary>
        /// Triggers the use of the Skill</summary>
        /// <param name=skillId>The Skill to use</param>
        public void UseMagicSkill(int skillId)
        {
            var magicSkill = SkillDatabase.GetMagicSkillById(skillId);
            magicSkill.Use(this);
        }

        /// <summary>
        /// Check if the character knows the magic skill, has enough
        /// magic energy and is not in cooldown of the Skill.</summary>
        /// <param name=magicSkill>The Skill to test</param>
        /// <returns> Whether the Skill van be used.</returns>
        private bool MagicSkillCanBeUsed(int skillId)
        {
            if (!MagicSkills.Contains(skillId))
            {
                return false;
            }
            var magicSkill = SkillDatabase.GetMagicSkillById(skillId);
            if (Magic < magicSkill.Cost)
            {
                return false;
            }
            return GameTime.time >= MagicSkillEndTimes[MagicSkills.IndexOf(skillId)];
        }

        #endregion 

        #region IFighter Implementations

        /// <summary>
        /// The life value.</summary>
        public float Life
        {
            get
            {
                return Stats.Life.Value;
            }
            set
            {
                Stats.Life.Value = value;
            }
        }

        /// <summary>
        /// Whether the Character is active or not.</summary>
        public bool IsDead { get { return _isDead; } set { _isDead = value; } }

        /// <summary>
        /// A list of current Enemies in reach of this Character.</summary>
        public List<IFighter> Enemies { get; set; } = new List<IFighter>();

        /// <summary>
        /// The relative time until the next attack is possible.</summary>
        public float TimeUntilNextAttack { get; set; }

        /// <summary>
        /// The attack skill used by this Character.</summary>
        public int CurrentAttackSkill { get; set; } = -1;

        /// <summary>
        /// The Attack Range takes the Weapon into account too.</summary>
        public float AttackRange {
            get
            {
                if (EquippedWeapon > -1)
                {
                    return Stats.AttackRange.Value + ItemDatabase.GetWeaponItemById(EquippedWeapon).Range;
                }
                else
                {
                    return Stats.AttackRange.Value;
                }
                
            }
        }

        /// <summary>
        /// Currently equipped Weapon, if any. Defaults to -1.</summary>
        public int EquippedWeapon { get; set; } = -1;

        /// <summary>
        /// A list of enemies in AttackRange.</summary>
        public List<IFighter> EnemiesInAttackRange { get; set; } = new List<IFighter>();

        /// <summary>
        /// Add a new enemy to the active enemies.</summary>
        /// <param name="enemy">The Enemy</param>
        /// <param name="index">The targeted index in the list.</param>
        public void AddEnemy(IFighter enemy, int index=0)
        {
            if (!Enemies.Contains(enemy))
            {
                Enemies.Insert(index, enemy);
                EmitOnEnemyEnteredAltertnessRange();
            }
        }

        /// <summary>
        /// Remove the Enemy from the active enemies of this Character.</summary>
        /// <param name="enemy"></param>
        public void RemoveEnemy(IFighter enemy)
        {
            if (Enemies.Contains(enemy))
            {
                Enemies.Remove(enemy);
                EmitOnEnemyLeftAltertnessRange();
            }
        }

        /// <summary>
        /// It is based on the time until the next attack may be triggered.</summary>
        /// <returns>Whether the Character is allowed to attack.</returns>
        public bool CanAttack()
        {
            return GameTime.time > TimeUntilNextAttack;
        }

        /// <summary>
        /// Trigger the current attack skill.</summary>
        /// <param name="enemy"></param>
        public void Attack(IFighter enemy)
        {
            TriggerCombatSkill(CurrentAttackSkill);
        }

        /// <summary>
        /// All available combat skills for this Character.</summary>
        public List<int> CombatSkills { get; set; } = new List<int>();

        /// <summary>
        /// Add a new skill to the available skills.</summary>
        /// <param name="skillId">The combat skill id.</param>
        public void LearnCombatSkill(int skillId)
        {
            if (!CombatSkills.Contains(skillId))
            {
                CombatSkills.Add(skillId);
                CombatSkillEndTimes.Add(-1);
                EmitOnCombatSkillLearned(skillId);
            }
        }

        /// <summary>
        /// The combat skill signal is emitted. 
        /// The actual use of the Skill has to be implmented and connected 
        /// via a delegate.</summary>
        /// <param name="skillId">The CombatSkill to trigger.</param>
        /// <returns>Whether the skill was triggered.</returns>
        public bool TriggerCombatSkill(int skillId)
        {
            if (!CombatSkillCanBeUsed(skillId))
            {
                return false;
            }
            var combatSkill = SkillDatabase.GetCombatSkillById(skillId);
            float endTime = GameTime.time + combatSkill.CooldownTime;
            if (EquippedWeapon > -1)
            {
                endTime += 1 / ItemDatabase.GetWeaponItemById(EquippedWeapon).Speed;
            }
            CombatSkillEndTimes[CombatSkills.IndexOf(skillId)] = endTime; 
            EmitOnCombatSkillTriggered(skillId);
            return true;
        }

        /// <summary>
        /// Subtract the damage from the current life</summary>
        public void OnAttacked(IFighter attacker, float damage)
        {
            Stats.Life.Value -= damage;
            AddEnemy(attacker);
        }

        /// <summary>
        /// Triggers the use of the Skill</summary>
        /// <param name=skillId>The Skill to use</param>
        public void UseCombatSkill(int skillId)
        {
            var combatSkill = SkillDatabase.GetCombatSkillById(skillId);
            combatSkill.Use(this);
        }

        /// <summary>
        /// Check if the character knows the combat skill, and is not
        /// in cooldown of the Skill.</summary>
        /// <param name=combatSkill>The Skill to test</param>
        /// <returns> Whether the Skill van be used.</returns>
        private bool CombatSkillCanBeUsed(int skillId)
        {
            if (!CombatSkills.Contains(skillId))
            {
                return false;
            }
            if (EnemiesInAttackRange.Count == 0)
            {
                return false;
            }
            return GameTime.time >= CombatSkillEndTimes[CombatSkills.IndexOf(skillId)];
        }
        
        /// <summary>
        /// The Character has just been killed.</summary>
        private void OnDeath(BaseAttribute sender)
        {
            ChangeState(DyingState);
            IsDead = true;
        }

        #endregion
    }

    #endregion

    #region Implementations

    /// <summary>
    /// Representation of a player controllable character.</summary>
    [Serializable]
    public class Player : BaseCharacter
    {
        /// <summary>
        /// Constructor is needed for seamless serialization.</summary>
        public Player() : base(new PlayerStats(), new PlayerInventory())
        {
            Controller.Register(this);
        }

        /// <summary>
        /// Register the Player Character at the Controller.</summary>
        public Player(string name) : base(new PlayerStats(), new PlayerInventory())
        {
            Name = name;
            Controller.Register(this);
        }
    }

    /// <summary>
    /// Representation of a hostile, game controlled character.</summary>
    [Serializable]
    public class Enemy : BaseCharacter
    {
        /// <summary>
        /// Constructor is needed for seamless serialization.</summary>
        public Enemy() : base(new EnemyStats(), new SimpleInventory())
        {
            Controller.Register(this);
        }

        /// <summary>
        /// Register the Enemy Character at the Controller.</summary>
        public Enemy(string name) : base(new EnemyStats(), new SimpleInventory())
        {
            Name = name;
            Controller.Register(this);
        }
    }

    #endregion
}
