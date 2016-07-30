using System.Collections.Generic;

namespace UserStorage.Interfaces.Creators
{
    public interface IDependencyCreator
    {
        T CreateInstance<T>();

        IEnumerable<T> CreateListOfInstances<T>();
    }
}
