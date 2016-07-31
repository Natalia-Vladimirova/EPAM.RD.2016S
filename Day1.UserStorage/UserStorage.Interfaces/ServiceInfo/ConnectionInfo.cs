using System;
using System.Net;

namespace UserStorage.Interfaces.ServiceInfo
{
    [Serializable]
    public class ConnectionInfo
    {
        public ConnectionInfo(string address, int port)
        {
            IPAddress parsedAddress;
            if (!IPAddress.TryParse(address, out parsedAddress))
            {
                throw new ArgumentException($"{nameof(address)} is invalid.");
            }

            IPAddress = parsedAddress;
            Port = port;
        }

        public IPAddress IPAddress { get; }

        public int Port { get; }
    }
}
