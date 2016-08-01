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

namespace MultiThreadingExample
{
    public class Program
    {
        private static Dictionary<Type, InstanceInfo> typesSingle = new Dictionary<Type, InstanceInfo>
        {
            { typeof(IIdGenerator), new InstanceInfo(typeof(FibonacciIdGenerator).AssemblyQualifiedName) },
            { typeof(IUserLoader), new InstanceInfo(typeof(UserXmlLoader).AssemblyQualifiedName) },
            { typeof(ILogService), new InstanceInfo(typeof(LogService).AssemblyQualifiedName) },
            { typeof(ISender), new InstanceInfo(typeof(Sender).AssemblyQualifiedName, new[] { new ConnectionInfo[] { } }) },
        };

        private static IDependencyCreator creator = new DependencyCreator(
            typesSingle,
            new Dictionary<Type, List<InstanceInfo>>
            {
                { typeof(IValidator), null }
            });

        private static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();
            var masterService = new MasterService(creator);
            masterService.Load();

            threads.Add(new Thread(() =>
            {
                masterService.Add(new User { FirstName = "Test", LastName = "qwerty" });
            }));

            threads.Add(new Thread(() =>
            {
                masterService.Add(new User { FirstName = "Test2", LastName = "qwerty2" });
            }));

            threads.Add(new Thread(() =>
            {
                int firstId = masterService.Search(u => true).FirstOrDefault();
                masterService.Delete(firstId);
            }));

            threads.Add(new Thread(() =>
            {
                int lastId = masterService.Search(u => true).LastOrDefault();
                masterService.Delete(lastId);
            }));

            threads.Add(new Thread(() =>
            {
                masterService.Search(u => true);
            }));

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Ready!");
            Console.ReadLine();
        }
    }
}
