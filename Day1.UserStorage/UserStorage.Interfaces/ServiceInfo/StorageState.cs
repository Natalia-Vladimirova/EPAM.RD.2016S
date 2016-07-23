using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.ServiceInfo
{
    public class StorageState
    {
        public int LastId { get; set; }
        public List<User> Users { get; set; }
    }
}
