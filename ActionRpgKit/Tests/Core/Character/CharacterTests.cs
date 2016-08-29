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
        ICharacter enemy;

        PassiveSkill passiveSkill;

        [SetUp]
        public void SetUp ()
        {
            GameTime.time = 0;
            player = new PlayerCharacter("John");
            enemy = new EnemyCharacter("Zombie");
            passiveSkill = new PassiveSkill("ShadowStrength",
                                            "A Description",
                                            cost: 10,
                                            preUseTime: 10,
                                            cooldownTime: 10);
        }

        [Test]
        public void UsingSkillTest ()
        {
            // Player triggers a Skill that is not learned yet
            bool triggered = player.TriggerSkill(passiveSkill);
            Assert.IsFalse(triggered);
            // Player learns the Skill and triggers it
            player.LearnSkill(passiveSkill);
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
            // Player triggers it again right away, which is not possible due to cooldown time
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsFalse(triggered);
            
            // Enemy uses the passive skill
            enemy.LearnSkill(passiveSkill);
            Assert.IsTrue(triggered);

            // Advance in Time and trigger again
            GameTime.time = 11;
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
            triggered = enemy.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
            
            // Take the use costs into account
            GameTime.time = 21;
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsTrue(triggered);
            GameTime.time = 31;
            // Can't be triggered because of lack of energy
            triggered = player.TriggerSkill(passiveSkill);
            Assert.IsFalse(triggered);
        }
    }
}
