namespace UserStorage.Interfaces.Generators
{
    public interface IIdGenerator
    {
        int CurrentId { get; }

        bool GenerateNextId();

        void SetInitialValue(int initialValue);
    }
}
