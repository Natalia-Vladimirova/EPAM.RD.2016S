using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using Configurator.CustomSection;
using UserStorage.Interfaces.Generators;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using UserStorage.Services;

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
            
            var dependencies = new Dictionary<Type, string>();
            dependencies.Add(typeof(IIdGenerator), servicesSection.IdGenerator.Type);
            dependencies.Add(typeof(IUserLoader), servicesSection.Loader.Type);
            dependencies.Add(typeof(ILogService), servicesSection.Logger.Type);

            var validators = new List<string>();
            
            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    masterType = servicesSection.Services[i].ServiceType;
                }
                else
                {
                    ISlaveService slaveService = CreateSlaveService(servicesSection.Services[i], i, new DependencyCreator(dependencies, null));
                    SlaveServices.Add(slaveService);
                    slavesInfo.Add(new ConnectionInfo(servicesSection.Services[i].IpAddress, servicesSection.Services[i].Port));
                    slaveService.ListenForUpdates();
                }
            }

            for (int j = 0; j < servicesSection.Validators.Count; j++)
            {
                validators.Add(servicesSection.Validators[j].Type);
            }
            
            MasterService = CreateMasterService(masterType, new DependencyCreator(dependencies, new Dictionary<Type, List<string>> { { typeof(IValidator), validators } }), slavesInfo);
            ((IMasterService)MasterService).Load();
        }

        public void End()
        {
            ((IMasterService)MasterService).Save();
        }

        private IMasterService CreateMasterService(string serviceType, IDependencyCreator creator, IEnumerable<ConnectionInfo> slavesInfo)
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

            if (type.GetInterface(typeof(IMasterService).Name) == null ||
                type.GetConstructor(new[] { typeof(IDependencyCreator), typeof(IEnumerable<ConnectionInfo>) }) == null)
            {
                throw new ArgumentException($"Unable to create service of type '{serviceType}' implementing interface '{nameof(IMasterService)}'.");
            }

            AppDomain appDomain = AppDomain.CreateDomain("Master");

            var master = (IMasterService)appDomain.CreateInstanceAndUnwrap(
                type.Assembly.FullName, 
                type.FullName, 
                true,
                BindingFlags.CreateInstance, 
                null,
                new object[] { creator, slavesInfo },
                CultureInfo.InvariantCulture,
                null);

            if (master == null)
            {
                throw new ConfigurationErrorsException("Unable to load domain of master service.");
            }

            return master;
        }

        private ISlaveService CreateSlaveService(ServiceElement service, int slaveIndex, IDependencyCreator creator)
        {
            if (service.ServiceType == null)
            {
                throw new ArgumentNullException($"{nameof(service.ServiceType)} must be not null.");
            }

            Type type = Type.GetType(service.ServiceType);

            if (type == null)
            {
                throw new NullReferenceException($"Type {service.ServiceType} not found.");
            }

            if (type.GetInterface(typeof(ISlaveService).Name) == null || 
                type.GetConstructor(new[] { typeof(IDependencyCreator), typeof(ConnectionInfo) }) == null)
            {
                throw new ArgumentException($"Unable to create service of type '{service.ServiceType}' implementing interface '{nameof(ISlaveService)}'.");
            }

            AppDomain appDomain = AppDomain.CreateDomain($"Slave{slaveIndex}");

            var slave = (ISlaveService)appDomain.CreateInstanceAndUnwrap(
                type.Assembly.FullName, 
                type.FullName, 
                true,
                BindingFlags.CreateInstance, 
                null, 
                new object[] { creator, new ConnectionInfo(service.IpAddress, service.Port) },
                CultureInfo.InvariantCulture, 
                null);

            if (slave == null)
            {
                throw new ConfigurationErrorsException($"Unable to load domain of slave service #{slaveIndex}.");
            }

            return slave;
        }
    }
}
