namespace UserStorage.Interfaces.Services
{
    public interface ISlaveService : IUserService
    {
        void InitializeUsers();
        void ListenForUpdates();
    }
}
