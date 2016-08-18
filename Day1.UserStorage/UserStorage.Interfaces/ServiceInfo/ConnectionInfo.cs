using System;
using System.Net;

namespace UserStorage.Interfaces.ServiceInfo
{
    /// <summary>
    /// Used to contain information about ip address and port.
    /// </summary>
    [Serializable]
    public class ConnectionInfo
    {
        /// <summary>
        /// Creates an instance of ConnectionInfo with specified ip address and port.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
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
