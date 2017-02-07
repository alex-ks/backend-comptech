using Comptech.Backend.Service.Models;
using Comptech.Backend.Service.Analytics;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using System.Threading.Tasks;
using System.Threading;

namespace Comptech.Backend.Service.Test
{
    public class AnalyticsClientTest
    {
        private IAnalyticsClient client = new AnalyticsClient("http://94.180.119.78/");
        private string modelName = "model1";
        private int photoId = 1;
        private string sessionId = null;

        [Fact]
        public async Task AnalyticsClientTestMethod()
        {
            while (null == sessionId)
            {   
                sessionId = await client.RequestRecognitionSession(modelName);
            }
            
            await client.UploadPhoto(new byte[]{1, 2, 3, 4}, sessionId);
            
            var recRes = await client.TryGetResults(sessionId, photoId);

            while (null == recRes)
            {
                recRes = await client.TryGetResults(sessionId, photoId);
            } 
        }
    }
}