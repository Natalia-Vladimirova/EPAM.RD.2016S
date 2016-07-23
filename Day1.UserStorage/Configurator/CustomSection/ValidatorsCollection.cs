using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.CustomSection
{
    [ConfigurationCollection(typeof(StartupServiceElement), AddItemName = "Validator")]
    public class ValidatorsCollection : ConfigurationElementCollection
    {
        public StartupServiceElement this[int index] => (StartupServiceElement)BaseGet(index);

        protected override ConfigurationElement CreateNewElement()
        {
            return new StartupServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StartupServiceElement)element).Type;
        }
    }
}
