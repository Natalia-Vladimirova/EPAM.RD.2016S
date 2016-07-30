using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    public class UserEventArgs : EventArgs
    {
        public UserEventArgs(User user, ServiceOperation operation)
        {
            User = user;
            Operation = operation;
        }

        public User User { get; }

        public ServiceOperation Operation { get; }
    }
}
