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
            var task = new RecognitionTask();
            task.ModelName = modelName;
            task.PhotoId = photoId;
            queue.Enqueue(task);
            Assert.NotNull(queue.GetQueueCount());
        }

        /// <summary>
        /// Test for Dequeue method
        /// </summary>
        [Fact]
        public void TestDequeue()
        {
            var task = new RecognitionTask();
            task.ModelName = modelName;
            task.PhotoId = photoId;
            queue.Enqueue(task);
            var taskRes = queue.Dequeue();
            Assert.Equal(task, taskRes);
            //not sure, if these two tests are required
            Assert.True(task.ModelName == taskRes.ModelName);
            Assert.True(task.PhotoId == taskRes.PhotoId);
        }
    }
}
