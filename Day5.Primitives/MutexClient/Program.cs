using System;
using System.Threading;

namespace MutexClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool createdNew = false;

            Mutex mutex = new Mutex(false, "MyMutex", out createdNew); ;
            
            Console.WriteLine("MutexClient. Mutex is new? " + createdNew + ". Waiting until mutex will be released.");

            // wait unit the mutex will be released
            mutex.WaitOne();

            Console.WriteLine("Press any key to release mutex.");
            Console.ReadKey();

            // release mutex
            mutex.ReleaseMutex();

            Console.WriteLine("Mutex is released. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
