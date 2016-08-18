namespace UserStorage.Interfaces.Services
{
    /// <summary>
    /// Represents service for loading and saving data.
    /// </summary>
    public interface IServiceLoader
    {
        /// <summary>
        /// Loads data.
        /// </summary>
        void Load();

        /// <summary>
        /// Saves data.
        /// </summary>
        void Save();
    }
}
