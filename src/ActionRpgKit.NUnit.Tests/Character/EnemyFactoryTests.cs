﻿using System;
using NUnit.Framework;
using ActionRpgKit.Character;
using ActionRpgKit.Character.Stats;
using ActionRpgKit.Item;
using ActionRpgKit.Core;

namespace ActionRpgKit.Tests.Character
{
    
    [TestFixture]
    [Category("Character")]
    class EnemyFactoryTests
    {

        [SetUp]
        public void SetUp()
        {
            EnemyPool.Initialize(size: 1);
            GameTime.Reset();

            // Items
            IItem item = new UsableItem();
            item.Id = 0;
            item.Name = "TestItem";

            // Initialize the EnemyFactory
            EnemyStats zombieStats = new EnemyStats();
            EnemyFactory.AddEnemyType("Zombie", zombieStats,
                                      new SimpleInventory(new IItem[] { item }, 
                                                          new int[] {1}));
        }
        
        [Test]
        public void EnemyFactoryTest()
        {
            var enemy = EnemyFactory.GetEnemyByType("Zombie");
            enemy.Stats.Life.Value = 20;
            Assert.AreEqual(20, enemy.Stats.Life.Value);
            EnemyPool.Release(enemy);
            enemy = EnemyFactory.GetEnemyByType("Zombie");
            Assert.AreEqual(0, enemy.Stats.Life.Value);
        }

        [Test]
        public void EnemyPoolTest()
        {
            // Retrieve an enemy and release it back to the pool
            var enemy = EnemyPool.Acquire();
            EnemyPool.Release(enemy);
            Assert.AreEqual(1, EnemyPool.Size);

            // Retrieve more enemies than the pool has to offer will 
            // increase the size.
            for(int i=0; i < 2; i++)
            {
                EnemyPool.Acquire();
            }
            Assert.AreEqual(2, EnemyPool.Size);
        }
    }
}
