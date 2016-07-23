using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.Validators;

namespace UserStorage.Validators
{
    class NameValidator : IValidator
    {
        public bool IsValid(User user)
        {
            return user?.FirstName != null && user.FirstName.Length > 1;
        }
    }
}
