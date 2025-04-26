using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _04_TPL_Library
{
    class _01_BufferBlock
    {
        public async Task RunBuffer()
        {
            var buffer = new BufferBlock<string>();

            // Producer task
            var producer = Task.Run(async () =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    string message = $"Message {i}";
                    await buffer.SendAsync(message);
                    Console.WriteLine($"Produced: {message}");
                    await Task.Delay(500); // simulate delay
                }

                buffer.Complete(); // Signal no more messages
            });

            // Consumer task
            var consumer = Task.Run(async () =>
            {
                while (await buffer.OutputAvailableAsync())
                {
                    var data = await buffer.ReceiveAsync();
                    Console.WriteLine($"\tConsumed: {data}");
                    //await Task.Delay(2000); // simulate delay
                }
            });

            await Task.WhenAll(producer, consumer);
        }
    }
}
