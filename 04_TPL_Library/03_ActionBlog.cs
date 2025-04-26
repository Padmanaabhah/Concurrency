using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _04_TPL_Library
{
    class _03_ActionBlog
    {
        public async Task RunActionBlock()
        {
            var printBlock = new ActionBlock<string>(message =>
            {
                Console.WriteLine($"Received: {message}");
            });

            // Send messages to the block
            await printBlock.SendAsync("Hello");
            await printBlock.SendAsync("World");

            printBlock.Complete(); // Tell the block you're done
            await printBlock.Completion; // Wait until it's done
        }
    }
}
