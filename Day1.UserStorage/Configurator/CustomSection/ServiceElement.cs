using System.Configuration;

namespace Configurator.CustomSection
{
    public class ServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("isMaster", DefaultValue = false, IsRequired = false)]
        public bool IsMaster
        {
            get { return (bool)base["isMaster"]; }
            set { base["isMaster"] = value; }
        }

        [ConfigurationProperty("serviceType", IsRequired = true)]
        public string ServiceType
        {
            get { return (string)base["serviceType"]; }
            set { base["serviceType"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }
    }
}
