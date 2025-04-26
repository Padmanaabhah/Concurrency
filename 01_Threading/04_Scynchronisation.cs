using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_Threading
{
    class Lock_Scync
    {
        static int counter = 0; // Shared resource
        static readonly object _lock = new object(); // Lock object

        static void IncrementCounter()
        {
            for (int i = 0; i < 1000; i++)
            {
                lock (_lock) // Ensures only one thread modifies 'counter' at a time
                {
                    counter++;
                }
            }
        }

        public void Run()
        {
            Thread t1 = new Thread(IncrementCounter);
            Thread t2 = new Thread(IncrementCounter);

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Console.WriteLine($"Final Counter Value: {counter}"); // Always 2000
        }
    }
}
