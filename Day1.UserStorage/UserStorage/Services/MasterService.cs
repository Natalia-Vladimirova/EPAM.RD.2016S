using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UserStorage.Interfaces.Creators;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Generators;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Network;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Services
{
    /// <summary>
    /// Master user service that can add, delete, search users and send messages to slave services.
    /// </summary>
    [Serializable]
    public class MasterService : MarshalByRefObject, IUserService, IServiceLoader
    {
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IIdGenerator idGenerator;
        private readonly IUserLoader loader;
        private readonly ILogService logger;
        private readonly ISender sender;
        private readonly IEnumerable<IValidator> validators;
        private IList<User> users;

        /// <summary>
        /// Creates an instance of MasterService with necessary dependency creator.
        /// </summary>
        /// <param name="creator">
        /// Creator that contains information about types of logger, id generator, loader, sender and validators.
        /// </param>
        public MasterService(IDependencyCreator creator)
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

            idGenerator = creator.CreateInstance<IIdGenerator>();
            if (idGenerator == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\t{nameof(idGenerator)} is null.");
                throw new InvalidOperationException($"Unable to create {nameof(idGenerator)}.");
            }

            loader = creator.CreateInstance<IUserLoader>();
            if (loader == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\t{nameof(loader)} is null.");
                throw new InvalidOperationException($"Unable to create {nameof(loader)}.");
            }

            sender = creator.CreateInstance<ISender>();
            if (sender == null)
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\t{nameof(sender)} is null.");
                throw new InvalidOperationException($"Unable to create {nameof(sender)}.");
            }

            validators = creator.CreateListOfInstances<IValidator>() ?? new List<IValidator>();
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tmaster service created.");
        }

        /// <summary>
        /// Adds user to local service collection.
        /// </summary>
        /// <param name="user">
        /// User to add.
        /// </param>
        /// <returns>
        /// Id of added user.
        /// </returns>
        public int Add(User user)
        {
            if (validators.Any(validator => !validator?.IsValid(user) ?? false))
            {
                logger.Log(TraceEventType.Error, $"{AppDomain.CurrentDomain.FriendlyName}:\tattempted addition of invalid user.");
                throw new ArgumentException($"{nameof(user)} is not valid.");
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                idGenerator.GenerateNextId();
                user.PersonalId = idGenerator.CurrentId;
                users.Add(user);
                logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tuser added (id: {idGenerator.CurrentId}).");
                sender.SendMessage(new ServiceMessage(user, ServiceOperation.Addition));
                return user.PersonalId;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Deletes user from local service collection.
        /// </summary>
        /// <param name="personalId">
        /// Id of user to delete.
        /// </param>
        public void Delete(int personalId)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                User user = users.FirstOrDefault(u => u.PersonalId == personalId);
                if (user != null)
                {
                    users.Remove(user);
                    logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tuser removed (id: {user.PersonalId}).");
                    sender.SendMessage(new ServiceMessage(user, ServiceOperation.Removing));
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Searches user in local service collection by criteria.
        /// </summary>
        /// <param name="criteria">
        /// Search criteria.
        /// </param>
        /// <returns>
        /// List of id of found users.
        /// </returns>
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

        /// <summary>
        /// Gets all users from local service collection.
        /// </summary>
        /// <returns>
        /// List of all users.
        /// </returns>
        public IList<User> GetAll()
        {
            logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tgetting all users.");
            return users;
        }

        /// <summary>
        /// Loads collection of users to local service collection using service loader.
        /// </summary>
        public void Load()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                var storageState = loader.Load();
                users = storageState.Users ?? new List<User>();
                idGenerator.SetInitialValue(storageState.LastId);
                logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tservice state loaded.");
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Saves local service collection of users using service loader.
        /// </summary>
        public void Save()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                var storageState = new StorageState
                {
                    LastId = idGenerator.CurrentId,
                    Users = users.ToList()
                };
                loader.Save(storageState);
                logger.Log(TraceEventType.Information, $"{AppDomain.CurrentDomain.FriendlyName}:\tservice state saved.");
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}
