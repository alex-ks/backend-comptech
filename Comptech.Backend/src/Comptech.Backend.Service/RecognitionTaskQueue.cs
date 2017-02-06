using Comptech.Backend.Service.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Comptech.Backend.Service
{
    /// <summary>
    /// RecognitionTaskQueue which handles recognition tasks sent to analytics by adding
    /// <code>RecognitionTask</code> to queue and removing from it
    /// </summary>
    /// <example>
    /// Adding task to queue:
    /// <code>
    /// RecognitionTask task = new RecognitionTask();
    /// ... //fill in RecognitionTask properties
    /// RecognitioTaskQueue queue = ... //get RecognitionTaskQueue from DI
    /// queue.Enqueue(task);
    /// </code>
    /// Getting task from queue:
    /// <code>
    /// RecognitioTaskQueue queue = ... //get RecognitionTaskQueue from DI
    /// RecognitionTask task = queue.Dequeue();
    /// </code>
    /// </example>
    public class RecognitionTaskQueue
    {
        private volatile BlockingCollection<RecognitionTask> queue = new BlockingCollection<RecognitionTask>(
            new ConcurrentQueue<RecognitionTask>()
            );
        private readonly ILogger logger;

        /// <summary>
        /// Basic constructor called by DI
        /// </summary>
        /// <param name="loggerFactory"></param>
        public RecognitionTaskQueue(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<RecognitionTaskQueue>();
        }

        /// <summary>
        /// Removes RecognitionTask from queue and returns it
        /// May hang up if queue is empty until at least one task is in queue
        /// </summary>
        /// <returns>RecognitionTask - recognition task for analytics service</returns>
        public RecognitionTask Dequeue()
        {
            RecognitionTask task;
            queue.TryTake(out task);
            return task;
        }

        /// <summary>
        /// Adds recognition task to recognition queue for analytics service
        /// </summary>
        /// <param name="task">RecognitionTask - recognition task for analytics service</param>
        public void Enqueue(RecognitionTask task)
        {
            queue.TryAdd(task);
        }
    }
}
