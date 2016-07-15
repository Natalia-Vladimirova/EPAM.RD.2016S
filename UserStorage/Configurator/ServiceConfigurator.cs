using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
    public class ServiceConfigurator
    {
        public void Start()
        {
            // TODO: read from config
            // TODO: validate number of master (only one - good, in other case - exception)
            // TODO: create master and slaves and give them their strategy; also slave must subscribe to master
            // TODO: call load function in master amd slaves
        }

        public void End()
        {
            // TODO: unsubscribe from events ?!
            // TODO: master need to save users list
        }

    }
}
