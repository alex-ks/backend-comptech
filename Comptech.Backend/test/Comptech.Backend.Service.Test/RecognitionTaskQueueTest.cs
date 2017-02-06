using Comptech.Backend.Service.Models;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class RecognitionTaskQueueTest
    {
        private RecognitionTaskQueue queue = new RecognitionTaskQueue(new LoggerFactory());
        private string modelName = "model1";
        private int photoId = 1;

        /// <summary>
        /// Test for Enqueue method
        /// </summary>
        [Fact]
        public void TestEnqueue()
        {
            queue.ClearQueue();
            var task = new RecognitionTask(modelName, photoId);
            queue.Enqueue(task);
            Assert.True(queue.GetQueueCount() == 1);
        }

        /// <summary>
        /// Test for Dequeue method
        /// </summary>
        [Fact]
        public void TestDequeue()
        {
            queue.ClearQueue();
            var task = new RecognitionTask(modelName, photoId);
            queue.Enqueue(task);
            var taskRes = queue.Dequeue();
            Assert.Equal(task, taskRes);
            //not sure, if these tests are required
            Assert.True(queue.GetQueueCount() == 0);
            Assert.True(task.ModelName == taskRes.ModelName);
            Assert.True(task.PhotoId == taskRes.PhotoId);
        }
    }
}
