using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Loaders;
using UserStorage.Interfaces.Strategies;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Strategies
{
    public class MasterStrategy : IMasterStrategy
    {
        private readonly IIdGenerator idGenerator;
        private readonly IEnumerable<Func<User, bool>> validates = new List<Func<User, bool>>();

        public event EventHandler<UserEventArgs> Addition = delegate { };
        public event EventHandler<UserEventArgs> Removing = delegate { };

        public IList<User> Users { get; }
        public StorageState StorageState
        {
            get
            {
                return new StorageState
                {
                    LastId = idGenerator.CurrentId,
                    Users = Users.ToList()
                };
            }
        }

        public MasterStrategy(IIdGenerator idGenerator, IList<User> users)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }
            this.idGenerator = idGenerator;
            Users = users ?? new List<User>();
        }

        public MasterStrategy(IIdGenerator idGenerator, IList<User> users, IEnumerable<Func<User, bool>> validates)
            : this(idGenerator, users)
        {
            if (validates == null)
            {
                throw new ArgumentNullException($"{nameof(validates)} must be not null.");
            }
            this.validates = validates;
        }

        public int Add(User user)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }

            if (validates.Any(validate => !validate(user)))
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
        
    }
}
