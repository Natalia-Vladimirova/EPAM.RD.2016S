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
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly ILogService logger;
        private readonly IReceiver receiver;
        private readonly IList<User> users;

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
                users = loader.Load().Users ?? new List<User>();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tslave service created.");
        }

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

        public IList<int> Search(Func<User, bool> criteria)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                var foundIds = users.Where(criteria).Select(u => u.PersonalId).ToList();
                logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tusers search ({foundIds.Count} found).");
                return foundIds;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        public IList<User> GetAll()
        {
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tgetting all users.");
            return users;
        }

        public void ListenForUpdates()
        {
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tstart receiving messages.");
            receiver.StartReceivingMessages();
        }

        public void StopListen()
        {
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tstop receiving messages.");
            receiver.StopReceiver();
        }

        private void UpdateOnModifying(object sender, UserEventArgs eventArgs)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                switch (eventArgs.Operation)
                {
                    case ServiceOperation.Addition:
                        users.Add(eventArgs.User);
                        logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user added (id: {eventArgs.User.PersonalId}).");
                        break;
                    case ServiceOperation.Removing:
                        users.Remove(eventArgs.User);
                        logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\treceived user removed (id: {eventArgs.User.PersonalId}).");
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