namespace IdGenerator
{
    public interface IIdGenerator
    {
        bool GenerateNextId();
        void SetInitialValue(int initialValue);
        int CurrentId { get; }
    }
}
