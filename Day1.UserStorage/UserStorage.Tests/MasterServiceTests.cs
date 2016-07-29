using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Services;
using static UserStorage.Tests.AuxilaryInfo;

namespace UserStorage.Tests
{
    [TestClass]
    public class MasterServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Master_Ctor_NullIdGenerator_ThrowAnException()
        {
            new MasterService(null, null, null, null, LogService.Instance);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Master_Ctor_NullLoader_ThrowAnException()
        {
            new MasterService(new FibonacciIdGenerator(), null, null, null, LogService.Instance);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Master_Ctor_NullConnectionInfo_ThrowAnException()
        {
            new MasterService(new FibonacciIdGenerator(), new TestLoader(), null, null, LogService.Instance);
        }

        [TestMethod]
        public void Master_Add_ValidUser_UserAddedToMaster()
        {
            // arrange
            string testFirstName = "Test";
            string testLastName = "LTest";
            DateTime testDateTime = DateTime.Now;
            var generator = new FibonacciIdGenerator();
            generator.SetInitialValue(TestState.LastId);
            generator.GenerateNextId();

            // act
            var master = new MasterService(new FibonacciIdGenerator(), new TestLoader(), null, new ConnectionInfo[] { }, LogService.Instance);
            master.Load();
            master.Add(new User { FirstName = testFirstName, LastName = testLastName, DateOfBirth = testDateTime, Gender = Gender.Female });

            // assert
            Assert.AreEqual(generator.CurrentId, master.Users.Last().PersonalId);
            Assert.AreEqual(testFirstName, master.Users.Last().FirstName);
            Assert.AreEqual(testLastName, master.Users.Last().LastName);
            Assert.AreEqual(testDateTime, master.Users.Last().DateOfBirth);
            Assert.AreEqual(Gender.Female, master.Users.Last().Gender);
        }

        [TestMethod]
        public void Master_Delete_ValidUser_UserRemovedFromMaster()
        {
            // act
            var master = new MasterService(new FibonacciIdGenerator(), new TestLoader(), null, new ConnectionInfo[] { }, LogService.Instance);
            master.Load();
            var user = master.Users.First();
            master.Delete(user.PersonalId);
            
            // assert
            Assert.AreEqual(master.Users.FirstOrDefault(u => u.PersonalId == user.PersonalId), null);
        }

        [TestMethod]
        public void Master_SearchForUser_OneCriteria_ReturnFirstUserId()
        {
            // act
            var master = new MasterService(new FibonacciIdGenerator(), new TestLoader(), null, new ConnectionInfo[] { }, LogService.Instance);
            master.Load();
            var user = master.Users.First();
            var foundUsers = master.SearchForUser(new Func<User, bool>[] { u => u.LastName == user.LastName });

            // assert
            CollectionAssert.AreEqual(new List<int> { user.PersonalId }, foundUsers.ToList());
        }

        [TestMethod]
        public void Master_SearchForUser_TwoCriteria_ReturnEmptyCollection()
        {
            // act
            var master = new MasterService(new FibonacciIdGenerator(), new TestLoader(), null, new ConnectionInfo[] { }, LogService.Instance);
            master.Load();
            var user = master.Users.First();
            var foundUsers = master.SearchForUser(new Func<User, bool>[] { u => u.LastName == user.LastName, u => u.PersonalId == user.PersonalId + 1 });

            // assert
            CollectionAssert.AreEqual(new List<int>(), foundUsers.ToList());
        }
    }
}
