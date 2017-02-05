using Comptech.Backend.Service.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Comptech.Backend.Service
{
    public class RecognitionTaskQueue
    {
        private volatile BlockingCollection<RecognitionTask> queue = new BlockingCollection<RecognitionTask>(
            new ConcurrentQueue<RecognitionTask>()
            );
        private readonly ILogger logger;

        public RecognitionTaskQueue(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<RecognitionTaskQueue>();
        }

        public RecognitionTask Dequeue()
        {
            RecognitionTask task;
            queue.TryTake(out task);
            return task;
        }

        public void Enqueue(RecognitionTask task)
        {
            queue.TryAdd(task);
        }
    }
}
