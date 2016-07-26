using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    [Serializable]
    public class SlaveService : MarshalByRefObject, ISlaveService
    {
        private readonly TcpListener server;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly ILogService logger;

        public IList<User> Users { get; }

        public SlaveService(ConnectionInfo info, IUserLoader loader, ILogService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException($"{nameof(logger)} must be not null.");
            }
            this.logger = logger;
            if (info == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\tnull argument {nameof(info)}.");
                throw new ArgumentNullException($"{nameof(info)} must be not null.");
            }
            if (loader == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\tnull argument {nameof(loader)}.");
                throw new ArgumentNullException($"{nameof(loader)} must be not null.");
            }
            readerWriterLock.EnterWriteLock();
            try
            {
                Users = loader.Load().Users ?? new List<User>();
                server = new TcpListener(info.IPAddress, info.Port);
                server.Start();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tslave service created.");
        }

        public int Add(User user)
        {
            server.Stop();
            logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\taddition attempt; access denied.");
            throw new AccessViolationException("Slave cannot write to storage.");
        }

        public void Delete(int personalId)
        {
            server.Stop();
            logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\tremoving attempt; access denied.");
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
                var foundIds = foundUsers.Select(u => u.PersonalId).ToList();
                logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tusers search ({foundIds.Count} found).");
                return foundIds;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }
        
        public async void ListenForUpdates()
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                while (true)
                {
                    using (TcpClient client = await server.AcceptTcpClientAsync())
                    using (NetworkStream stream = client.GetStream())
                    {
                        logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tusers update received.");

                        string serializedMessage = string.Empty;
                        byte[] data = new byte[1024];

                        while (stream.DataAvailable)
                        {
                            int i = await stream.ReadAsync(data, 0, data.Length);
                            serializedMessage += Encoding.UTF8.GetString(data, 0, i);
                        }

                        readerWriterLock.EnterWriteLock();
                        try
                        {
                            ServiceMessage message = serializer.Deserialize<ServiceMessage>(serializedMessage);
                            UpdateOnModifying(message);
                        }
                        catch
                        {
                            logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\tunable to deserialize request.");
                            throw new InvalidOperationException("Unable to deserialize request.");
                        }
                        finally
                        {
                            readerWriterLock.ExitWriteLock();
                        }
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
                    logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user added.");
                    break;
                case ServiceOperation.Removing:
                    Users.Remove(message.User);
                    logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user removed.");
                    break;
            }
        }
        
    }
}