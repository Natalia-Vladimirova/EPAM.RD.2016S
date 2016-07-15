using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage.Interfaces.Entities
{
    public class User
    {
        public int PersonalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public List<Visa> Visa { get; set; }

        public override bool Equals(object obj)
        {
            User user = obj as User;
            if (user == null)
            {
                return false;
            }

            if (ReferenceEquals(this, user))
            {
                return true;
            }

            if (PersonalId == user.PersonalId)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return PersonalId ^ FirstName.GetHashCode() ^ LastName.GetHashCode() ^ DateOfBirth.GetHashCode() ^ Gender.GetHashCode();
        }

    }
}
