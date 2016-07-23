using System;
using System.Collections.Generic;

namespace UserStorage.Interfaces.Entities
{
    [Serializable]
    public class User
    {
        public int PersonalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public List<Visa> Visas { get; set; }

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

            if (PersonalId == user.PersonalId && FirstName == user.FirstName && LastName == user.LastName)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = PersonalId.GetHashCode();
            if (FirstName != null)
            {
                hashCode ^= FirstName.GetHashCode();
            }
            if (LastName != null)
            {
                hashCode ^= LastName.GetHashCode();
            }
            return hashCode ^ DateOfBirth.GetHashCode() ^ Gender.GetHashCode();
        }
        
    }
}
