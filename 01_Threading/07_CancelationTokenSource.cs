using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_Threading
{
    class CancelationTokenSource
    {
        public void Run()
        {
            Console.WriteLine("Hello, World!");
            var networkingWork = new NetworkingWork1();

            var pingThread = new Thread(networkingWork.CheckNetworkStatus);
            var ctSource = new CancellationTokenSource();

            pingThread.Start(ctSource.Token);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Main thread working...");
                Thread.Sleep(100);
            }

            Console.WriteLine("Cancelation initiated...");
            ctSource.Cancel();
            pingThread.Join();
            ctSource.Dispose();

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

    class NetworkingWork1
    {
        public void CheckNetworkStatus(object data)
        {
            var cancelToken = (CancellationToken)data;
            while (!cancelToken.IsCancellationRequested)
            {
                bool isNetworkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                Console.WriteLine($"Is network available? Answer: {isNetworkUp}");
            }
        }
 
    }
}
