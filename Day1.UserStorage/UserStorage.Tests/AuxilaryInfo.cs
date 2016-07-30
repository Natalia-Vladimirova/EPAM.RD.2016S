using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Generators;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using UserStorage.Services;

namespace UserStorage.Tests
{
    public static class AuxilaryInfo
    {
        private static Dictionary<Type, string> typesSingle = new Dictionary<Type, string>
        {
            { typeof(IIdGenerator), typeof(FibonacciIdGenerator).AssemblyQualifiedName },
            { typeof(IUserLoader), typeof(TestLoader).AssemblyQualifiedName },
            { typeof(ILogService), typeof(LogService).AssemblyQualifiedName },
        };

        public static IDependencyCreator Creator => new DependencyCreator(
            typesSingle,
            new Dictionary<Type, List<string>>
            {
                { typeof(IValidator), null }
            });

        public static StorageState TestState => new StorageState
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

        private static List<User> TestUsers => new List<User>
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

        public class TestLoader : IUserLoader
        {
            private StorageState StorageState => TestState;

            public StorageState Load() => StorageState;

            public void Save(StorageState state)
            {
            }
        }
    }
}
