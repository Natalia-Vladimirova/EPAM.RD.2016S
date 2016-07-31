using System;
using System.Collections.Generic;
using Configurator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Loaders;

namespace WcfService
{
    public class Program
    {
        private static void Main(string[] args)
        {
            SaveExample();

            var configurator = new ServiceConfigurator();
            configurator.Start();

            Console.WriteLine("Press <Enter> to stop the services.");
            Console.ReadLine();

            configurator.End();
        }

        private static void SaveExample()
        {
            var loader = new UserXmlLoader();

            var users = new List<User>
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
                    Visas = new List<Visa>
                    {
                        new Visa
                        {
                            Country = "Russia",
                            Start = new DateTime(2014, 03, 01),
                            End = new DateTime(2015, 03, 22)
                        }
                    }
                },
                new User
                {
                    PersonalId = 3,
                    FirstName = "Kate",
                    LastName = "Brown",
                    DateOfBirth = new DateTime(1995, 4, 13),
                    Gender = Gender.Female,
                    Visas = new List<Visa>()
                }
            };

            var state = new StorageState
            {
                LastId = 3,
                Users = users
            };

            loader.Save(state);
        }
    }
}
