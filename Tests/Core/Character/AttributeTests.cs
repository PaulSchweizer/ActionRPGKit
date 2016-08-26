
using System;
using NUnit.Framework;
using Character.Attribute;

namespace CharacterTests
{
    [TestFixture]
    [Category("Character.Attribute")]
    public class AttributeTests
    {
        PrimaryAttribute Body;
        
        [SetUp]
        public void SetUp ()
        {
            Body = new PrimaryAttribute("Body", 0, 999, 10);
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
            
            // Add a modifier
            Body.AddModifier(new Modifier("StrengthBuff", 10, 10));
            Assert.AreEqual(10, Body.Value);
            
            // Advance in time
            for(int i=0; i<10; i++)
            {
                GameTime.time = i;
                Assert.AreEqual(10, Body.Value);
            }
            // Until the modifier's life cycle is over
            GameTime.time += 1;
            Assert.AreEqual(0, Body.Value);
            
            // Add some more modifiers and advance time
            GameTime.time = 0;
            Body.AddModifier(new Modifier("StrengthBuff", 10, 10));
            Body.AddModifier(new Modifier("AnotherStrengthBuff", 20, 5));
            Assert.AreEqual(30, Body.Value);

            GameTime.time = 5;
            Assert.AreEqual(10, Body.Value);
            Assert.AreEqual(1, Body.Modifiers.Count);
            
            GameTime.time = 10;
            Assert.AreEqual(0, Body.Value);
            Assert.AreEqual(0, Body.Modifiers.Count);  
        }
    }
}
