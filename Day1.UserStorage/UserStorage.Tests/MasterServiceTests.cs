using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserStorage.Interfaces.Entities;
using UserStorage.Services;
using static UserStorage.Tests.AuxilaryInfo;

namespace UserStorage.Tests
{
    [TestClass]
    public class MasterServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Master_Ctor_NullCreator_ThrowAnException()
        {
            new MasterService(null);
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
            var master = new MasterService(Creator);
            master.Load();
            master.Add(new User { FirstName = testFirstName, LastName = testLastName, DateOfBirth = testDateTime, Gender = Gender.Female });

            // assert
            Assert.AreEqual(generator.CurrentId, master.GetAll().Last().PersonalId);
            Assert.AreEqual(testFirstName, master.GetAll().Last().FirstName);
            Assert.AreEqual(testLastName, master.GetAll().Last().LastName);
            Assert.AreEqual(testDateTime, master.GetAll().Last().DateOfBirth);
            Assert.AreEqual(Gender.Female, master.GetAll().Last().Gender);
        }

        [TestMethod]
        public void Master_Delete_ValidUser_UserRemovedFromMaster()
        {
            // act
            var master = new MasterService(Creator);
            master.Load();
            var user = master.GetAll().First();
            master.Delete(user.PersonalId);
            
            // assert
            Assert.AreEqual(master.GetAll().FirstOrDefault(u => u.PersonalId == user.PersonalId), null);
        }

        [TestMethod]
        public void Master_SearchForUser_OneCriteria_ReturnFirstUserId()
        {
            // act
            var master = new MasterService(Creator);
            master.Load();
            var user = master.GetAll().First();
            var foundUsers = master.Search(new Func<User, bool>[] { u => u.LastName == user.LastName });

            // assert
            CollectionAssert.AreEqual(new List<int> { user.PersonalId }, foundUsers.ToList());
        }

        [TestMethod]
        public void Master_SearchForUser_TwoCriteria_ReturnEmptyCollection()
        {
            // act
            var master = new MasterService(Creator);
            master.Load();
            var user = master.GetAll().First();
            var foundUsers = master.Search(new Func<User, bool>[] { u => u.LastName == user.LastName, u => u.PersonalId == user.PersonalId + 1 });

            // assert
            CollectionAssert.AreEqual(new List<int>(), foundUsers.ToList());
        }
    }
}
