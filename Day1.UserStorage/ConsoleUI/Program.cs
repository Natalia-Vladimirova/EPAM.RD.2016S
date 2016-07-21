using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage.Services;
using IdGenerator;
using UserStorage.Interfaces.Entities;
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

            var t = new LogUserService(new SlaveService(new MasterService(new FibonacciIdGenerator(), new UserXmlLoader())));
            t.Add(new User {FirstName = "jhk" });
            Console.ReadLine();
        }
    }
}
