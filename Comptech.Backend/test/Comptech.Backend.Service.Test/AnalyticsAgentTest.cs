using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Analytics;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class AnalyticsAgentTest
    {
        private AspApplicationMock app;
        private IAnalyticsClient client = new AnalyticsClientMock();
        private MockRepository repository = new MockRepository(MockBehavior.Default);
        //private ILoggerFactory loggerFactory = new LoggerFactory();
        private RecognitionTaskQueue queue = new RecognitionTaskQueue(new LoggerFactory());
        private IPhotoRepository photoRepository;
        private IConfiguration configuration;
        private IRecognitionResultsRepository recognitionResultsRepository;

        public AnalyticsAgentTest()
        {
            var config = new Dictionary<string, string>()
            {
                
                ["AnalyticsPollingTimeout"] = "1000",
                ["AnalyticsPollingTimeoutBetweenTasks"] = "1000",
                ["PhotoUploadTryCount"] = "5"
            };
            app = new AspApplicationMockBuilder(config).Build();
            configuration = app.ServiceProvider.GetRequiredService<IConfiguration>();
        }
        
        [Fact]
        public async void CanRun()
        {
            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            var testSession = new Session(user.Id, DateTime.UtcNow, DateTime.UtcNow, SessionStatus.FINISHED);
            testSession.SessionID = 1;
            var testPhoto = new Photo(testSession.SessionID, new byte[] { 0x20, 0x20, 0x20 }, DateTime.UtcNow);
            testPhoto.PhotoID = 2;

            photoRepository = repository.Of<IPhotoRepository>()
                .Where(pr => pr.GetPhotoById(It.IsAny<int>()) == testPhoto)
                .First();

            recognitionResultsRepository = new RecognitionResultsMock();

            for (int i = 0; i < 5; i++)
            {
                RecognitionTask recognitionTask = new RecognitionTask("model1", 2);
                queue.Enqueue(recognitionTask);
            }

            ILoggerFactory logger = new LoggerFactory();
            AnalyticsAgent agent = new AnalyticsAgent(queue, client, logger, photoRepository, recognitionResultsRepository, configuration);

            agent.PollingTask();

            Thread.Sleep(1000);

            Assert.True(recognitionResultsRepository.GetRecognitionResultsByPhotoId(2) != null);
        }
    }
}
