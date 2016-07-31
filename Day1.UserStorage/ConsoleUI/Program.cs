using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Configurator.Creators;
using IdGenerator;
using UserStorage.Interfaces.Creators;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Generators;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using UserStorage.Loaders;
using UserStorage.Network;
using UserStorage.Services;

namespace ConsoleUI
{
    public class Program
    {
        private static volatile bool endWork;

        private static Dictionary<Type, InstanceInfo> typesSingle = new Dictionary<Type, InstanceInfo>
        {
            { typeof(IIdGenerator), new InstanceInfo(typeof(FibonacciIdGenerator).AssemblyQualifiedName) },
            { typeof(IUserLoader), new InstanceInfo(typeof(UserXmlLoader).AssemblyQualifiedName) },
            { typeof(ILogService), new InstanceInfo(typeof(LogService).AssemblyQualifiedName) },
            { typeof(ISender), new InstanceInfo(typeof(Sender).AssemblyQualifiedName, new[] { new[] { new ConnectionInfo("127.0.0.1", 131) } }) },
            { typeof(IReceiver), new InstanceInfo(typeof(Receiver).AssemblyQualifiedName, new[] { new ConnectionInfo("127.0.0.1", 131) }) },
        };

        public static IDependencyCreator Creator => new DependencyCreator(
            typesSingle,
            new Dictionary<Type, List<InstanceInfo>>
            {
                { typeof(IValidator), null }
            });

        private static void Main(string[] args)
        {
            SaveExample();
            
            List<Thread> threads = new List<Thread>();
            var masterService = new MasterService(Creator);
            masterService.Load();
            var slaveService = new SlaveService(Creator);
            slaveService.ListenForUpdates();

            threads.Add(new Thread(() => WorkMaster(masterService)));
            threads.Add(new Thread(() => WorkSlave(slaveService)));
            
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

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static void WorkMaster(IUserService master)
        {
            while (!endWork)
            {
                master.Add(new User { FirstName = "Test", LastName = "LTest" });
                int firstId = master.Search(new Func<User, bool>[] { u => true }).FirstOrDefault();
                master.Delete(firstId);
                Thread.Sleep(1000);
            }
        }

        private static void WorkSlave(IUserService slave)
        {
            while (!endWork)
            {
                slave.Search(new Func<User, bool>[] { u => u.FirstName == "Test" });
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
