﻿using NUnit.Framework;
using ActionRpgKit.Core;
using ActionRpgKit.Character.Skill;
using ActionRpgKit.Item;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace ActionRpgKit.Tests.Skill
{
    [TestFixture]
    [Category("Character.Skill")]
    public class SkillTests
    {

        UsableItem herb;
        UsableItem coin;
        MagicSkill passiveMagicSkill;
        CombatSkill meleeSkill;
        CombatSkill meleeMultiTargetsSkill;
        CombatSkill rangedSkill;

        [SetUp]
        public void SetUp ()
        {
            GameTime.Reset();

            herb = new UsableItem();
            herb.Id = 0;
            herb.Name = "Herb";
            herb.Description = "A common herb";
            coin = new UsableItem();
            coin.Id = 1;
            coin.Name = "Coin";
            coin.Description = "A gold coin";

            passiveMagicSkill = new PassiveMagicSkill(id: 0,
                                                      name: "ShadowStrength",
                                                      description: "A +10 Buff to the user's strength.",
                                                      preUseTime: 10,
                                                      cooldownTime: 5,
                                                      itemSequence: new int[] { herb.Id },
                                                      cost: 10,
                                                      duration: 10,
                                                      modifierValue: 10,
                                                      modifiedAttributeName: "Body");
            meleeSkill = new GenericCombatSkill(id: 1,
                                                name: "SwordFighting",
                                                description: "Wield a sword effectively.",
                                                preUseTime: 1,
                                                cooldownTime: 1,
                                                damage: 1,
                                                maximumTargets: 1,
                                                range: 1,
                                                itemSequence: new int[] { });
            meleeMultiTargetsSkill = new GenericCombatSkill(id: 2,
                                                            name: "MultiHit",
                                                            description: "Wield a sword against multiple opponents.",
                                                            preUseTime: 1,
                                                            cooldownTime: 1,
                                                            damage: 1,
                                                            maximumTargets: 2,
                                                            range: 1,
                                                            itemSequence: new int[] { });
            rangedSkill = new GenericCombatSkill(id: 3,
                                                 name: "Longbow",
                                                 description: "Notch! Aim! Loose!",
                                                 preUseTime: 2,
                                                 cooldownTime: 1,
                                                 damage: 1,
                                                 maximumTargets: 1,
                                                 range: 10,
                                                 itemSequence: new int[] { });
        }

        [Test]
        public void SkillDatabaseTest()
        {
            SkillDatabase.MagicSkills = new MagicSkill[] { passiveMagicSkill };
            SkillDatabase.CombatSkills = new CombatSkill[] { meleeSkill, meleeMultiTargetsSkill, rangedSkill };

            Assert.IsNull(SkillDatabase.GetMagicSkillByName("DoesNotExist"));
            Assert.IsNull(SkillDatabase.GetCombatSkillByName("DoesNotExist"));
            Assert.AreSame(passiveMagicSkill, SkillDatabase.GetMagicSkillByName("ShadowStrength"));
            Assert.AreSame(meleeSkill, SkillDatabase.GetCombatSkillByName("SwordFighting"));
        }

        [Test]
        public void SerializeSkillsTest()
        {
            BinarySerialize(passiveMagicSkill);
            var serializedPassiveMagicSkill = (PassiveMagicSkill)BinaryDeserialize(passiveMagicSkill);
            DeserializedSkillTest(serializedPassiveMagicSkill, passiveMagicSkill);
        }

        private void DeserializedSkillTest(BaseSkill deserializedSkill, BaseSkill originalSkill)
        {
            Assert.AreEqual(deserializedSkill.Id, originalSkill.Id);
            Assert.AreEqual(deserializedSkill.Name, originalSkill.Name);
            Assert.AreEqual(deserializedSkill.Description, originalSkill.Description);
            Assert.AreEqual(deserializedSkill.CooldownTime, originalSkill.CooldownTime);
            Assert.AreEqual(deserializedSkill.ItemSequence, originalSkill.ItemSequence);
            Assert.AreEqual(deserializedSkill.CooldownTime, originalSkill.CooldownTime);
        }

        private void BinarySerialize(BaseSkill stats)
        {
            var serializedFile = Path.GetTempPath() + string.Format("/__SkillsTest__.bin");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(serializedFile,
                                           FileMode.Create,
                                           FileAccess.Write,
                                           FileShare.None);
            formatter.Serialize(stream, stats);
            stream.Close();
        }

        private BaseSkill BinaryDeserialize(BaseSkill stats)
        {
            var serializedFile = Path.GetTempPath() + string.Format("/__SkillsTest__.bin");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(serializedFile,
                                    FileMode.Open,
                                    FileAccess.Read,
                                    FileShare.Read);
            BaseSkill serializedStats = (BaseSkill)formatter.Deserialize(stream);
            stream.Close();
            File.Delete(serializedFile);
            return serializedStats;
        }
    }
}
