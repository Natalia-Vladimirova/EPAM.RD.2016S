using System.Collections.Generic;

namespace IdGenerator
{
    public class FibonacciIdGenerator : IIdGenerator
    {
        private readonly IEnumerator<int> idGenerator;

        public int CurrentId => idGenerator.Current;

        public FibonacciIdGenerator()
        {
            idGenerator = GenerateId();
        }

        public bool GenerateNextId()
        {
            return idGenerator.MoveNext();
        }

        public void SetInitialValue(int initialValue)
        {
            if (initialValue > 0)
            {
                do
                {
                    idGenerator.MoveNext();
                } while (idGenerator.Current < initialValue);
            }
        }

        private IEnumerator<int> GenerateId()
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
