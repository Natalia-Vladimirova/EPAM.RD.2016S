using System.Collections.Generic;

namespace IdGenerator
{
    public interface IIdGenerator
    {
        bool GenerateNextId();
        int CurrentId { get; }
    }
}
