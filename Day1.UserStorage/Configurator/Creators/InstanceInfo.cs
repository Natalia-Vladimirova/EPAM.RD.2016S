using System;

namespace Configurator.Creators
{
    [Serializable]
    public class InstanceInfo
    {
        public InstanceInfo(string typeName, params object[] parameters)
        {
            TypeName = typeName;
            Parameters = parameters;
        }

        public string TypeName { get; }
        
        public object[] Parameters { get; }
    }
}
