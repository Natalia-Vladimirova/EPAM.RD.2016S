using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    public interface IUserService
    {
        IList<User> Users { get; }
        int Add(User user);
        void Delete(int personalId);
        IList<int> SearchForUser(Func<User, bool>[] criteria);
    }
}
