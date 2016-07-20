using System;
using System.Collections.Generic;
using System.Threading;

namespace ReadWrite
{
    class Program
    {
        // replace Object type with appropriate type for slim version of manual reset event.
        private static IList<Thread> CreateWorkers(ManualResetEventSlim mres, Action action, int threadsNum, int cycles)
        {
            var threads = new Thread[threadsNum];

            for (int i = 0; i < threadsNum; i++)
            {
                Action d = () =>
                {
                    // Wait for signal.
                    mres.Wait();

                    for (int j = 0; j < cycles; j++)
                    {
                        action();
                    }
                };

                // Create a new thread that will run the delegate above here.
                Thread thread = new Thread(() => d()); 

                threads[i] = thread;
            }

            return threads;
        }

        static void Main(string[] args)
        {
            var list = new MyList();

            // Replace Object type with slim version of manual reset event here.
            var mres = new ManualResetEventSlim();

            var threads = new List<Thread>();

            threads.AddRange(CreateWorkers(mres, () => { list.Add(1); }, 10, 100));
            threads.AddRange(CreateWorkers(mres, () => { list.Get(); }, 10, 100));
            threads.AddRange(CreateWorkers(mres, () => { list.Remove(); }, 10, 100));

            foreach (var thread in threads)
            {
                // Start all threads.
                thread.Start();
            }

            Console.WriteLine("Press any key to run unblock working threads.");
            Console.ReadKey();

            // NOTE: When an user presses the key all waiting worker threads should begin their work.
            // Send a signal to all worker threads that they can run.
            mres.Set();

            foreach (var thread in threads)
            {
                // Wait for all working threads
                thread.Join();
            }

            Console.WriteLine("Press any key.");
            Console.ReadKey();
        }
    }
}
