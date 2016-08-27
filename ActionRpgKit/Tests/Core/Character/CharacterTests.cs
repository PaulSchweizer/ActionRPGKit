﻿using NUnit.Framework;
using Character;
using Character.Skill;

namespace CharacterTests
{
    [TestFixture]
    [Category("Character.Character")]
    public class CharacterTests
    {

        ICharacter player;

        PassiveSkill passiveSkill;

        [SetUp]
        public void SetUp ()
        {
            GameTime.time = 0;
            player = new Player("John");
            passiveSkill = new PassiveSkill("ShadowStrength",
                                            "A Description",
                                            cost: 20,
                                            preUseTime: 20,
                                            cooldownTime: 20);
        }

        [Test]
        public void UsingSkillTest ()
        {
            // Trigger a Skill
            player.LearnSkill(passiveSkill);
            bool triggered = player.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsFalse(triggered);

            // Advance in Time
            GameTime.time = 21;
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
        }
    }
}
