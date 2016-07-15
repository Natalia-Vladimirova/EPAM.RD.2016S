using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciIdGenerator
{
    public interface IIdGenerator
    {
        IEnumerator<int> GenerateId();
    }
}
