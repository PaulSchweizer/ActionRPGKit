﻿using System;
using NUnit.Framework;
using ActionRpgKit.Core;
using ActionRpgKit.Character.Stats;

namespace ActionRpgKit.Tests.Stats
{
    [TestFixture]
    [Category("Character.Stats")]
    public class StatsTests
    {
        PlayerStats playerStats;
        EnemyStats enemyStats;

        [SetUp]
        public void SetUp()
        {
            GameTime.Reset();
            playerStats = new PlayerStats();
            enemyStats = new EnemyStats();
        }

        [Test]
        public void RegenerationTest ()
        {
        }
    }
}