using Comptech.Backend.Service.Models;
using Comptech.Backend.Service.Analytics;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Test
{
    public class AnalyticsClientTest
    {
        private IAnalyticsClient client = new AnalyticsClient("94.180.119.78");
        private string modelName = "model1";
        private int photoId = 1;
        private string sessionId = null;
        private ILogger logger;

        [Fact]
        public async Task AnalyticsClientTestMethod()
        {
            using (logger.BeginScope(nameof(AnalyticsClientTestMethod)))
            {
                    while (null == sessionId)
                    {   
                        sessionId = await client.RequestRecognitionSession(modelName);
                    }
                    
                    logger.LogInformation("sessionId = {0}", sessionId);
                    
                    await client.UploadPhoto(new byte[10], sessionId);
                    
                    var recRes = await client.TryGetResults(sessionId, photoId);

                    while (null == recRes)
                    {
                        recRes = await client.TryGetResults(sessionId, photoId);
                    } 
            }
        }
    }
}