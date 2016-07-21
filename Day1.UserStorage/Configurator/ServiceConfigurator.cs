using System;
using System.Collections.Generic;
using System.Configuration;
using Configurator.CustomSection;
using IdGenerator;
using UserStorage.Interfaces.Services;
using UserStorage.Loaders;
using UserStorage.Services;

namespace Configurator
{
    public class ServiceConfigurator
    {
        private IMasterService masterService;
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

            for (int i = 0; i < servicesSection.ServiceItems.Count; i++)
            {
                // validate number of master (only one - good, in other case - exception)
                if (servicesSection.ServiceItems[i].ServiceType == "Master" && servicesSection.ServiceItems[i].Count != 1)
                {
                    throw new ConfigurationErrorsException("Count of masters must be one.");
                }

                if (servicesSection.ServiceItems[i].ServiceType == "Slave")
                {
                    slavesCount = servicesSection.ServiceItems[i].Count;
                }
            }

            // create master and slaves and give them their strategies
            masterService = new MasterService(new FibonacciIdGenerator(), new UserXmlLoader());
            masterService.Load();

            slaveServices = new List<IUserService>();

            for (int i = 0; i < slavesCount; i++)
            {
                IUserService slaveService = new SlaveService(masterService);
                slaveServices.Add(slaveService);
            }
        }

        public void End()
        {
            // TODO: unsubscribe from events ?!

            // master need to save storage state
            masterService.Save();
        }

    }
}
