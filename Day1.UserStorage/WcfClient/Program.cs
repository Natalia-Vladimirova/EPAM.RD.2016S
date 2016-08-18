using System;
using WcfClient.MasterServiceReference;

namespace WcfClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            WcfUserServiceClient client = new WcfUserServiceClient("BasicHttpBinding_IWcfUserService");

            while (true)
            {
                int id = client.Add(new User { FirstName = "Test", LastName = "LTest" });
                client.Search(null);
                client.Delete(id);
                client.Search(null);

                Console.WriteLine("Press <Enter> to continue.");
                Console.WriteLine("Press other key to stop loop.");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }
            }

            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
