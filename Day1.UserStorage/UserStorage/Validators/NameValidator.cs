using System;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Validators
{
    [Serializable]
    public class NameValidator : IValidator
    {
        public bool IsValid(User user)
        {
            return user?.FirstName != null && user.FirstName.Length > 1;
        }
    }
}
