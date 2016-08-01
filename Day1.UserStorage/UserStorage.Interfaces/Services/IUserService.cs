using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    public interface IUserService
    {
        int Add(User user);

        void Delete(int personalId);

        IList<int> Search(Func<User, bool> criteria);

        IList<User> GetAll();
    }
}
