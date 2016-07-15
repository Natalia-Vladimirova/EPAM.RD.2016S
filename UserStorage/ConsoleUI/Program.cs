using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage.Services;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new UserLoader();
            loader.Save(null);
        }
    }
}
