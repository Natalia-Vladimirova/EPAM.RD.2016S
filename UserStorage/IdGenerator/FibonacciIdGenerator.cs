using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdGenerator
{
    public class FibonacciIdGenerator : IIdGenerator
    {
        public IEnumerator<int> GenerateId()
        {
            int previous = 0;
            int current = 1;

            while (true)
            {
                int next = previous + current;
                previous = current;
                current = next;
                yield return current;
            }
        }
    }
}
