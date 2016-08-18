using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    /// <summary>
    /// Represents user service that can add, delete and search users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds user to local repository.
        /// </summary>
        /// <param name="user">
        /// User to add.
        /// </param>
        /// <returns>
        /// Id of added user.
        /// </returns>
        int Add(User user);

        /// <summary>
        /// Deletes user from local repository.
        /// </summary>
        /// <param name="personalId">
        /// Id of user to delete.
        /// </param>
        void Delete(int personalId);

        /// <summary>
        /// Searches user in local repository by criteria.
        /// </summary>
        /// <param name="criteria">
        /// Search criteria.
        /// </param>
        /// <returns>
        /// List of id of found users.
        /// </returns>
        IList<int> Search(Func<User, bool> criteria);

        /// <summary>
        /// Gets all users from local repository.
        /// </summary>
        /// <returns>
        /// List of all users.
        /// </returns>
        IList<User> GetAll();
    }
}
