using System;
using System.Collections;
using System.Collections.Generic;
using UserStorage.Interfaces.Generators;

namespace IdGenerator
{
    /// <summary>
    /// Id generator that uses fibonacci sequence as ids.
    /// </summary>
    public class FibonacciIdGenerator : IIdGenerator
    {
        private readonly IEnumerator<int> idGenerator;

        public FibonacciIdGenerator()
        {
            idGenerator = new FibonacciIterator();
        }

        /// <summary>
        /// Returns current id from fibonacci sequence.
        /// </summary>
        public int CurrentId => idGenerator.Current;

        /// <summary>
        /// Generates an id from fibonacci sequence.
        /// </summary>
        /// <returns>
        /// Whether the next id exists.
        /// </returns>
        public bool GenerateNextId()
        {
            return idGenerator.MoveNext();
        }

        /// <summary>
        /// Sets a value which the generator has to start from.
        /// </summary>
        /// <param name="initialValue">
        /// Starting value.
        /// </param>
        public void SetInitialValue(int initialValue)
        {
            if (initialValue > 0)
            {
                do
                {
                    idGenerator.MoveNext();
                }
                while (idGenerator.Current < initialValue);
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

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                checked
                {
                    current = previous + next;
                }

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
