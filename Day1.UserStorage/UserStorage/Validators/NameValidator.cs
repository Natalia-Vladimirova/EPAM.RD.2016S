using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Validators
{
    /// <summary>
    /// Used to check first name of user.
    /// </summary>
    public class NameValidator : IValidator
    {
        /// <summary>
        /// Verifies that first name of user is not null and its length is more than one.
        /// </summary>
        /// <param name="user">
        /// User to verify.
        /// </param>
        /// <returns>
        /// Whether user is valid.
        /// </returns>
        public bool IsValid(User user)
        {
            return user?.FirstName != null && user.FirstName.Length > 1;
        }
    }
}
