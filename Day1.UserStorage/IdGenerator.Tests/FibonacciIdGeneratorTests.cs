using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdGenerator.Tests
{
    [TestClass]
    public class FibonacciIdGeneratorTests
    {
        [TestMethod]
        public void GenerateId_FirstElement_ReturnOne()
        {
            //act
            var generator = new FibonacciIdGenerator();
            generator.GenerateNextId();

            //assert
            Assert.AreEqual(1, generator.CurrentId);
        }

        [TestMethod]
        public void GenerateId_SecondElement_ReturnTwo()
        {
            //act
            var generator = new FibonacciIdGenerator();
            generator.GenerateNextId();
            generator.GenerateNextId();

            //assert
            Assert.AreEqual(2, generator.CurrentId);
        }

        [TestMethod]
        public void GenerateId_FirstElement_ReturnThree()
        {
            //act
            var generator = new FibonacciIdGenerator();
            generator.GenerateNextId();
            generator.GenerateNextId();
            generator.GenerateNextId();

            //assert
            Assert.AreEqual(3, generator.CurrentId);
        }

        [TestMethod]
        public void GenerateId_InitialValueFiveNextElement_ReturnEight()
        {
            //act
            var generator = new FibonacciIdGenerator(5);
            generator.GenerateNextId();

            //assert
            Assert.AreEqual(8, generator.CurrentId);
        }
    }
}
