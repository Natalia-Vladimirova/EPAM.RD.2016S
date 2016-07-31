using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using UserStorage.Interfaces.Services;

namespace WcfServiceLibrary
{
    public class WcfHost : MarshalByRefObject
    {
        private ServiceHost host;
        private IUserService userService;

        public void Start(string address, IUserService userService)
        {
            Uri baseAddress = new Uri(address);
            var service = new WcfUserService(userService);
            host = new ServiceHost(service, baseAddress);
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);

            (userService as IServiceLoader)?.Load();
            (userService as IListener)?.ListenForUpdates();
            this.userService = userService;

            host.Open();

            Console.WriteLine($"Domain: {AppDomain.CurrentDomain.FriendlyName}");
            Console.WriteLine($"The service is ready at {baseAddress}");
            Console.WriteLine();
        }

        public void Close()
        {
            host.Close();

            (host as IDisposable)?.Dispose();
        }
    }
}
