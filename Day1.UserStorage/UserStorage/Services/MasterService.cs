using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Services
{
    public class MasterService : IMasterService
    {
        private readonly IIdGenerator idGenerator;
        private readonly IEnumerable<IValidator> validators;
        private readonly IUserLoader loader;

        public event EventHandler<UserEventArgs> Addition = delegate { };
        public event EventHandler<UserEventArgs> Removing = delegate { };

        public IList<User> Users { get; private set; }

        public MasterService(IIdGenerator idGenerator, IUserLoader loader, IEnumerable<IValidator> validators)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }
            this.idGenerator = idGenerator;

            if (loader == null)
            {
                throw new ArgumentNullException($"{nameof(loader)} must be not null.");
            }
            this.loader = loader;
            this.validators = validators;
        }
        
        public int Add(User user)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }

            if (validators?.Any(validator => !validator.IsValid(user)) ?? false)
            {
                throw new ArgumentException($"{nameof(user)} is not valid.");
            }
            idGenerator.GenerateNextId();
            user.PersonalId = idGenerator.CurrentId;
            Users.Add(user);
            OnAdd(this, new UserEventArgs(user));
            return user.PersonalId;
        }

        public void Delete(int personalId)
        {
            User userToRemove = Users.FirstOrDefault(u => u.PersonalId == personalId);
            if (userToRemove != null)
            {
                Users.Remove(userToRemove);
                OnDelete(this, new UserEventArgs(userToRemove));
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

        protected virtual void OnAdd(object sender, UserEventArgs e)
        {
            Addition(sender, e);
        }

        protected virtual void OnDelete(object sender, UserEventArgs e)
        {
            Removing(sender, e);
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
    }
}
