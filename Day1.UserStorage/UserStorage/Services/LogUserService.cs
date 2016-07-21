using System;
using System.Collections.Generic;
using System.Diagnostics;
using UserStorage.Interfaces.Entities;
using UserStorage.Interfaces.ServiceInfo;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    public class LogUserService : IUserService
    {
        private readonly IUserService service;
        private readonly BooleanSwitch boolSwitch;
        private readonly TraceSource source;

        public IList<User> Users => service.Users;
        public StorageState StorageState => service.StorageState;

        public LogUserService(IUserService service)
        {
            if (service == null)
            {
                if (boolSwitch.Enabled)
                    source.TraceEvent(TraceEventType.Error, 0, "User service is null!");
                throw new ArgumentNullException(nameof(service));
            }
            boolSwitch = new BooleanSwitch("Switch", "");
            source = new TraceSource("Source");
            this.service = service;
        }

        public int Add(User user)
        {
            if (boolSwitch.Enabled)
                source.TraceEvent(TraceEventType.Information, 0, "Add!");
            return service.Add(user);
        }

        public void Delete(int personalId)
        {
            if (boolSwitch.Enabled)
                source.TraceEvent(TraceEventType.Information, 0, "Delete!");
            service.Delete(personalId);
        }

        public IList<int> SearchForUser(Func<User, bool>[] criteria)
        {
            if (boolSwitch.Enabled)
                source.TraceEvent(TraceEventType.Information, 0, "Search!");
            return service.SearchForUser(criteria);
        }
    }
}
