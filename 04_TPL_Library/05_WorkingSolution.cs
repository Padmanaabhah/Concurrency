using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace _04_TPL_Library
{
    public class MyData
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    class _05_WorkingSolution
    {
        private static readonly object DbLock = new();
        private static readonly string DbPath = "MyMessages.db";
        private static LiteDatabase _db;
        private static DateTime _lastShrinkTime = DateTime.UtcNow;

        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("\nCtrl+C detected. Shutting down...");
                e.Cancel = true; // Prevent immediate process termination
                cts.Cancel();
            };

            _db = new LiteDatabase(DbPath);
            //_db = new LiteDatabase("Filename=MyMessages.db;Connection=shared");
            var messages = _db.GetCollection<MyData>("Messages");
            messages.EnsureIndex(x => x.Timestamp);

            var inputQueue = new BufferBlock<MyData>(new DataflowBlockOptions { BoundedCapacity = 10000 });

            var saveToDbBlock = new TransformBlock<MyData, MyData>(data =>
            {
                // ✅ Validation
                if (string.IsNullOrWhiteSpace(data.Payload))
                {
                    Console.WriteLine($"[INVALID] {data.Id} skipped");
                    return null;
                }

                lock (DbLock)
                {
                    try
                    {
                        messages.Insert(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Save failed {ex.Message}");
                    }
                }

                //Console.WriteLine($"[SAVE] {data.Id}");
                return data;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            var sendToEndpointBlock = new TransformBlock<MyData, MyData>(async data =>
            {
                await Task.Delay(100); // Simulate send delay
                                       //Console.WriteLine($"[FORWARD] {data.Id}");
                return data;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
            });
            var deleteBatchBlock = new BatchBlock<MyData>(10);

            var deleteFromDbBlock = new ActionBlock<MyData[]>(async batch =>
            {
                lock (DbLock)
                {
                    foreach (var data in batch)
                    {
                        try
                        {
                            messages.Delete(data.Id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[DELETE] {ex.Message}");
                        }
                    }

                    if ((DateTime.UtcNow - _lastShrinkTime) > TimeSpan.FromMinutes(2))
                    {
                        _db.Rebuild();
                        _lastShrinkTime = DateTime.UtcNow;
                        Console.WriteLine("[SHRINK] Database compacted.");
                    }
                }
                await Task.CompletedTask;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            // Link blocks with validation filter
            inputQueue.LinkTo(saveToDbBlock, new DataflowLinkOptions { PropagateCompletion = true });
            saveToDbBlock.LinkTo(sendToEndpointBlock, new DataflowLinkOptions { PropagateCompletion = true }, data => data != null);
            saveToDbBlock.LinkTo(DataflowBlock.NullTarget<MyData>()); // Drop invalid

            sendToEndpointBlock.LinkTo(deleteBatchBlock, new DataflowLinkOptions { PropagateCompletion = true });
            deleteBatchBlock.LinkTo(deleteFromDbBlock, new DataflowLinkOptions { PropagateCompletion = true });

            Console.WriteLine("Starting message generation...");
            int messageCounter = 0;

            _ = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var data = new MyData
                    {
                        Id = Guid.NewGuid().ToString(),
                        Payload = $"Payload {messageCounter++}"
                    };
                    await inputQueue.SendAsync(data, cts.Token);

                    if (messageCounter % 100 == 0)
                    {
                        Console.WriteLine($"Posted message {messageCounter}");
                    }

                    await Task.Delay(100, cts.Token); // honor cancellation
                }
            });

            await Task.Delay(-1); // Keep app running

            await Task.Run(() => cts.Token.WaitHandle.WaitOne());

            inputQueue.Complete();

            deleteBatchBlock.TriggerBatch();              // 4. Flush leftover
            deleteBatchBlock.Complete();                  // 5. Mark batch block as done

            // Wait up to 2 minutes for all blocks to complete
            TimeSpan timeout = TimeSpan.FromMinutes(1);

            var saveCompleted = Task.WhenAny(saveToDbBlock.Completion, Task.Delay(timeout));
            var sendCompleted = Task.WhenAny(sendToEndpointBlock.Completion, Task.Delay(timeout));
            var deleteCompleted = Task.WhenAny(deleteFromDbBlock.Completion, Task.Delay(timeout));

            await Task.WhenAll(saveCompleted, sendCompleted, deleteCompleted);

            Console.WriteLine("Processing complete.");
        }
    }
}
