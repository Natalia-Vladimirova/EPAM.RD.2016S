using System.Configuration;

namespace Configurator.CustomSection
{
    [ConfigurationCollection(typeof(IpElement), AddItemName = "Ip")]
    public class IpsCollection : ConfigurationElementCollection
    {
        public IpElement this[int index] => (IpElement)BaseGet(index);

        protected override ConfigurationElement CreateNewElement()
        {
            return new IpElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IpElement)element).Port;
        }
    }
}
