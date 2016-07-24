using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configurator;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Services;
using System.Threading;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //SaveExample();

            //var loader = new UserXmlLoader();
            //var state = loader.Load();

            var configurator = new ServiceConfigurator();
            configurator.Start();
            configurator.MasterService.Add(new User { FirstName = "Test", LastName = "qwerty" });
            ShowUsers(configurator.MasterService.Users);
            Console.WriteLine();
            Thread.Sleep(1000);
            ShowSlaves(configurator.SlaveServices);

            //var t = new LogUserService(new SlaveService(new MasterService(new FibonacciIdGenerator(), new UserXmlLoader())));
            Console.WriteLine("Ready!");
            Console.ReadLine();
        }

        static void ShowSlaves(IEnumerable<IUserService> slaves)
        {
            foreach (var slave in slaves)
            {
                ShowUsers(slave.Users);
                Console.WriteLine();
            }
        }

        static void ShowUsers(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                Console.WriteLine($"{user.PersonalId})\t{user.FirstName} {user.LastName}; {user.Gender}; {user.DateOfBirth}");
                Console.Write($"Visas: ");

                if (user.Visas == null)
                {
                    Console.WriteLine("no visas");
                    return;
                }

                foreach (var visa in user.Visas)
                {
                    Console.Write($"{visa.Country}  ");
                }
                Console.WriteLine();
            }
        }

        static void SaveExample()
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
