using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Network
{
    /// <summary>
    /// Used to send messages using tcp connection.
    /// </summary>
    public class Sender : ISender
    {
        private readonly IEnumerable<ConnectionInfo> connectionInfo;

        /// <summary>
        /// Creates an instance of tcp sender with necessary connection info.
        /// </summary>
        /// <param name="connectionInfo">
        /// Contains information about ip addresses and ports where tcp sender will send messages.
        /// </param>
        public Sender(IEnumerable<ConnectionInfo> connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException($"{nameof(connectionInfo)} must be not null.");
            }

            this.connectionInfo = connectionInfo;
        }

        /// <summary>
        /// Sends messages using given addresses and ports via tcp connection.
        /// </summary>
        /// <param name="message">
        /// Message to send.
        /// </param>
        public async void SendMessage(ServiceMessage message)
        {
            var serializer = new JavaScriptSerializer();
            string serializedMessage = serializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(serializedMessage);

            foreach (var info in connectionInfo)
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(info.IPAddress, info.Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
        }
    }
}
