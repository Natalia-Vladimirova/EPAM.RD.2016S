using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Validators
{
    public interface IValidator
    {
        bool IsValid(User user);
    }
}
