using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Loaders
{
    /// <summary>
    /// Used to load and save storage state.
    /// </summary>
    public interface IUserLoader
    {
        /// <summary>
        /// Loads storage state.
        /// </summary>
        /// <returns>
        /// Saved storage state.
        /// </returns>
        StorageState Load();

        /// <summary>
        /// Saves storage state.
        /// </summary>
        /// <param name="state">
        /// Storage state to save.
        /// </param>
        void Save(StorageState state);
    }
}
