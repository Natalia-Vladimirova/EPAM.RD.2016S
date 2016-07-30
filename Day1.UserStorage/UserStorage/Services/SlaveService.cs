using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UserStorage.Interfaces.Creators;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    [Serializable]
    public class SlaveService : MarshalByRefObject, IUserService, IListener
    {
        private readonly ILogService logger;
        private readonly IReceiver receiver;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        public SlaveService(IDependencyCreator creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException($"{nameof(creator)} must be not null.");
            }

            logger = creator.CreateInstance<ILogService>();
            if (logger == null)
            {
                throw new InvalidOperationException($"Unable to create {nameof(logger)}.");
            }
            
            var loader = creator.CreateInstance<IUserLoader>();
            if (loader == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\t{nameof(loader)} is null.");
                throw new InvalidOperationException($"Unable to create {nameof(loader)}.");
            }

            receiver = creator.CreateInstance<IReceiver>();
            if (receiver == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\t{nameof(receiver)} is null.");
                throw new InvalidOperationException($"Unable to create {nameof(receiver)}.");
            }

            receiver.Updating += UpdateOnModifying;

            readerWriterLock.EnterWriteLock();
            try
            {
                Users = loader.Load().Users ?? new List<User>();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tslave service created.");
        }

        public IList<User> Users { get; }

        public int Add(User user)
        {
            receiver.StopReceiver();
            logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\taddition attempt; access denied.");
            throw new AccessViolationException("Slave cannot write to storage.");
        }

        public void Delete(int personalId)
        {
            receiver.StopReceiver();
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
        
        public void ListenForUpdates()
        {
            receiver.StartReceivingMessages();
        }

        private void UpdateOnModifying(object sender, UserEventArgs eventArgs)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                switch (eventArgs.Operation)
                {
                    case ServiceOperation.Addition:
                        Users.Add(eventArgs.User);
                        logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user added.");
                        break;
                    case ServiceOperation.Removing:
                        Users.Remove(eventArgs.User);
                        logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user removed.");
                        break;
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}