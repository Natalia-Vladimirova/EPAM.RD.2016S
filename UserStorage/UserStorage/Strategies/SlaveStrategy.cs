using System;
using IdGenerator;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Strategies
{
    public class SlaveStrategy : ServiceStrategy
    {
        public SlaveStrategy(MasterStrategy master)
        {
            master.Addition += UpdateOnAdd;
            master.Addition += UpdateOnRemove;
        }

        public override int Add(User user, IIdGenerator idGenerator)
        {
            throw new AccessViolationException("Slave cannot write to storage.");
        }

        public override void Delete(int personalId)
        {
            throw new AccessViolationException("Slave cannot delete from storage.");
        }

        private void UpdateOnAdd(object sender, UserEventArgs eventArgs)
        {
            Users.Add(eventArgs.User);
        }

        private void UpdateOnRemove(object sender, UserEventArgs eventArgs)
        {
            Users.Remove(eventArgs.User);
        }

    }
}