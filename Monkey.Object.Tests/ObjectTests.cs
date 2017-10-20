using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Monkey.Object.Tests
{
    [TestClass]
    public class ObjectTests
    {
        [TestMethod]
        public void TestStringHashKey()
        {
            var hello1 = new String
            {
                Value = "Hello World"
            };
            var hello2 = new String
            {
                Value = "Hello World"
            };
            var diff1 = new String
            {
                Value = "My name is johnny"
            };
            var diff2 = new String
            {
                Value = "My name is johnny"
            };

            Assert.AreEqual(hello2.HashKey(), hello1.HashKey(), "strings with same content have different hash keys");
            Assert.AreEqual(diff2.HashKey(), diff1.HashKey(), "strings with same content have different hash keys");
            Assert.AreNotEqual(diff1.HashKey(), hello1.HashKey(), "strings with different content have same hash keys");
        }
    }
}