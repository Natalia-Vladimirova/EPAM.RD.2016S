using System;
using System.Collections;
using System.Collections.Generic;

namespace IdGenerator
{
    [Serializable]
    public class FibonacciIdGenerator : IIdGenerator
    {
        private readonly IEnumerator<int> idGenerator;

        public int CurrentId => idGenerator.Current;

        public FibonacciIdGenerator()
        {
            idGenerator = new FibonacciIterator();
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

        [Serializable]
        private class FibonacciIterator : IEnumerator<int>
        {
            private int previous = 0;
            private int next = 1;
            private int current = -1;
            
            public int Current
            {
                get
                {
                    if (current == -1)
                    {
                        throw new InvalidOperationException($"{nameof(Current)} is unreachable.");
                    }
                    return current;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                current = previous + next;
                previous = next;
                next = current;
                return true;
            }

            public void Reset()
            {
                current = -1;
            }
        }
    }
}
