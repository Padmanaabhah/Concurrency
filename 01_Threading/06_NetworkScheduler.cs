using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_Threading
{
    class Network_scheduler
    {
        public void Run()
        {
            Console.WriteLine("Hello, World!");
            var networkingWork = new NetworkingWork();

            var bgThread1 = new Thread(networkingWork.CheckNetworkStatus);
            var bgThread2 = new Thread(networkingWork.CheckNetworkStatus);
            var bgThread3 = new Thread(networkingWork.CheckNetworkStatus);
            var bgThread4 = new Thread(networkingWork.CheckNetworkStatus);
            var bgThread5 = new Thread(networkingWork.CheckNetworkStatus);

            bgThread1.Priority = ThreadPriority.Lowest;
            bgThread2.Priority = ThreadPriority.BelowNormal;
            bgThread3.Priority = ThreadPriority.Normal;
            bgThread4.Priority = ThreadPriority.AboveNormal;
            bgThread5.Priority = ThreadPriority.Highest;

            bgThread1.Start("Lowest");
            bgThread2.Start("BelowNormal");
            bgThread3.Start("Normal");
            bgThread4.Start("AboveNormal");
            bgThread5.Start("Highest");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Main thread working...");
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

    class NetworkingWork
    {
        public void CheckNetworkStatus(object data)
        {
            for (int i = 0; i < 12; i++)
            {
                bool isNetworkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                Console.WriteLine($"Thread priority {(string)data}; Is network available? Answer: {isNetworkUp}");
                i++;
            }
        }
    }
}
