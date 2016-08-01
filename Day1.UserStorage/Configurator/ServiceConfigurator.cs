using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using Configurator.Creators;
using Configurator.CustomSection;
using UserStorage.Interfaces.Creators;
using UserStorage.Interfaces.Generators;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using WcfServiceLibrary;

namespace Configurator
{
    public class ServiceConfigurator
    {
        private WcfHost masterHost;
        private List<WcfHost> slaveHosts;

        public void Start()
        {
            StartupServicesConfigSection servicesSection = (StartupServicesConfigSection)ConfigurationManager.GetSection("StartupServices");
            StartupIpConfigSection ipsSection = (StartupIpConfigSection)ConfigurationManager.GetSection("IpList");

            if (servicesSection == null)
            {
                throw new NullReferenceException("Unable to read section from config.");
            }

            int masterCount = 0;

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    masterCount++;
                }
            }

            if (masterCount > 1)
            {
                throw new ConfigurationErrorsException("Count of masters must be one.");
            }

            string masterType = string.Empty;
            string masterHostAddress = string.Empty;
            slaveHosts = new List<WcfHost>();

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    masterType = servicesSection.Services[i].ServiceType;
                    masterHostAddress = servicesSection.Services[i].HostAddress;
                }
                else
                {
                    var slaveDependencies = new Dictionary<Type, InstanceInfo>();
                    slaveDependencies.Add(typeof(IUserLoader), new InstanceInfo(servicesSection.Loader.Type));
                    slaveDependencies.Add(typeof(ILogService), new InstanceInfo(servicesSection.Logger.Type));
                    slaveDependencies.Add(typeof(IReceiver), new InstanceInfo(servicesSection.Receiver.Type, new ConnectionInfo(servicesSection.Services[i].IpAddress, servicesSection.Services[i].Port)));

                    var slaveHost = CreateServiceHost(
                        servicesSection.Services[i].ServiceType, 
                        new DependencyCreator(slaveDependencies, null), 
                        servicesSection.Services[i].HostAddress);
                    slaveHosts.Add(slaveHost);
                }
            }

            if (masterCount == 0)
            {
                return;
            }

            var slavesInfo = new List<ConnectionInfo>();

            if (ipsSection.Ips != null)
            {
                for (int i = 0; i < ipsSection.Ips.Count; i++)
                {
                    slavesInfo.Add(new ConnectionInfo(ipsSection.Ips[i].Address, ipsSection.Ips[i].Port));
                }
            }

            var masterDependencies = new Dictionary<Type, InstanceInfo>();
            masterDependencies.Add(typeof(IIdGenerator), new InstanceInfo(servicesSection.IdGenerator.Type));
            masterDependencies.Add(typeof(IUserLoader), new InstanceInfo(servicesSection.Loader.Type));
            masterDependencies.Add(typeof(ILogService), new InstanceInfo(servicesSection.Logger.Type));
            masterDependencies.Add(typeof(ISender), new InstanceInfo(servicesSection.Sender.Type, slavesInfo));

            var validators = new List<InstanceInfo>();

            if (servicesSection.Validators != null)
            {
                for (int j = 0; j < servicesSection.Validators.Count; j++)
                {
                    validators.Add(new InstanceInfo(servicesSection.Validators[j].Type));
                }
            }

            masterHost = CreateServiceHost(
                masterType, 
                new DependencyCreator(masterDependencies, new Dictionary<Type, List<InstanceInfo>> { { typeof(IValidator), validators } }), 
                masterHostAddress);
        }

        public void End()
        {
            masterHost?.Close();

            if (slaveHosts != null)
            {
                foreach (var slaveHost in slaveHosts)
                {
                    slaveHost.Close();
                }
            }
        }
        
        private WcfHost CreateServiceHost(string serviceType, IDependencyCreator creator, string hostAddress)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException($"{nameof(serviceType)} must be not null.");
            }

            Type type = Type.GetType(serviceType);

            if (type == null)
            {
                throw new NullReferenceException($"Type {serviceType} not found.");
            }

            if (type.GetInterface(typeof(IUserService).Name) == null ||
                type.GetConstructor(new[] { typeof(IDependencyCreator) }) == null)
            {
                throw new ArgumentException($"Unable to create service of type '{serviceType}' implementing interface '{nameof(IUserService)}'.");
            }

            string domainName = GetDomainNameFromHostAddress(hostAddress);
            AppDomain appDomain = AppDomain.CreateDomain(domainName);

            var host = (WcfHost)appDomain.CreateInstanceAndUnwrap(
                typeof(WcfHost).Assembly.FullName, 
                typeof(WcfHost).FullName);
            
            var service = (IUserService)appDomain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName,
                true,
                BindingFlags.CreateInstance,
                null,
                new object[] { creator },
                CultureInfo.InvariantCulture,
                null);
            
            if (service == null)
            {
                throw new ConfigurationErrorsException($"Unable to load domain of service '{domainName}'.");
            }

            host.Start(hostAddress, service);
            return host;
        }

        private string GetDomainNameFromHostAddress(string hostAddress)
        {
            return hostAddress.Substring(hostAddress.LastIndexOf('/') + 1);
        }
    }
}
