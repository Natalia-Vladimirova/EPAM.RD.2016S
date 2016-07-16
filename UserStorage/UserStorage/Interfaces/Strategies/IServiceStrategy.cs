using System;
using System.Collections.Generic;
using IdGenerator;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Strategies
{
    public interface IServiceStrategy
    {
        IList<User> Users { get; set; }
        int Add(User user, IIdGenerator idGenerator);
        void Delete(int personalId);
        IList<int> SearchForUser(Func<User, bool>[] criteria);
    }
}
