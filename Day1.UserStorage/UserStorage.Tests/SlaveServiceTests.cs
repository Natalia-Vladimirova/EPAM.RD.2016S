using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage.Interfaces.Entities;
using UserStorage.Services;
using static UserStorage.Tests.AuxilaryInfo;

namespace UserStorage.Tests
{
    [TestClass]
    public class SlaveServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Slave_Ctor_NullCreator_ThrowAnException()
        {
            new SlaveService(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(AccessViolationException))]
        public void Slave_Add_ThrowAnException()
        {
            var slave = new SlaveService(Creator);
            slave.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessViolationException))]
        public void Slave_Delete_ThrowAnException()
        {
            var slave = new SlaveService(Creator);
            slave.Delete(1);
        }

        [TestMethod]
        public void Slave_SearchForUser_OneCriteria_ReturnFirstUserId()
        {
            // act
            var slave = new SlaveService(Creator);
            var user = slave.GetAll().First();
            var foundUsers = slave.Search(new Func<User, bool>[] { u => u.LastName == user.LastName });
            slave.StopListen();

            // assert
            CollectionAssert.AreEqual(new List<int> { user.PersonalId }, foundUsers.ToList());
        }
    }
}
