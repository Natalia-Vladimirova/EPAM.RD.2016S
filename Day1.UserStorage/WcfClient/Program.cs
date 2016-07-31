using System;
using WcfClient.MasterServiceReference;

namespace WcfClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            WcfUserServiceClient client = new WcfUserServiceClient("BasicHttpBinding_IWcfUserService");
            int id = client.Add(new User { FirstName = "Test", LastName = "LTest" });
            client.Delete(id);

            Console.WriteLine("Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
