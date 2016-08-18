using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Network
{
    /// <summary>
    /// Used to send messages.
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sends messages.
        /// </summary>
        /// <param name="message">
        /// Message to send.
        /// </param>
        void SendMessage(ServiceMessage message);
    }
}
