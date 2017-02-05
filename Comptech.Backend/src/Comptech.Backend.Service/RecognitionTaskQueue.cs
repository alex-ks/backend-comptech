using Comptech.Backend.Service.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Comptech.Backend.Service
{
    public class RecognitionTaskQueue
    {
        private static RecognitionTaskQueue instance;

        private BlockingCollection<RecognitionTask> queue = new BlockingCollection<RecognitionTask>(
            new ConcurrentQueue<RecognitionTask>()
            );
        private readonly ILogger logger;

        public static RecognitionTaskQueue GetRecognitionTaskQueue()
        {
            if (instance == null)
                instance = new RecognitionTaskQueue(new LoggerFactory());

            return instance;
        }

        public RecognitionTask Dequeue()
        {
            RecognitionTask task = null;
            using (logger.BeginScope(nameof(Dequeue)))
            {
                var result = queue.TryTake(out task);
                if (!result)
                    logger.LogWarning("Failed attempt to dequeue RecognitionTask from queue");
            }
            return task;
        }

        public void Enqueue(RecognitionTask task)
        {
            using (logger.BeginScope(nameof(Enqueue)))
            {
                var result = queue.TryAdd(task);
                if (!result)
                    logger.LogWarning("Failed attempt to add RecognitionTask to RecognitionTaskQueue");
            }
        }

        private RecognitionTaskQueue(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<RecognitionTaskQueue>();
        }
    }
}
