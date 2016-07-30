using System.Configuration;

namespace Configurator.CustomSection
{
    public class StartupServicesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("IdGenerator")]
        public StartupServiceElement IdGenerator => (StartupServiceElement)base["IdGenerator"];

        [ConfigurationProperty("Loader")]
        public StartupServiceElement Loader => (StartupServiceElement)base["Loader"];

        [ConfigurationProperty("Logger")]
        public StartupServiceElement Logger => (StartupServiceElement)base["Logger"];

        [ConfigurationProperty("Sender")]
        public StartupServiceElement Sender => (StartupServiceElement)base["Sender"];

        [ConfigurationProperty("Receiver")]
        public StartupServiceElement Receiver => (StartupServiceElement)base["Receiver"];

        [ConfigurationProperty("Validators")]
        public ValidatorsCollection Validators => (ValidatorsCollection)base["Validators"];

        [ConfigurationProperty("Services")]
        public ServicesCollection Services => (ServicesCollection)base["Services"];
    }
}
