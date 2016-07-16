using System;
using System.Collections.Generic;
using System.Linq;
using IdGenerator;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Strategies
{
    public class MasterStrategy : ServiceStrategy
    {
        private readonly IEnumerable<Func<User, bool>> validates;

        public event EventHandler<UserEventArgs> Addition = delegate { };
        public event EventHandler<UserEventArgs> Removing = delegate { };

        public MasterStrategy()
        {
            validates = new List<Func<User, bool>>();
        }

        public MasterStrategy(IEnumerable<Func<User, bool>> validates)
        {
            if (validates == null)
            {
                throw new ArgumentNullException($"{nameof(validates)} must be not null.");
            }

            this.validates = validates;
        }

        public override int Add(User user, IIdGenerator idGenerator)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException($"{nameof(idGenerator)} must be not null.");
            }

            if (validates.Any(validate => !validate(user)))
            {
                throw new ArgumentException($"{nameof(user)} is not valid.");
            }
            idGenerator.GenerateId().MoveNext();
            user.PersonalId = idGenerator.GenerateId().Current;
            Users.Add(user);
            OnAdd(this, new UserEventArgs(user));
            return user.PersonalId;
        }

        public override void Delete(int personalId)
        {
            User userToRemove = Users.FirstOrDefault(u => u.PersonalId == personalId);
            if (userToRemove != null)
            {
                Users.Remove(userToRemove);
                OnDelete(this, new UserEventArgs(userToRemove));
            }
        }

        protected virtual void OnAdd(object sender, UserEventArgs e)
        {
            Addition(sender, e);
        }

        protected virtual void OnDelete(object sender, UserEventArgs e)
        {
            Removing(sender, e);
        }

    }
}
