namespace UserStorage.Interfaces.Services
{
    /// <summary>
    /// Used to get updates.
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Starts listening for updates.
        /// </summary>
        void ListenForUpdates();

        /// <summary>
        /// Stops listening for updates.
        /// </summary>
        void StopListen();
    }
}
