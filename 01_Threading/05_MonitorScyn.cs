using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_Threading
{
    class MonitorScyn
    {
        static int counter = 0; // Shared resource
        static readonly object _lock = new object(); // Lock object

        static void IncrementCounter()
        {
            for (int i = 0; i < 1000; i++)  
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_lock, ref lockTaken); // Explicitly acquire lock
                    counter++;
                }
                finally
                {
                    if (lockTaken) // Ensure lock is released
                    {
                        Monitor.Exit(_lock);
                    }
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
