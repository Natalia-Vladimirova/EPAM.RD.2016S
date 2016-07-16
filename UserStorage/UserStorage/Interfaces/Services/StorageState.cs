using System;
using System.Collections.Generic;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    public class StorageState
    {
        public int LastId { get; set; }
        public List<User> Users { get; set; } 
    }
}
