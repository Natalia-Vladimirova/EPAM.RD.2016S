using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;
using System.Net.Sockets;

namespace UserStorage.Services
{
    [Serializable]
    public class MasterService : MarshalByRefObject, IMasterService
    {
        private readonly IIdGenerator idGenerator;
        private readonly IUserLoader loader;
        private readonly IEnumerable<IValidator> validators;
        private readonly IEnumerable<ConnectionInfo> slavesInfo;

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
            idGenerator.GenerateNextId();
            user.PersonalId = idGenerator.CurrentId;
            Users.Add(user);
            NotifyAboutChanges(new ServiceMessage(user, ServiceOperation.Addition));
            return user.PersonalId;
        }

        public void Delete(int personalId)
        {
            User user = Users.FirstOrDefault(u => u.PersonalId == personalId);
            if (user != null)
            {
                Users.Remove(user);
                NotifyAboutChanges(new ServiceMessage(user, ServiceOperation.Removing));
            }
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

        public void Load()
        {
            var storageState = loader.Load();
            Users = storageState.Users ?? new List<User>();
            idGenerator.SetInitialValue(storageState.LastId);
        }

        public void Save()
        {
            var storageState = new StorageState
            {
                LastId = idGenerator.CurrentId,
                Users = Users.ToList()
            };
            loader.Save(storageState);
        }

        private void NotifyAboutChanges(ServiceMessage message)
        {
            var serializer = new JavaScriptSerializer();
            foreach (var slave in slavesInfo)
            {
                using (TcpClient client = new TcpClient())
                {

                    string serializedMessage;
                    try
                    {
                        serializedMessage = serializer.Serialize(message);
                    }
                    catch
                    {
                        throw new InvalidOperationException("Unable to deserialize request.");
                    }

                    byte[] data = Encoding.UTF8.GetBytes(serializedMessage);

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
