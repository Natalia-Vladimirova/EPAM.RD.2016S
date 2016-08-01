using System.Configuration;

namespace Configurator.CustomSection
{
    public class StartupIpConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Ips")]
        public IpsCollection Ips => (IpsCollection)base["Ips"];
    }
}
