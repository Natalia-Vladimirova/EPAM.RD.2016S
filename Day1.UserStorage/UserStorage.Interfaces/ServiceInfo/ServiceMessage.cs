using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    /// <summary>
    /// Used to contain service message.
    /// </summary>
    [Serializable]
    public class ServiceMessage
    {
        public ServiceMessage()
        {
        }

        /// <summary>
        /// Creates an instance of ServiceMessage with specified user and operation.
        /// </summary>
        /// <param name="user">
        /// User for transmission.
        /// </param>
        /// <param name="operation">
        /// Operation that indicates action to do with user.
        /// </param>
        public ServiceMessage(User user, ServiceOperation operation)
        {
            User = user;
            Operation = operation;
        }

        /// <summary>
        /// User for transmission.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Operation that indicates action to do with transmittable user.
        /// </summary>
        public ServiceOperation Operation { get; set; }
    }
}
