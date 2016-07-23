using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Services
{
    public interface IMasterService : IUserService
    {
        event EventHandler<UserEventArgs> Addition;
        event EventHandler<UserEventArgs> Removing;

        void Load();
        void Save();
    }
}
