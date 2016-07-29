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

            if (masterCount != 1)
            {
                throw new ConfigurationErrorsException("Count of masters must be one.");
            }

            SlaveServices = new List<IUserService>();
            string masterType = string.Empty;
            var slavesInfo = new List<ConnectionInfo>();

            var generator = CreateInstance<IIdGenerator>(servicesSection.IdGenerator.Type);
            var loader = CreateInstance<IUserLoader>(servicesSection.Loader.Type);
            var validators = new List<IValidator>();

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    masterType = servicesSection.Services[i].ServiceType;
                }
                else
                {
                    ISlaveService slaveService = CreateSlaveService(servicesSection.Services[i], i, loader, LogService.Instance);
                    SlaveServices.Add(slaveService);
                    slavesInfo.Add(new ConnectionInfo(servicesSection.Services[i].IpAddress, servicesSection.Services[i].Port));
                    slaveService.ListenForUpdates();
                }
            }

            for (int j = 0; j < servicesSection.Validators.Count; j++)
            {
                validators.Add(CreateInstance<IValidator>(servicesSection.Validators[j].Type));
            }
            
            MasterService = CreateMasterService(masterType, generator, loader, validators, slavesInfo, LogService.Instance);
            ((IMasterService)MasterService).Load();
        }

        public void End()
        {
            ((IMasterService)MasterService).Save();
        }

        private IMasterService CreateMasterService(string serviceType, IIdGenerator generator, IUserLoader loader, IEnumerable<IValidator> validators, IEnumerable<ConnectionInfo> slavesInfo, ILogService logService)
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
                type.GetConstructor(new[]
                {
                    typeof(IIdGenerator), typeof(IUserLoader), typeof(IEnumerable<IValidator>),
                    typeof(IEnumerable<ConnectionInfo>), typeof(ILogService)
                }) == null)
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
                new object[] { generator, loader, validators, slavesInfo, logService },
                CultureInfo.InvariantCulture,
                null);

            if (master == null)
            {
                throw new ConfigurationErrorsException("Unable to load domain of master service.");
            }

            return master;
        }

        private ISlaveService CreateSlaveService(ServiceElement service, int slaveIndex, IUserLoader loader, ILogService logService)
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
                type.GetConstructor(new[] { typeof(ConnectionInfo), typeof(IUserLoader), typeof(ILogService) }) == null)
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
                new object[] { new ConnectionInfo(service.IpAddress, service.Port), loader, logService },
                CultureInfo.InvariantCulture, 
                null);

            if (slave == null)
            {
                throw new ConfigurationErrorsException($"Unable to load domain of slave service #{slaveIndex}.");
            }

            return slave;
        }

        private T CreateInstance<T>(string instanceType)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException($"{nameof(instanceType)} must be not null.");
            }

            Type type = Type.GetType(instanceType);

            if (type == null)
            {
                throw new NullReferenceException($"Type '{instanceType}' not found.");
            }

            if (type.GetInterface(typeof(T).Name) == null || type.GetConstructor(new Type[] { }) == null)
            {
                throw new ArgumentException($"Unable to create instance of type '{instanceType}' implementing interface '{typeof(T).Name}'.");
            }

            return (T)Activator.CreateInstance(type);
        }
    }
}
