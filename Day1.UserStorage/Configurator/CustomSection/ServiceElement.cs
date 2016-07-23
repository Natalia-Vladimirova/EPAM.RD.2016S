using System.Configuration;

namespace Configurator.CustomSection
{
    public class ServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("serviceName", IsKey = true, IsRequired = true)]
        public string ServiceName
        {
            get { return (string)base["serviceName"]; }
            set { base["serviceName"] = value; }
        }

        [ConfigurationProperty("serviceType", IsKey = false, IsRequired = true)]
        public string ServiceType
        {
            get { return (string)base["serviceType"]; }
            set { base["serviceType"] = value; }
        }

        [ConfigurationProperty("count", IsKey = false, IsRequired = true)]
        public int Count
        {
            get { return (int)base["count"]; }
            set { base["count"] = value; }
        }
    }
}
