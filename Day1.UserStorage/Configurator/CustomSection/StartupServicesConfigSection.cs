using System.Configuration;

namespace Configurator.CustomSection
{
    public class StartupServicesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("IdGenerator")]
        public StartupServiceElement IdGenerator => (StartupServiceElement)base["IdGenerator"];

        [ConfigurationProperty("Loader")]
        public StartupServiceElement Loader => (StartupServiceElement)base["Loader"];

        [ConfigurationProperty("Validators")]
        public ValidatorsCollection Validators => (ValidatorsCollection)base["Validators"];

        [ConfigurationProperty("Services")]
        public ServicesCollection Services => (ServicesCollection)base["Services"];
    }
}
