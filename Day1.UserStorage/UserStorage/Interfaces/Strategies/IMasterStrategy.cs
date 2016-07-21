using System;
using System.Collections.Generic;
using System.Linq;
using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Strategies
{
    public interface IMasterStrategy : IServiceStrategy
    {
        event EventHandler<UserEventArgs> Addition;
        event EventHandler<UserEventArgs> Removing;
    }
}
