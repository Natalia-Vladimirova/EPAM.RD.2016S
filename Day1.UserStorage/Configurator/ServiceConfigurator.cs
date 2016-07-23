using System;
using System.Collections.Generic;
using System.Configuration;
using Configurator.CustomSection;
using IdGenerator;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;

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

            int slavesCount = 0;
            string masterType = null;
            string slaveType = null;

            for (int i = 0; i < servicesSection.Services.Count; i++)
            {
                // validate number of master (only one - good, in other case - exception)
                if (servicesSection.Services[i].ServiceName == "Master")
                {
                    if (servicesSection.Services[i].Count != 1)
                    {
                        throw new ConfigurationErrorsException("Count of masters must be one.");
                    }
                    masterType = servicesSection.Services[i].ServiceType;
                }

                if (servicesSection.Services[i].ServiceName == "Slave")
                {
                    slavesCount = servicesSection.Services[i].Count;
                    slaveType = servicesSection.Services[i].ServiceType;
                }
            }

            var generator = CreateIdGenerator(servicesSection.IdGenerator.Type);
            var loader = CreateUserLoader(servicesSection.Loader.Type);
            var validators = new List<IValidator>();

            for (int i = 0; i < servicesSection.Validators.Count; i++)
            {
                validators.Add(CreateValidator(servicesSection.Validators[i].Type));
            }

            // create master and slaves and give them their strategies
            masterService = CreateMasterService(masterType, generator, loader, validators);
            ((IMasterService)masterService).Load();

            slaveServices = new List<IUserService>();

            for (int i = 0; i < slavesCount; i++)
            {
                IUserService slaveService = CreateSlaveService(slaveType, (IMasterService)masterService);
                slaveServices.Add(slaveService);
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

            var ctor = type.GetConstructor(new [] { typeof(IIdGenerator), typeof(IUserLoader), typeof(IEnumerable<IValidator>) });

            if (ctor == null)
            {
                throw new ArgumentException($"Type {serviceType} doesn't have necessary constructor.");
            }

            return (IMasterService)ctor.Invoke(new object[] { generator, loader, validators });
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

        private IUserService CreateSlaveService(string serviceType, IMasterService master)
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
            
            return (IUserService)ctor.Invoke(new object[] { master });
        }

    }
}
