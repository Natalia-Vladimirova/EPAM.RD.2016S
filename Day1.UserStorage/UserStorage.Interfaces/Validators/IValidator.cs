using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Validators
{
    /// <summary>
    /// Used to validate user.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Check that user is valid.
        /// </summary>
        /// <param name="user">
        /// User to check.
        /// </param>
        /// <returns>
        /// Whether user is valid.
        /// </returns>
        bool IsValid(User user);
    }
}
