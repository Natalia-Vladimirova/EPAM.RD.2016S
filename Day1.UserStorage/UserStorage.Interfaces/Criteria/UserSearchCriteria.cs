using System;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Criteria
{
    public static class UserSearchCriteria
    {
        public static bool Compare(this User user, User criteria)
        {
            if (user == null)
            {
                return false;
            }

            if (criteria == null)
            {
                return true;
            }

            bool isSame = true;

            if (criteria.PersonalId != default(int))
            {
                isSame &= user.PersonalId == criteria.PersonalId;
            }

            if (criteria.FirstName != null)
            {
                isSame &= user.FirstName == criteria.FirstName;
            }

            if (criteria.LastName != null)
            {
                isSame &= user.LastName == criteria.LastName;
            }

            if (criteria.DateOfBirth != default(DateTime))
            {
                isSame &= user.DateOfBirth == criteria.DateOfBirth;
            }

            return isSame;
        }
    }
}
