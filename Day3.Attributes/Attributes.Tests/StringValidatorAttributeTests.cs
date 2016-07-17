using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Attributes.Tests
{
    [TestClass]
    public class StringValidatorAttributeTests
    {
        [TestMethod]
        public void IsValid_NullValue_ReturnFalse()
        {
            var attr = new StringValidatorAttribute(4);
            Assert.AreEqual(false, attr.IsValid(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_IntValue_ThrowAnException()
        {
            var attr = new StringValidatorAttribute(4);
            attr.IsValid(5);
        }

        [TestMethod]
        public void IsValid_StringValueLengthLessThanFourOrEqual_ReturnTrue()
        {
            var attr = new StringValidatorAttribute(4);
            Assert.AreEqual(true, attr.IsValid("test"));
        }

        [TestMethod]
        public void IsValid_StringValueLengthGreaterThanFour_ReturnFalse()
        {
            var attr = new StringValidatorAttribute(4);
            Assert.AreEqual(false, attr.IsValid("test2"));
        }

    }
}
