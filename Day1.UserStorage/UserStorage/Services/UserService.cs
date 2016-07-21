using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Strategies;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceStrategy serviceStrategy;

        public IList<User> Users => serviceStrategy.Users;
        public StorageState StorageState => serviceStrategy.StorageState;

        public UserService(IServiceStrategy serviceStrategy)
        {
            if (serviceStrategy == null)
            {
                throw new ArgumentNullException($"{nameof(serviceStrategy)} must be not null.");
            }
            this.serviceStrategy = serviceStrategy;
        }

        public int Add(User user)
        {
            return serviceStrategy.Add(user);
        }

        public void Delete(int personalId)
        {
            serviceStrategy.Delete(personalId);
        }

        public IList<int> SearchForUser(Func<User, bool>[] criteria)
        {
            return serviceStrategy.SearchForUser(criteria);
        }
        
    }
}
