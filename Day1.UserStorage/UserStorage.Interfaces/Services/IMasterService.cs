namespace UserStorage.Interfaces.Services
{
    public interface IMasterService : IUserService
    {
        void Load();

        void Save();
    }
}
