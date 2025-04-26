using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _04_TPL_Library
{
    class _04_TransformBlock
    {
        public async Task RunTransform()
        {
            var squareBlock = new TransformBlock<int, int>(number =>
            {
                return number * number;
            });

            var printBlock = new ActionBlock<int>(result =>
            {
                Console.WriteLine($"Result: {result}");
            });

            // Link transform to action
            squareBlock.LinkTo(printBlock);

            // Send input
            await squareBlock.SendAsync(3); // Output: 9
            await squareBlock.SendAsync(5); // Output: 25

            squareBlock.Complete();
            await printBlock.Completion;
        }
    }
}
