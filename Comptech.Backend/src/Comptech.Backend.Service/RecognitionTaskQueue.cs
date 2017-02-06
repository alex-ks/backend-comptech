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
            using (logger.BeginScope(nameof(Dequeue)))
            {
                var task = queue.Take();
                logger.LogInformation($"Task dequeued. Task information: model name {task.ModelName}, photoId {task.PhotoId}.");
                return task;
            }
        }

        /// <summary>
        /// Adds recognition task to recognition queue for analytics service
        /// </summary>
        /// <param name="task">RecognitionTask - recognition task for analytics service</param>
        public void Enqueue(RecognitionTask task)
        {
            using (logger.BeginScope(nameof(Enqueue)))
            {
                queue.Add(task);
                logger.LogInformation($"Task enqueued. Task information: model name {task.ModelName}, photoId {task.PhotoId}.");
            }
        }

        /// <summary>
        /// Returns recognition tasks count in queue.
        /// It is basically created for unit testing
        /// </summary>
        /// <returns>int - count of recognition tasks in queue</returns>
        public int GetQueueCount()
        {
            return queue.Count;
        }

        /// <summary>
        /// Clears recognition tasks queue. For testing purpose only
        /// </summary>
        public void ClearQueue()
        {
            //remove every item in queue
            for(int i = 0; i < queue.Count; ++i)
                queue.Take();
        }
    }
}
