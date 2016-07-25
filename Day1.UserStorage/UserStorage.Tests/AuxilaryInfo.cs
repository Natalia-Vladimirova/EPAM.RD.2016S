using System;
using System.Collections.Generic;
using System.Linq;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Tests
{
    public static class AuxilaryInfo
    {
        private static List<User> TestUsers { get; } = new List<User>
        {
            new User
            {
                PersonalId = 1,
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1996, 5, 5),
                Gender = Gender.Female,
                Visas = new List<Visa>
                {
                    new Visa
                    {
                        Country = "Britain",
                        Start = new DateTime(2015, 08, 06),
                        End = new DateTime(2015, 09, 16)
                    },
                    new Visa
                    {
                        Country = "Germany",
                        Start = new DateTime(2016, 01, 05),
                        End = new DateTime(2016, 02, 22)
                    }
                }
            },
            new User
            {
                PersonalId = 2,
                FirstName = "Tom",
                LastName = "Reddle",
                DateOfBirth = new DateTime(1995, 7, 3),
                Gender = Gender.Male,
                Visas = new List<Visa>()
            }
        };

        public static StorageState TestState { get; } = new StorageState
        {
            LastId = 2,
            Users = TestUsers.Select(u => new User
            {
                PersonalId = u.PersonalId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                Visas = u.Visas
            }).ToList()
        };

        public class TestLoader : IUserLoader
        {
            private StorageState storageState => TestState;

            public StorageState Load() => storageState;

            public void Save(StorageState state) { }
        }

    }
}
