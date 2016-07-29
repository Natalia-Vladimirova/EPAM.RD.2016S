using UserStorage.Interfaces.ServiceInfo;

namespace UserStorage.Interfaces.Loaders
{
    public interface IUserLoader
    {
        StorageState Load();

        void Save(StorageState state);
    }
}
