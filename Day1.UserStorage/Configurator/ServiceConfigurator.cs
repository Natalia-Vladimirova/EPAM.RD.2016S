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
            // read from config
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
                    var generator = CreateIdGenerator(servicesSection.IdGenerator.Type);
                    var loader = CreateUserLoader(servicesSection.Loader.Type);
                    var validators = new List<IValidator>();

                    for (int j = 0; j < servicesSection.Validators.Count; j++)
                    {
                        validators.Add(CreateValidator(servicesSection.Validators[j].Type));
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

            if (type.GetInterface("IMasterService") == null)
            {
                throw new ArgumentException($"Type {serviceType} doesn't implement interface {nameof(IMasterService)}.");
            }

            var ctor = type.GetConstructor(new[] { typeof(IIdGenerator), typeof(IUserLoader), typeof(IEnumerable<IValidator>) });

            if (ctor == null)
            {
                throw new ArgumentException($"Type {serviceType} doesn't have necessary constructor.");
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

        private IIdGenerator CreateIdGenerator(string generatorType)
        {
            if (generatorType == null)
            {
                throw new ArgumentNullException($"{nameof(generatorType)} must be not null.");
            }

            Type type = Type.GetType(generatorType);

            if (type == null)
            {
                throw new NullReferenceException($"Type {generatorType} not found.");
            }

            if (type.GetInterface("IIdGenerator") == null)
            {
                throw new ArgumentException($"Type {generatorType} doesn't implement interface {nameof(IIdGenerator)}.");
            }

            var ctor = type.GetConstructor(new Type[] {});

            if (ctor == null)
            {
                throw new ArgumentException($"Type {generatorType} doesn't have necessary constructor.");
            }

            return (IIdGenerator) ctor.Invoke(new object[] {});
        }

        private IUserLoader CreateUserLoader(string loaderType)
        {
            if (loaderType == null)
            {
                throw new ArgumentNullException($"{nameof(loaderType)} must be not null.");
            }

            Type type = Type.GetType(loaderType);

            if (type == null)
            {
                throw new NullReferenceException($"Type {loaderType} not found.");
            }

            if (type.GetInterface("IUserLoader") == null)
            {
                throw new ArgumentException($"Type {loaderType} doesn't implement interface {nameof(IUserLoader)}.");
            }

            var ctor = type.GetConstructor(new Type[] { });

            if (ctor == null)
            {
                throw new ArgumentException($"Type {loaderType} doesn't have necessary constructor.");
            }

            return (IUserLoader)ctor.Invoke(new object[] { });
        }

        private IValidator CreateValidator(string validatorType)
        {
            if (validatorType == null)
            {
                throw new ArgumentNullException($"{nameof(validatorType)} must be not null.");
            }

            Type type = Type.GetType(validatorType);

            if (type == null)
            {
                throw new NullReferenceException($"Type {validatorType} not found.");
            }

            if (type.GetInterface("IValidator") == null)
            {
                throw new ArgumentException($"Type {validatorType} doesn't implement interface {nameof(IValidator)}.");
            }

            var ctor = type.GetConstructor(new Type[] { });

            if (ctor == null)
            {
                throw new ArgumentException($"Type {validatorType} doesn't have necessary constructor.");
            }

            return (IValidator)ctor.Invoke(new object[] { });
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

            if (type.GetInterface("IUserService") == null)
            {
                throw new ArgumentException($"Type {serviceType} doesn't implement interface {nameof(IUserService)}.");
            }

            var ctor = type.GetConstructor(new[] { typeof(IMasterService) });

            if (ctor == null)
            {
                throw new ArgumentException($"Type {serviceType} doesn't have necessary constructor.");
            }

            AppDomain appDomain = AppDomain.CreateDomain($"Slave{slaveIndex}");

            var slave = (IUserService)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true,
                BindingFlags.CreateInstance, null,
                new object[] { master },
                CultureInfo.InvariantCulture, null);

            return slave;
        }

    }
}
