using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    [Serializable]
    public class ServiceMessage
    {
        public User User { get; set; }
        public ServiceOperation Operation { get; set; }

        public ServiceMessage() { }

        public ServiceMessage(User user, ServiceOperation operation)
        {
            User = user;
            Operation = operation;
        }
    }
}
