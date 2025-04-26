using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_Threading
{
    class BackgroundThread_Param
    {
        public void StartBackgroundThread()
        {
            var bgThread = new Thread((object? data) =>
            {
                if (data is null) return;

                int counter = 0;
                var result = int.TryParse(data.ToString(), out int maxCount);
                if (!result) return;

                while (counter < maxCount)
                {
                    bool isNetworkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                    Console.WriteLine($"Is network available? Answer: {isNetworkUp}");
                    Thread.Sleep(100);
                    counter++;
                }
            });

            bgThread.IsBackground = true;
            bgThread.Start(12);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Main thread working...");
                Task.Delay(500);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
