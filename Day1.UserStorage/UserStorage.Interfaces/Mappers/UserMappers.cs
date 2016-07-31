using System.Linq;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.XmlEntities;

namespace UserStorage.Interfaces.Mappers
{
    public static class UserMappers
    {
        public static User ToUser(this XmlUser xmlUser)
        {
            if (xmlUser == null)
            {
                return null;
            }

            return new User
            {
                PersonalId = xmlUser.PersonalId,
                FirstName = xmlUser.FirstName,
                LastName = xmlUser.LastName,
                DateOfBirth = xmlUser.DateOfBirth,
                Gender = xmlUser.Gender,
                Visas = xmlUser.Visas.Select(v => v.ToVisa()).ToList()
            };
        }

        public static XmlUser ToXmlUser(this User user)
        {
            if (user == null)
            {
                return null;
            }

            return new XmlUser
            {
                PersonalId = user.PersonalId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Visas = user.Visas.Select(v => v.ToXmlVisa()).ToList()
            };
        }
    }
}
