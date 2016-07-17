using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.CustomSection
{
    public class StartupServicesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Services")]
        public ServicesCollection ServiceItems
        {
            get { return ((ServicesCollection)(base["Services"])); }
        }
    }
}
