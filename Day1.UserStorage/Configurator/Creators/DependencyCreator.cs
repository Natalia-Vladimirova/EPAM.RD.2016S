using System;
using System.Collections.Generic;
using System.Linq;
using UserStorage.Interfaces.Creators;

namespace Configurator.Creators
{
    [Serializable]
    public class DependencyCreator : IDependencyCreator
    {
        private readonly Dictionary<Type, InstanceInfo> typesSingle;
        private readonly Dictionary<Type, List<InstanceInfo>> typesList;

        public DependencyCreator(Dictionary<Type, InstanceInfo> typesSingle, Dictionary<Type, List<InstanceInfo>> typesList)
        {
            this.typesSingle = typesSingle;
            this.typesList = typesList;
        }

        public T CreateInstance<T>()
        {
            if (typesSingle == null)
            {
                throw new ArgumentNullException("Creator hasn't got types.");
            }

            var info = typesSingle[typeof(T)];
            return Create<T>(info);
        }

        public IEnumerable<T> CreateListOfInstances<T>()
        {
            if (typesList == null)
            {
                throw new ArgumentNullException("Creator hasn't got types.");
            }

            var types = typesList[typeof(T)];

            if (types == null)
            {
                return null;
            }

            var instances = new List<T>();
            instances.AddRange(types.Select(Create<T>));
            return instances;
        }
        
        private T Create<T>(InstanceInfo instanceInfo)
        {
            if (instanceInfo == null)
            {
                throw new ArgumentNullException($"{nameof(instanceInfo)} must be not null.");
            }

            Type type = Type.GetType(instanceInfo.TypeName);

            if (type == null)
            {
                throw new NullReferenceException($"Type '{instanceInfo.TypeName}' not found.");
            }

            if (type.GetInterface(typeof(T).Name) == null)
            {
                throw new ArgumentException($"'{instanceInfo.TypeName}' doesn't implement interface '{typeof(T).Name}'.");
            }

            return (T)Activator.CreateInstance(type, instanceInfo.Parameters);
        }
    }
}
