using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdGenerator
{
    public interface IIdGenerator
    {
        IEnumerator<int> GenerateId();
    }
}
