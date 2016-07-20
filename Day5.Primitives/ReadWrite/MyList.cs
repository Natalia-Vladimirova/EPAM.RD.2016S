using System;
using System.Collections.Generic;
using System.Threading;

namespace ReadWrite
{
    // TODO: Use ReaderWriterLockSlim to protect this class.
    public class MyList
    {
        private List<int> list = new List<int>()
        {
            1,
            2,
            3,
            4
        };

        private ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        public void Add(int value)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                Console.WriteLine("Write");
                list.Add(value);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void Remove()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                if (list.Count > 0)
            {
                Console.WriteLine("Write");
                list.RemoveAt(0);
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public int Get()
        {
            readerWriterLock.EnterReadLock();
            try
            {
                int value = 0;
                if (list.Count > 0)
                {
                    Console.WriteLine("Read");
                    value = list[0];
                }

                return value;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }
    }
}
