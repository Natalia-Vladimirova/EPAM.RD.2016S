namespace UserStorage.Interfaces.Generators
{
    public interface IIdGenerator
    {
        bool GenerateNextId();
        void SetInitialValue(int initialValue);
        int CurrentId { get; }
    }
}
