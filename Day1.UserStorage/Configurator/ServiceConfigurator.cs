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

namespace Configurator
{
    public class ServiceConfigurator
    {
        public IUserService MasterService { get; private set; }

        public List<IUserService> SlaveServices { get; private set; }

        public void Start()
        {
            StartupServicesConfigSection servicesSection = (StartupServicesConfigSection)ConfigurationManager.GetSection("StartupServices");

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

            SlaveServices = new List<IUserService>();

            string masterType = string.Empty;
            var slavesInfo = new List<ConnectionInfo>();

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    masterType = servicesSection.Services[i].ServiceType;
                }
                else
                {
                    var connectionInfo = new ConnectionInfo(servicesSection.Services[i].IpAddress, servicesSection.Services[i].Port);
                    var slaveDependencies = new Dictionary<Type, InstanceInfo>();
                    slaveDependencies.Add(typeof(IUserLoader), new InstanceInfo(servicesSection.Loader.Type));
                    slaveDependencies.Add(typeof(ILogService), new InstanceInfo(servicesSection.Logger.Type));
                    slaveDependencies.Add(typeof(IReceiver), new InstanceInfo(servicesSection.Receiver.Type, connectionInfo));

                    IUserService slaveService = CreateService($"Slave{i}", servicesSection.Services[i].ServiceType, new DependencyCreator(slaveDependencies, null));
                    SlaveServices.Add(slaveService);
                    slavesInfo.Add(connectionInfo);
                    (slaveService as IListener)?.ListenForUpdates();
                }
            }

            var masterDependencies = new Dictionary<Type, InstanceInfo>();
            masterDependencies.Add(typeof(IIdGenerator), new InstanceInfo(servicesSection.IdGenerator.Type));
            masterDependencies.Add(typeof(IUserLoader), new InstanceInfo(servicesSection.Loader.Type));
            masterDependencies.Add(typeof(ILogService), new InstanceInfo(servicesSection.Logger.Type));
            masterDependencies.Add(typeof(ISender), new InstanceInfo(servicesSection.Sender.Type, slavesInfo));

            var validators = new List<InstanceInfo>();

            for (int j = 0; j < servicesSection.Validators.Count; j++)
            {
                validators.Add(new InstanceInfo(servicesSection.Validators[j].Type));
            }

            MasterService = CreateService("Master", masterType, new DependencyCreator(masterDependencies, new Dictionary<Type, List<InstanceInfo>> { { typeof(IValidator), validators } }));
            (MasterService as IServiceLoader)?.Load();
        }

        public void End()
        {
            (MasterService as IServiceLoader)?.Save();
        }

        private IUserService CreateService(string serviceName, string serviceType, IDependencyCreator creator)
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

            AppDomain appDomain = AppDomain.CreateDomain(serviceName);

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
                throw new ConfigurationErrorsException($"Unable to load domain of service '{serviceName}'.");
            }

            return service;
        }
    }
}
