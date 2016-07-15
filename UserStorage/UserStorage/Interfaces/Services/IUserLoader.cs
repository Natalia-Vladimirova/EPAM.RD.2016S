using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorage.Interfaces.Entities;

namespace UserStorage.Interfaces.Services
{
    public interface IUserLoader
    {
        StorageState Load();
        void Save(StorageState state);
    }
}
