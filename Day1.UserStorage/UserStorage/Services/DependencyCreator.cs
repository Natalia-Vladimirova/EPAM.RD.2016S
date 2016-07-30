using System;
using System.Collections.Generic;
using System.Linq;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    [Serializable]
    public class DependencyCreator : IDependencyCreator
    {
        private readonly Dictionary<Type, string> typesSingle;
        private readonly Dictionary<Type, List<string>> typesList;

        public DependencyCreator(Dictionary<Type, string> typesSingle, Dictionary<Type, List<string>> typesList)
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

            string type = typesSingle[typeof(T)];
            return Create<T>(type);
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

        private T Create<T>(string instanceType)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException($"{nameof(instanceType)} must be not null.");
            }

            Type type = Type.GetType(instanceType);

            if (type == null)
            {
                throw new NullReferenceException($"Type '{instanceType}' not found.");
            }

            if (type.GetInterface(typeof(T).Name) == null || type.GetConstructor(new Type[] { }) == null)
            {
                throw new ArgumentException($"Unable to create instance of type '{instanceType}' implementing interface '{typeof(T).Name}'.");
            }

            return (T)Activator.CreateInstance(type);
        }
    }
}
