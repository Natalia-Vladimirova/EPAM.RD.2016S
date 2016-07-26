using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Configurator;
using UserStorage.Interfaces.Entities;

namespace MultiThreadingExample
{
    public class Program
    {
        private static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();
            var configurator = new ServiceConfigurator();
            configurator.Start();
            
            threads.Add(new Thread(() =>
            {
                configurator.MasterService.Add(new User {FirstName = "Test", LastName = "qwerty"});
            }));

            threads.Add(new Thread(() =>
            {
                configurator.MasterService.Add(new User { FirstName = "Test2", LastName = "qwerty2" });
            }));

            threads.Add(new Thread(() =>
            {
                int firstId = configurator.MasterService.SearchForUser(new Func<User, bool>[] { u => true }).FirstOrDefault();
                configurator.MasterService.Delete(firstId);
            }));

            threads.Add(new Thread(() =>
            {
                int lastId = configurator.MasterService.SearchForUser(new Func<User, bool>[] { u => true }).LastOrDefault();
                configurator.MasterService.Delete(lastId);
            }));

            threads.Add(new Thread(() =>
            {
                configurator.MasterService.SearchForUser(new Func<User, bool>[] { u => true });
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
