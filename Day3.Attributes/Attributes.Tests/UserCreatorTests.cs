using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Attributes.Tests
{
    [TestClass]
    public class UserCreatorTests
    {
        [TestMethod]
        public void CreateUsers_ReturnListOfUsers()
        {
            //arrange
            var arrangeUsers = new User[] 
            {
                new User(1) { FirstName = "Alexander", LastName = "Alexandrov" },
                new User(2) { FirstName = "Semen", LastName = "Semenov" },
                new User(3) { FirstName = "Petr", LastName = "Petrov" }
            };

            //act
            UserCreator uc = new UserCreator();
            var actUsers = uc.CreateUsers();

            //assert
            CollectionAssert.AreEqual(arrangeUsers, actUsers);
        }

        [TestMethod]
        public void CreateUsers_ReturnListOfAdvancedUsers()
        {
            //arrange
            var arrangeUsers = new AdvancedUser[] 
            {
                new AdvancedUser(4, 2329423) { FirstName = "Pavel", LastName = "Pavlov" }
            };

            //act
            UserCreator uc = new UserCreator();
            var actUsers = uc.CreateAdvancedUsers();

            //assert
            CollectionAssert.AreEqual(arrangeUsers, actUsers);
        }

    }
}
