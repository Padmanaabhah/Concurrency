using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _04_TPL_Library
{
    class _02_BroadcastBlock
    {
        public async Task RunBroadcastBlock()
        {
            var broadcast = new BroadcastBlock<string>(msg => msg); // Just returns the same message

            var consumer1 = new ActionBlock<string>(msg =>
                Console.WriteLine($"Consumer 1 received: {msg}")
            );

            var consumer2 = new ActionBlock<string>(msg =>
                Console.WriteLine($"Consumer 2 received: {msg}")
            );

            // Link broadcast to both consumers
            broadcast.LinkTo(consumer1);
            broadcast.LinkTo(consumer2);

            // Post a single message
            broadcast.Post("IoT gateway connected!");

            await Task.Delay(2000);

            consumer1.Complete();
            consumer2.Complete();

            await Task.WhenAll(consumer1.Completion, consumer2.Completion);
        }
    }
}
