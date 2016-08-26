
using System;
using NUnit.Framework;
namespace CharacterTests
{
    [TestFixture]
    [Category("Character.Attribute")]
    public class CharacterAttributeTests
    {
        Character.PrimaryAttribute Body;
        
        [SetUp]
        public void SetUp ()
        {
            Body = new Character.PrimaryAttribute("Body", 0, 999, 10);
            GameTime.time = 0f;
        }
        
        [Test]
        public void PrimaryAttributeTest()
        {
            // Check the default values
            Assert.AreEqual(999, Body.MaxValue);
            Assert.AreEqual(0, Body.MinValue);
            Assert.AreEqual(10, Body.Value);

            // Set some values
            Body.Value = 12;
            Assert.AreEqual(Body.Value, 12);     
            Body.Value = 10000;
            Assert.AreEqual(Body.Value, 999);
            Body.Value = -10000;
            Assert.AreEqual(Body.Value, 0);
            
            // Add a basic modifier
            Body.AddModifier(new Character.Modifier("StrengthBuff", 10, 10));
            Assert.AreEqual(20, Body.Value);
            
        }
    }
}
