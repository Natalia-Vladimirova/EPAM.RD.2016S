using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    /// <summary>
    /// Used to contain user and service operation.
    /// </summary>
    public class UserEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of UserEventArgs with specified user and operation.
        /// </summary>
        /// <param name="user">
        /// User for updating.
        /// </param>
        /// <param name="operation">
        /// Operation that indicates action to do with user.
        /// </param>
        public UserEventArgs(User user, ServiceOperation operation)
        {
            User = user;
            Operation = operation;
        }

        /// <summary>
        /// User for updating.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Operation that indicates action to do with updated user.
        /// </summary>
        public ServiceOperation Operation { get; }
    }
}
