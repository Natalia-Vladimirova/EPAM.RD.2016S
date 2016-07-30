using System.Collections.Generic;

namespace UserStorage.Interfaces.Services
{
    public interface IDependencyCreator
    {
        T CreateInstance<T>();

        IEnumerable<T> CreateListOfInstances<T>();
    }
}
