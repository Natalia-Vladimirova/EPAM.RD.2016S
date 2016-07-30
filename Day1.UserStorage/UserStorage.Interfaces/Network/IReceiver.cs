using System;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Network
{
    public interface IReceiver
    {
        event EventHandler<UserEventArgs> Updating;

        void StartReceivingMessages();

        void StopReceiver();
    }
}
