using System;
using System.Net;

namespace UserStorage.Interfaces.ServiceInfo
{
    [Serializable]
    public class ConnectionInfo
    {
        public IPAddress IPAddress { get; }
        public int Port { get; }

        public ConnectionInfo(string ipAddress, int port)
        {
            IPAddress address;
            if (!IPAddress.TryParse(ipAddress, out address))
            {
                throw new ArgumentException($"{nameof(ipAddress)} is invalid.");
            }
            IPAddress = address;
            Port = port;
        }
    }
}
