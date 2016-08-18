using System;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Network
{
    /// <summary>
    /// Used to receive messages.
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Updating is raised whenever message is received.
        /// </summary>
        event EventHandler<UserEventArgs> Updating;

        /// <summary>
        /// Starts receiving messages.
        /// </summary>
        void StartReceivingMessages();

        /// <summary>
        /// Stops receiving messages.
        /// </summary>
        void StopReceiver();
    }
}
