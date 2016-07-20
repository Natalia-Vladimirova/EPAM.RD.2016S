using System;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new ManualResetEventSlim(false);
            var pingEvent = new AutoResetEvent(false);
            var pongEvent = new AutoResetEvent(false);

            // Create a new cancellation token source.
            CancellationTokenSource cts = new CancellationTokenSource();

            // Assign an appropriate value to token variable.
            CancellationToken token = cts.Token; 
            Action ping = () =>
            {
                Console.WriteLine("ping: Waiting for start.");
                start.Wait(token);

                bool continueRunning = true;

                while (continueRunning)
                {
                    Console.WriteLine("ping!");

                    // write ping-pong functionality here using pingEvent and pongEvent here.
                    pongEvent.Set();
                    pingEvent.WaitOne();
                    
                    Thread.Sleep(1000);
                    
                    // Use cancellation token "token" internals here to set appropriate value.
                    continueRunning = !token.IsCancellationRequested; 
                }

                // Fix issue with blocked pong task.
                pongEvent.Set();

                Console.WriteLine("ping: done");

            };

            Action pong = () =>
            {
                Console.WriteLine("pong: Waiting for start.");
                start.Wait(token);

                pongEvent.WaitOne();
                Thread.Sleep(1000);

                bool continueRunning = true;

                while (continueRunning)
                {
                    Console.WriteLine("pong!");

                    // write ping-pong functionality here using pingEvent or pongEvent here.
                    pingEvent.Set();

                    // write ping-pong functionality here using pingEvent or pongEvent here.
                    pongEvent.WaitOne();

                    Thread.Sleep(1000);

                    // Use cancellation token "token" internals here to set appropriate value.
                    continueRunning = !token.IsCancellationRequested;
                }

                // Fix issue with blocked ping task.
                pingEvent.Set();

                Console.WriteLine("pong: done");
            };


            var pingTask = Task.Run(ping, token);
            var pongTask = Task.Run(pong, token);

            Console.WriteLine("Press any key to start.");
            Console.WriteLine("After ping-pong game started, press any key to exit.");
            Console.ReadKey();
            start.Set();

            Console.ReadKey();
            // cancel both tasks using cancellation token.
            cts.Cancel();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
