using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Strategies;

namespace UserStorage.Strategies
{
    public abstract class ServiceStrategy : IServiceStrategy
    {
        public IList<User> Users { get; set; } = new List<User>();

        public abstract int Add(User user, IIdGenerator idGenerator);
        public abstract void Delete(int personalId);

        public virtual IList<int> SearchForUser(Func<User, bool>[] criteria)
        {
            IEnumerable<User> foundUsers = Users;
            foreach (var cr in criteria)
            {
                foundUsers = foundUsers.Where(cr);
            }
            return foundUsers.Select(u => u.PersonalId).ToList();
        }

    }
}
