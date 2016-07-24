using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    [Serializable]
    public class SlaveService : MarshalByRefObject, ISlaveService
    {
        private readonly TcpListener server = null;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        public IList<User> Users { get; private set; }

        public SlaveService(ConnectionInfo info)
        {
            if (info?.IPAddress == null)
            {
                throw new ArgumentException($"{nameof(info)} is invalid.");
            }
            server = new TcpListener(info.IPAddress, info.Port);
            server.Start();
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
            readerWriterLock.EnterReadLock();
            try
            {
                IEnumerable<User> foundUsers = Users;
                foreach (var cr in criteria)
                {
                    foundUsers = foundUsers.Where(cr);
                }
                return foundUsers.Select(u => u.PersonalId).ToList();
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        async public void InitializeUsers()
        {
            try
            {
                using (TcpClient client = await server.AcceptTcpClientAsync())
                using (NetworkStream stream = client.GetStream())
                {
                    Console.WriteLine($"Initializing {AppDomain.CurrentDomain.FriendlyName}");

                    string serializedMessage = string.Empty;
                    byte[] data = new byte[1024];

                    while (stream.DataAvailable)
                    {
                        int i = await stream.ReadAsync(data, 0, data.Length);
                        serializedMessage += Encoding.UTF8.GetString(data, 0, i);
                    }

                    var serializer = new JavaScriptSerializer();

                    readerWriterLock.EnterWriteLock();
                    try
                    {
                        Users = serializer.Deserialize<List<User>>(serializedMessage) ?? new List<User>();
                    }
                    catch
                    {
                        throw new InvalidOperationException("Unable to deserialize request.");
                    }
                    finally
                    {
                        readerWriterLock.ExitWriteLock();
                    }
                }
            }
            catch
            {
                server.Stop();
            }
        }

        async public void ListenForUpdates()
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                while (true)
                {
                    using (TcpClient client = await server.AcceptTcpClientAsync())
                    using (NetworkStream stream = client.GetStream())
                    {
                        Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} connected!");

                        string serializedMessage = string.Empty;
                        byte[] data = new byte[1024];

                        while (stream.DataAvailable)
                        {
                            int i = await stream.ReadAsync(data, 0, data.Length);
                            serializedMessage += Encoding.UTF8.GetString(data, 0, i);
                        }

                        ServiceMessage message;
                        readerWriterLock.EnterWriteLock();
                        try
                        {
                            message = serializer.Deserialize<ServiceMessage>(serializedMessage);
                            UpdateOnModifying(message);
                        }
                        catch
                        {
                            throw new InvalidOperationException("Unable to deserialize request.");
                        }
                        finally
                        {
                            readerWriterLock.ExitWriteLock();
                        }
                    }
                }
            }
            catch
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