using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Configurator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Loaders;

namespace ConsoleUI
{
    public class Program
    {
        private static volatile bool endWork;

        private static void Main(string[] args)
        {
            SaveExample();
            
            List<Thread> threads = new List<Thread>();
            var configurator = new ServiceConfigurator();
            configurator.Start();

            threads.Add(new Thread(() => WorkMaster(configurator.MasterService)));
            threads.AddRange(configurator.SlaveServices.Select(slave => new Thread(() => WorkSlave(slave))));
            
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            
            Console.WriteLine("Press any key to end work of services.");
            Console.ReadKey();
            endWork = true;

            foreach (var thread in threads)
            {
                thread.Join();
            }

            configurator.End();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static void WorkMaster(IUserService master)
        {
            while (!endWork)
            {
                master.Add(new User { FirstName = "Test", LastName = "LTest" });
                int firstId = master.SearchForUser(new Func<User, bool>[] { u => true }).FirstOrDefault();
                master.Delete(firstId);
                Thread.Sleep(1000);
            }
        }

        private static void WorkSlave(IUserService slave)
        {
            while (!endWork)
            {
                slave.SearchForUser(new Func<User, bool>[] { u => u.FirstName == "Test" });
                Thread.Sleep(1000);
            }
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
