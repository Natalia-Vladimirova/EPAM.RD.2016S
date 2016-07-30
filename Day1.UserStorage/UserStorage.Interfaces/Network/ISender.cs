using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Network
{
    public interface ISender
    {
        void SendMessage(ServiceMessage message);
    }
}
