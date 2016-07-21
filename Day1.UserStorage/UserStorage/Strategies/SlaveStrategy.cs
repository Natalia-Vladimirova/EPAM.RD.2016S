using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Strategies;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Strategies
{
    public class SlaveStrategy : IServiceStrategy
    {
        public IList<User> Users { get; }
        public StorageState StorageState
        {
            get
            {
                throw new AccessViolationException("Slave haven't got a state.");
            }
        }

        public SlaveStrategy(IMasterStrategy master)
        {
            if (master == null)
            {
                throw new ArgumentNullException($"{nameof(master)} must be not null.");
            }

            master.Addition += UpdateOnAdd;
            master.Removing += UpdateOnRemove;

            Users = master.Users;
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

        private void UpdateOnAdd(object sender, UserEventArgs eventArgs)
        {
            Users.Add(eventArgs.User);
        }

        private void UpdateOnRemove(object sender, UserEventArgs eventArgs)
        {
            Users.Remove(eventArgs.User);
        }

    }
}