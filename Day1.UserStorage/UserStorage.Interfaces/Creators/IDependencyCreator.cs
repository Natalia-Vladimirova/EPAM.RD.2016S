using System.Collections.Generic;

namespace UserStorage.Interfaces.Creators
{
    /// <summary>
    /// Used to create instances according to interfaces.
    /// </summary>
    public interface IDependencyCreator
    {
        /// <summary>
        /// Creates a single object that implements interface T.
        /// </summary>
        /// <returns>
        /// Object implementing interface T.
        /// </returns>
        T CreateInstance<T>();

        /// <summary>
        /// Creates a list of objects that implement interface T.
        /// </summary>
        /// <returns>
        /// List of objects implementing interface T.
        /// </returns>
        IEnumerable<T> CreateListOfInstances<T>();
    }
}
