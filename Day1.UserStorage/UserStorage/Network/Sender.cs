using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Network
{
    public class Sender : ISender
    {
        private readonly IEnumerable<ConnectionInfo> slavesInfo;

        public Sender(IEnumerable<ConnectionInfo> slavesInfo)
        {
            if (slavesInfo == null)
            {
                throw new ArgumentNullException($"{nameof(slavesInfo)} must be not null.");
            }

            this.slavesInfo = slavesInfo;
        }
        
        public async void SendMessage(ServiceMessage message)
        {
            var serializer = new JavaScriptSerializer();
            string serializedMessage = serializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(serializedMessage);

            foreach (var slave in slavesInfo)
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(slave.IPAddress, slave.Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
            }
        }
    }
}
