using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Attributes.Tests
{
    [TestClass]
    public class IntValidatorAttributeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_NullValue_ThrowAnException()
        {
            var attr = new IntValidatorAttribute(1, 3);
            attr.IsValid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_StringValue_ThrowAnException()
        {
            var attr = new IntValidatorAttribute(1, 3);
            string value = "test";
            attr.IsValid(value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_LongValue_ThrowAnException()
        {
            var attr = new IntValidatorAttribute(1, 3);
            long value = 11111111111;
            attr.IsValid(value);
        }

        [TestMethod]
        public void IsValid_IntValueInRange_ReturnTrue()
        {
            var attr = new IntValidatorAttribute(1, 3);
            int value = 3;
            Assert.AreEqual(true, attr.IsValid(value));
        }

        [TestMethod]
        public void IsValid_IntValueOutOfRange_ReturnFalse()
        {
            var attr = new IntValidatorAttribute(1, 3);
            int value = 5;
            Assert.AreEqual(false, attr.IsValid(value));
        }

    }
}
