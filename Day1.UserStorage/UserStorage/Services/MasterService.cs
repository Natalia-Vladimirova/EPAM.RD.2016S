using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Services
{
    [Serializable]
    public class MasterService : MarshalByRefObject, IMasterService
    {
        private readonly IIdGenerator idGenerator;
        private readonly IUserLoader loader;
        private readonly IEnumerable<IValidator> validators;
        private readonly IEnumerable<ConnectionInfo> slavesInfo;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        public IList<User> Users { get; private set; }

        public MasterService(IIdGenerator idGenerator, IUserLoader loader, IEnumerable<IValidator> validators, IEnumerable<ConnectionInfo> slavesInfo)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }
            if (loader == null)
            {
                throw new ArgumentNullException($"{nameof(loader)} must be not null.");
            }
            if (slavesInfo == null)
            {
                throw new ArgumentNullException($"{nameof(slavesInfo)} must be not null.");
            }
            this.idGenerator = idGenerator;
            this.loader = loader;
            this.validators = validators;
            this.slavesInfo = slavesInfo;
        }

        public int Add(User user)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }

            if (validators?.Any(validator => !validator?.IsValid(user) ?? false) ?? false)
            {
                throw new ArgumentException($"{nameof(user)} is not valid.");
            }
            readerWriterLock.EnterWriteLock();
            try
            {
                idGenerator.GenerateNextId();
                user.PersonalId = idGenerator.CurrentId;
                Users.Add(user);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
            SendMessageToSlaves(new ServiceMessage(user, ServiceOperation.Addition));
            return user.PersonalId;
        }

        public void Delete(int personalId)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                User user = Users.FirstOrDefault(u => u.PersonalId == personalId);
                if (user != null)
                {
                    Users.Remove(user);
                    SendMessageToSlaves(new ServiceMessage(user, ServiceOperation.Removing));
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
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

        public void Load()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                var storageState = loader.Load();
                Users = storageState.Users ?? new List<User>();
                idGenerator.SetInitialValue(storageState.LastId);
                SendMessageToSlaves(Users);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void Save()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                var storageState = new StorageState
                {
                    LastId = idGenerator.CurrentId,
                    Users = Users.ToList()
                };
                loader.Save(storageState);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        private void SendMessageToSlaves<T>(T message)
        {
            var serializer = new JavaScriptSerializer();
            string serializedMessage = serializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(serializedMessage);

            foreach (var slave in slavesInfo)
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(slave.IPAddress, slave.Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }
        
    }
}
