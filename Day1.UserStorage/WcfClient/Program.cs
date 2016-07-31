using System;
using System.Linq;
using WcfClient.MasterServiceReference;

namespace WcfClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            WcfUserServiceClient client = new WcfUserServiceClient("BasicHttpBinding_IWcfUserService");
            int id = client.Search(new Func<User, bool>[] { u => u.PersonalId == 1 }).FirstOrDefault();
        }
    }
}
