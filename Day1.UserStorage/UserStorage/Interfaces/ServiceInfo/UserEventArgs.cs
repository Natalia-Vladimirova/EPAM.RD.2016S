using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    public class UserEventArgs : EventArgs
    {
        public User User { get; }

        public UserEventArgs(User user)
        {
            User = user;
        }
    }
}
