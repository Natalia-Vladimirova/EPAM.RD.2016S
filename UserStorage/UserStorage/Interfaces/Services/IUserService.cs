using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    public interface IUserService
    {
        void Add(User user);
        void Delete(int personalId);
        IEnumerable<int> SearchForUser(Func<User, bool>[] criteria);
    }
}
