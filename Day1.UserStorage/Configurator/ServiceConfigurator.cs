using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Configurator.CustomSection;
using IdGenerator;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using System.Globalization;

namespace Configurator
{
    public class ServiceConfigurator
    {
        private IUserService masterService;
        private List<IUserService> slaveServices;

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

            slaveServices = new List<IUserService>();

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                if (servicesSection.Services[i].IsMaster)
                {
                    var generator = CreateInstance<IIdGenerator>(servicesSection.IdGenerator.Type);
                    var loader = CreateInstance<IUserLoader>(servicesSection.Loader.Type);
                    var validators = new List<IValidator>();

                    for (int j = 0; j < servicesSection.Validators.Count; j++)
                    {
                        validators.Add(CreateInstance<IValidator>(servicesSection.Validators[j].Type));
                    }

                    masterService = CreateMasterService(servicesSection.Services[i].ServiceType, generator, loader, validators);
                    ((IMasterService)masterService).Load();
                }
                else
                {
                    IUserService slaveService = CreateSlaveService(servicesSection.Services[i].ServiceType, i, (IMasterService)masterService);
                    slaveServices.Add(slaveService);
                }
            }
        }

        public void End()
        {
            ((IMasterService)masterService).Save();
        }

        private IMasterService CreateMasterService(string serviceType, IIdGenerator generator, IUserLoader loader, IEnumerable<IValidator> validators)
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
                type.GetConstructor(new[] { typeof(IIdGenerator), typeof(IUserLoader), typeof(IEnumerable<IValidator>) }) == null)
            {
                throw new ArgumentException($"Unable to create service of type '{serviceType}' implementing interface '{nameof(IMasterService)}'.");
            }

            AppDomain appDomain = AppDomain.CreateDomain("Master");

            var master = (IMasterService)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true,
                BindingFlags.CreateInstance, null,
                new object[] { generator, loader, validators },
                CultureInfo.InvariantCulture, null);

            if (master == null)
            {
                throw new ConfigurationErrorsException("Unable to load domain of master service.");
            }
            return master;
        }
        
        private IUserService CreateSlaveService(string serviceType, int slaveIndex, IMasterService master)
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

            if (type.GetInterface(typeof(IUserService).Name) == null || type.GetConstructor(new[] { typeof(IMasterService) }) == null)
            {
                throw new ArgumentException($"Unable to create service of type '{serviceType}' implementing interface '{nameof(IUserService)}'.");
            }

            AppDomain appDomain = AppDomain.CreateDomain($"Slave{slaveIndex}");

            var slave = (IUserService)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true,
                BindingFlags.CreateInstance, null,
                new object[] { master },
                CultureInfo.InvariantCulture, null);

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
