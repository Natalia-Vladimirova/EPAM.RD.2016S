using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Services;
using UserStorage.Interfaces.Strategies;

namespace UserStorage.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceStrategy serviceStrategy;
        private readonly IIdGenerator idGenerator;
        private readonly IUserLoader loader;

        public UserService(IServiceStrategy serviceStrategy, IIdGenerator idGenerator, IUserLoader loader)
        {
            if (serviceStrategy == null)
            {
                throw new ArgumentNullException($"{nameof(serviceStrategy)} must be not null.");
            }

            if (loader == null)
            {
                throw new ArgumentNullException($"{nameof(loader)} must be not null.");
            }

            this.serviceStrategy = serviceStrategy;
            this.idGenerator = idGenerator;
            this.loader = loader;
        }

        public void Add(User user)
        {
            serviceStrategy.Add(user, idGenerator);
        }

        public void Delete(int personalId)
        {
            serviceStrategy.Delete(personalId);
        }

        public IEnumerable<int> SearchForUser(Func<User, bool>[] criteria)
        {
            return serviceStrategy.SearchForUser(criteria);
        }

        public void Load()
        {
            StorageState state = loader.Load();
            serviceStrategy.Users = state.Users.ToList();

            if (state.LastId > 0)
            {
                do
                {
                    idGenerator.GenerateId().MoveNext();
                } while (idGenerator.GenerateId().Current < state.LastId);
            }
        }

        public void Save()
        {
            StorageState state = new StorageState
            {
                LastId = idGenerator.GenerateId().Current,
                Users = serviceStrategy.Users
            };
            loader.Save(state);
        }

    }
}
