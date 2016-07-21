using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage.Services;
using Configurator;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Strategies;
using UserStorage.Loaders;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //var loader = new UserLoader();
            //loader.Save(null);
            /*var configurator = new ServiceConfigurator();
            configurator.Start();*/

            var t = new LogUserService(new UserService(new MasterStrategy(new FibonacciIdGenerator(), new List<User>())));
            t.Add(new User {FirstName = "jhk" });
            Console.ReadLine();
        }
    }
}
