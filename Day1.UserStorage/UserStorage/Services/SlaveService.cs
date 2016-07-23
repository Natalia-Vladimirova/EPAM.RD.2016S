using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    [Serializable]
    public class SlaveService : MarshalByRefObject, ISlaveService
    {
        private TcpListener server = null;
        public IList<User> Users { get; }

        public SlaveService(IMasterService master, ConnectionInfo info)
        {
            //if (master == null)
            //{
            //    throw new ArgumentNullException($"{nameof(master)} must be not null.");
            //}

            if (info?.IPAddress == null)
            {
                throw new ArgumentException($"{nameof(info)} is invalid.");
            }

            server = new TcpListener(info.IPAddress, info.Port);
            server.Start();
            
            Users = master?.Users ?? new List<User>();
        }

        public int Add(User user)
        {
            throw new AccessViolationException("Slave cannot write to storage.");
        }

        public void Delete(int personalId)
        {
            throw new AccessViolationException("Slave cannot delete from storage.");
        }

        public IList<int> SearchForUser(Func<User, bool>[] criteria)
        {
            IEnumerable<User> foundUsers = Users;
            foreach (var cr in criteria)
            {
                foundUsers = foundUsers.Where(cr);
            }
            return foundUsers.Select(u => u.PersonalId).ToList();
        }

        async public void ListenForUpdates()
        {
            try
            {
                while (true)
                {
                    using (TcpClient client = await server.AcceptTcpClientAsync())
                    using (NetworkStream stream = client.GetStream())
                    {
                        Console.Write($"{AppDomain.CurrentDomain.FriendlyName} connected!");

                        string serializedMessage = string.Empty;
                        byte[] data = new byte[1024];

                        while (stream.DataAvailable)
                        {
                            int i = await stream.ReadAsync(data, 0, data.Length);
                            serializedMessage += Encoding.UTF8.GetString(data, 0, i);
                        }

                        ServiceMessage message;
                        try
                        {
                            var serializer = new JavaScriptSerializer();
                            message = serializer.Deserialize<ServiceMessage>(serializedMessage);
                        }
                        catch
                        {
                            throw new InvalidOperationException("Unable to deserialize request.");
                        }
                        UpdateOnModifying(message);
                    }
                }
            }
            finally
            {
                server.Stop();
            }
        }

        private void UpdateOnModifying(ServiceMessage message)
        {
            switch (message.Operation)
            {
                case ServiceOperation.Addition:
                    Users.Add(message.User);
                    break;
                case ServiceOperation.Removing:
                    Users.Remove(message.User);
                    break;
            }
        }
        
    }
}