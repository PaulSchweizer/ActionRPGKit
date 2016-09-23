﻿using System;
using NUnit.Framework;
using ActionRpgKit.Core;
using ActionRpgKit.Character;
using ActionRpgKit.Character.Skill;

namespace ActionRpgKit.Tests.Character
{
    
    [SetUpFixture]
    public class MySetUpClass
    {
        [SetUp]
    	public void RunBeforeAnyTests()
    	{
            CharacterTests.player = new Player();
            CharacterTests.enemy = new Enemy();
            CharacterTests.meleeSkill = new MeleeSkill(id: 1,
                            name: "SwordFighting",
                            description: "Wield a sword effectively.",
                            preUseTime: 1,
                            cooldownTime: 1,
                            damage: 1,
                            maximumTargets: 1);
            CharacterTests.player.LearnCombatSkill(CharacterTests.meleeSkill);
            CharacterTests.enemy.Stats.Life.Value = 10;
            GameTime.Reset();
    	}
    }
    
    [TestFixture]
    [Category("Character.Character")]
    class CharacterTests
    {
        public static Player player;
        public static Enemy enemy;
        public static ICombatSkill meleeSkill;

        [Test]
        public void LifeCallbackTest()
        {
            // Set the life of the enemy to 0 and expect the enemy to die
            enemy.Stats.Life.Value = -10;
            Assert.AreEqual(enemy.CurrentState is DyingState);
        }

        [Test]
        public void StatesTest()
        {
            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | P + + + E
            player.Stats.AlertnessRange.Value = 4;
            player.Position.Set(0, 0, 0);
            enemy.Position.Set(4, 0, 0);
            player.CurrentAttackSkill = player.CombatSkills[0];

            // Initial State
            Assert.IsTrue(player.CurrentState is IdleState);

            // Add Enemy switches to alert state
            player.AddEnemy(enemy);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is AlertState);

            // No more Enemy switches to idle state
            player.RemoveEnemy(enemy);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is IdleState);

            // Add Enemy and go into Chase state
            player.AddEnemy(enemy);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is AlertState);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is ChaseState);

            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + + P + E
            player.Position.Set(2, 0, 0);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is ChaseState);

            //     0 1 2 3 4
            //   + - - - - - 
            // 0 | + + + P E
            player.Position.Set(3, 0, 0);
            player.CurrentState.UpdateState(player);
            Assert.IsTrue(player.CurrentState is AttackState);

            // Attack and get rid of the enemy
            for (int i = 1; i < 11; i ++)
            {
                GameTime.time += 1;
                player.CurrentState.UpdateState(player);
                Assert.IsTrue(player.CurrentState is AttackState);
                Assert.AreEqual(10 - i, enemy.Stats.Life.Value);
            }

            // All of the enemies are gone, so the Character switches back to alert state 
        }
    }
}
