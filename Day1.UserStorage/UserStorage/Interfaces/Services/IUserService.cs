using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Services
{
    public interface IUserService
    {
        IList<User> Users { get; }
        StorageState StorageState { get; }
        int Add(User user);
        void Delete(int personalId);
        IList<int> SearchForUser(Func<User, bool>[] criteria);
    }
}
