using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UserStorage.Interfaces.Entities
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int PersonalId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
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
