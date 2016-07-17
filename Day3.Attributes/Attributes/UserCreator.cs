using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Attributes
{
    public class UserCreator
    {
        public List<User> CreateUsers()
        {
            List<User> users = new List<User>();
            var attrs = typeof(User).GetCustomAttributes<InstantiateUserAttribute>();

            if (attrs != null)
            {
                foreach (var attr in attrs)
                {
                    if (attr.Id == null)
                    {
                        attr.Id = MatchParameterWithProperty(typeof(User), "id");
                    }
                    users.Add(new User(attr.Id.Value) { FirstName = attr.FirstName, LastName = attr.LastName });
                }
            }
            return users;
        }

        public List<AdvancedUser> CreateAdvancedUsers()
        {
            List<AdvancedUser> users = new List<AdvancedUser>();
            var attrs = Assembly.GetExecutingAssembly().GetCustomAttributes<InstantiateAdvancedUserAttribute>();

            if (attrs != null)
            {
                foreach (var attr in attrs)
                {
                    if (attr.Id == null)
                    {
                        attr.Id = MatchParameterWithProperty(typeof(AdvancedUser), "id");
                    }
                    if (attr.ExternalId == null)
                    {
                        attr.ExternalId = MatchParameterWithProperty(typeof(AdvancedUser), "externalId");
                    }
                    users.Add(new AdvancedUser(attr.Id.Value, attr.ExternalId.Value) { FirstName = attr.FirstName, LastName = attr.LastName });
                }
            }
            return users;
        }

        private int MatchParameterWithProperty(Type type, string paramName)
        {
            string propertyName = type.GetConstructors()
                .FirstOrDefault(c => c.GetCustomAttributes<MatchParameterWithPropertyAttribute>() != null)
                ?.GetCustomAttributes<MatchParameterWithPropertyAttribute>()
                ?.FirstOrDefault(a => a.ParamName == paramName)
                ?.PropertyName;

            if (propertyName == null)
            {
                throw new InvalidOperationException("Unable to get property name.");
            }
            
            var defaultAttr = type.GetProperty(propertyName).GetCustomAttribute<DefaultValueAttribute>();

            if (defaultAttr == null)
            {
                throw new InvalidOperationException("Unable to get default property attribute.");
            }

            int defaultValue;

            if (!int.TryParse(defaultAttr.Value?.ToString(), out defaultValue))
            {
                throw new InvalidOperationException("Unable to get default property value.");
            }

            return defaultValue;
        }

    }
}
