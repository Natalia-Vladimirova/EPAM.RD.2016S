namespace UserStorage.Interfaces.Services
{
    public interface ISlaveService : IUserService
    {
        void ListenForUpdates();
    }
}
