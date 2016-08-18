using System;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Network
{
    /// <summary>
    /// Used to receive messages using tcp connection.
    /// </summary>
    public class Receiver : IReceiver
    {
        private readonly TcpListener server;

        /// <summary>
        /// Creates an instance of tcp receiver with necessary connection info.
        /// </summary>
        /// <param name="info">
        /// Contains information about ip address and port which tcp receiver will start on.
        /// </param>
        public Receiver(ConnectionInfo info)
        {
            server = new TcpListener(info.IPAddress, info.Port);
            server.Start();
        }

        /// <summary>
        /// Updating is raised whenever message is received.
        /// </summary>
        public event EventHandler<UserEventArgs> Updating = delegate { };

        /// <summary>
        /// Starts receiving messages via tcp connection.
        /// </summary>
        public async void StartReceivingMessages()
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                while (true)
                {
                    using (TcpClient client = await server.AcceptTcpClientAsync())
                    using (NetworkStream stream = client.GetStream())
                    {
                        string serializedMessage = string.Empty;
                        byte[] data = new byte[1024];

                        int i = 0;
                        do
                        {
                            i = await stream.ReadAsync(data, 0, data.Length);
                            serializedMessage += Encoding.UTF8.GetString(data, 0, i);
                        }
                        while (i >= 1024);
                        
                        try
                        {
                            ServiceMessage message = serializer.Deserialize<ServiceMessage>(serializedMessage);
                            OnUpdate(this, new UserEventArgs(message.User, message.Operation));
                        }
                        catch
                        {
                            throw new InvalidOperationException("Unable to deserialize request.");
                        }
                    }
                }
            }
            finally
            {
                server.Stop();
            }
        }

        /// <summary>
        /// Stops receiving messages.
        /// </summary>
        public void StopReceiver()
        {
            server.Stop();
        }

        protected virtual void OnUpdate(object sender, UserEventArgs e)
        {
            Updating(sender, e);
        }
    }
}
