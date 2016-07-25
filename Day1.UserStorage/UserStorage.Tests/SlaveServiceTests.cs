using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Services;
using static UserStorage.Tests.AuxilaryInfo;

namespace UserStorage.Tests
{
    [TestClass]
    public class SlaveServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Slave_Ctor_NullConnectionInfo_ThrowAnException()
        {
            new SlaveService(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessViolationException))]
        public void Slave_Add_ThrowAnException()
        {
            var slave = new SlaveService(new ConnectionInfo("127.0.0.1", 131), new TestLoader());
            slave.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessViolationException))]
        public void Slave_Delete_ThrowAnException()
        {
            var slave = new SlaveService(new ConnectionInfo("127.0.0.1", 131), new TestLoader());
            slave.Delete(1);
        }

        [TestMethod]
        public void Slave_SearchForUser_OneCriteria_ReturnFirstUserId()
        {
            // act
            var slave = new SlaveService(new ConnectionInfo("127.0.0.1", 131), new TestLoader());
            var user = slave.Users.First();
            var foundUsers = slave.SearchForUser(new Func<User, bool>[] { u => u.LastName == user.LastName });

            // assert
            CollectionAssert.AreEqual(new List<int> { user.PersonalId }, foundUsers.ToList());
        }
        
    }
}
