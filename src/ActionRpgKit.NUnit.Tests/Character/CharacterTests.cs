﻿using System;
using NUnit.Framework;
using ActionRpgKit.Core;
using ActionRpgKit.Character;
using ActionRpgKit.Character.Skill;
using ActionRpgKit.Item;

namespace ActionRpgKit.Tests.Character
{
    
    [TestFixture]
    [Category("Character.Character")]
    class CharacterTests
    {
        Player player;
        Enemy enemy;
        ICombatSkill meleeSkill;
        IMagicSkill passiveMagicSkill;
        int _stateChanged;
        int _magicSkillLearned;
        int _magicSkillTriggered;
        int _combatSkillLearned;
        int _combatSkillTriggered;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            player = new Player();
            enemy = new Enemy();
            meleeSkill = new GenericCombatSkill(id: 1,
                            name: "SwordFighting",
                            description: "Wield a sword effectively.",
                            preUseTime: 1,
                            cooldownTime: 1,
                            damage: 1,
                            maximumTargets: 1,
                            range: 1,
                            itemSequence: new IItem[] {});
            passiveMagicSkill = new PassiveMagicSkill(id: 0,
                                        name: "ShadowStrength",
                                        description: "A +10 Buff to the user's strength.",
                                        preUseTime: 10,
                                        cooldownTime: 5,
                                        itemSequence: new IItem[] { },
                                        cost: 10,
                                        duration: 10,
                                        modifierValue: 10,
                                        modifiedAttributeName: "Body");
            player.LearnCombatSkill(meleeSkill);
            player.LearnMagicSkill(passiveMagicSkill);
            enemy.Stats.Life.Value = 10;
            GameTime.Reset();
        }

        [Test]
        public void BasicsTest()
        {
            // Call ToString() to make sure it does not throw an error
            player.ToString();
            enemy.ToString();
        }

        [Test]
        public void LifeCallbackTest()
        {
            // Set the life of the enemy to 0 and expect the enemy to die
            enemy.Stats.Life.Value = -10;
            Assert.IsTrue(enemy.CurrentState is DyingState);
            Assert.IsTrue(enemy.IsDead);
        }

        [Test]
        public void StatesTest()
        {
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | P + + + E
            player.Stats.AlertnessRange.Value = 4 * 4;
            player.Position.Set(0, 0);
            enemy.Position.Set(4, 0);
            player.CurrentAttackSkill = player.CombatSkills[0];

            // Initial State
            Assert.IsTrue(player.CurrentState is IdleState);

            // Add Enemy switches to alert state
            player.AddEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);

            // No more Enemy switches to idle state
            player.RemoveEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is IdleState);

            // Add Enemy and go into Chase state
            player.AddEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);
            
            // Remove the enemy again and drop out of the chase again
            player.RemoveEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);
            
            // Enemy is back and the chase continues 
            player.AddEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);
            
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + + P + E
            player.Position.Set(2, 0);
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);

            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + + + P E
            player.Position.Set(3, 0);
            player.Update();
            Assert.IsTrue(player.CurrentState is AttackState);

            // Attack and get rid of the enemy
            for (int i = 1; i < 11; i ++)
            {
                GameTime.time += 1;
                player.Update();
                Assert.AreEqual(10 - i, enemy.Stats.Life.Value);
                enemy.Update();
                Assert.AreEqual(1, enemy.Enemies.Count);
            }

            // All of the enemies are gone, so the Character switches back to 
            // AlertState and then to IdleState.
            Assert.AreEqual(0, player.Enemies.Count);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);
            player.Update();
            Assert.IsTrue(player.CurrentState is IdleState);

            // Reset the enemy and simulate a fleeing enemy after it has been attacked
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + + E P +
            GameTime.time += 1;
            enemy.IsDead = false;
            enemy.Life = 10;
            enemy.Position.Set(2, 0);
            player.AddEnemy(enemy);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);
            player.Update();
            Assert.IsTrue(player.CurrentState is AttackState);
            player.Update();

            // Enemy flees
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + E + P +
            enemy.Position.Set(1, 0);
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);

            // Player chases after the enemy
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + E P + +
            player.Position.Set(2, 0);
            player.Update();
            Assert.IsTrue(player.CurrentState is AttackState);

            // Enemy is out of AlertnessRange so we eventually return to Idle
            //     0 1 2 3 4 5
            //   + - - - - - - 
            // 0 | E + + + P
            player.Position.Set(4, 0);
            enemy.Position.Set(0, 0);
            player.Stats.AlertnessRange.Value = 1;
            player.Update();
            Assert.IsTrue(player.CurrentState is ChaseState);
            player.Update();
            Assert.IsTrue(player.CurrentState is AlertState);
            player.Update();
            Assert.IsTrue(player.CurrentState is IdleState);
        }

        [Test]
        public void BaseCharacterEventsTest()
        {
            player.OnStateChanged += new StateChangedHandler(StateChangedTest);
            enemy.OnMagicSkillLearned += new MagicSkillLearnedHandler(MagicSkillLearnedTest);
            player.OnMagicSkillTriggered += new MagicSkillTriggeredHandler(MagicSkillTriggeredTest);
            enemy.OnCombatSkillLearned += new CombatSkillLearnedHandler(CombatSkillLearnedTest);
            player.OnCombatSkillTriggered += new CombatSkillTriggeredHandler(CombatSkillTriggeredTest);

            // Change the State
            Assert.AreEqual(0, _stateChanged);
            player.ChangeState(player._alertState);
            Assert.AreEqual(1, _stateChanged);
            player.ChangeState(player._idleState);
            Assert.AreEqual(2, _stateChanged);

            // Learn a new Magic Skill
            Assert.AreEqual(0, _magicSkillLearned);
            enemy.LearnMagicSkill(passiveMagicSkill);
            Assert.AreEqual(1, _magicSkillLearned);

            // Trigger a Magic Skill
            Assert.AreEqual(0, _magicSkillTriggered);
            player.TriggerMagicSkill(passiveMagicSkill);
            Assert.AreEqual(1, _magicSkillTriggered);
            player.TriggerMagicSkill(passiveMagicSkill);
            Assert.AreEqual(1, _magicSkillTriggered);

            // Learn a new Combat Skill
            Assert.AreEqual(0, _combatSkillLearned);
            enemy.LearnCombatSkill(meleeSkill);
            Assert.AreEqual(1, _combatSkillLearned);

            // Trigger a Combat Skill
            
            // No enemy in reach
            player.TriggerCombatSkill(meleeSkill);
            Assert.AreEqual(0, _combatSkillTriggered);
            
            // Set the enemy in reach
            player.Position.Set(0, 0);
            enemy.Position.Set(0, 0);
            player.AddEnemy(enemy);
            GameTime.time += 1;
            player.TriggerCombatSkill(meleeSkill);
            Assert.AreEqual(1, _combatSkillTriggered);
            
            // Not enough time has passed to trigger again
            player.TriggerCombatSkill(meleeSkill);
            Assert.AreEqual(1, _combatSkillTriggered);
        }

        public void StateChangedTest(ICharacter sender, IState previousState, IState newState)
        {
            _stateChanged += 1;
        }

        public void MagicSkillLearnedTest(IMagicUser sender, IMagicSkill skill)
        {
            _magicSkillLearned += 1;
        }

        public void MagicSkillTriggeredTest(IMagicUser sender, IMagicSkill skill)
        {
            _magicSkillTriggered += 1;
        }

        public void CombatSkillLearnedTest(IFighter sender, ICombatSkill skill)
        {
            _combatSkillLearned += 1;
        }

        public void CombatSkillTriggeredTest(IFighter sender, ICombatSkill skill)
        {
            _combatSkillTriggered += 1;
        }
    }
}
