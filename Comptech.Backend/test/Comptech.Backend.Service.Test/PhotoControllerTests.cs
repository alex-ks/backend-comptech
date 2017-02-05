using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Controllers;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Decryptor;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class PhotoControllerTests
    {
        private AspApplicationMock app;
        private IPhotoRepository photoRepository;
        private IConfiguration conf;
        private ISessionRepository sessionRepository;
        private MockRepository repository;
        private IRecognitionResultsRepository recognitionResultsRepository;

        public PhotoControllerTests()
        {
            var config = new Dictionary<string, string>()
            {
                ["SessionTimeout"] = "00:01:00",
                ["TimeoutCheckInterval"] = "00:00:01",
                ["ModelName"] = "1"
            };
            app = new AspApplicationMockBuilder(config).Build();
            conf = app.ServiceProvider.GetRequiredService<IConfiguration>();
        }

        private PhotoController CreateController(Session testSession, Photo testPhoto)
        {
            repository = new MockRepository(MockBehavior.Default);

            sessionRepository = repository.Of<ISessionRepository>()
                .Where(sr => sr.GetLastSessionForUser(It.IsAny<int>()) == testSession)
                .First();

            photoRepository = repository.Of<IPhotoRepository>()
                .Where(pr => pr.GetLastPhotoInSession(It.IsAny<int>()) == testPhoto)
                .First();

            return new PhotoController(new LoggerFactory(), photoRepository, sessionRepository)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = app.HttpContext
                }
            };
        }

        private IRecognitionResultsRepository GetRecognitionResults(MockRepository repository, RecognitionResults testRecognitionResults)
        {
            return recognitionResultsRepository = repository.Of<IRecognitionResultsRepository>()
                .Where(rr => rr.GetRecognitionResultsByPhotoId(It.IsAny<int>()) == testRecognitionResults)
                .First();
        }

        [Fact]
        public async void GetSessionId()
        {
            //Arrange
            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            var testSession = new Session(user.Id, DateTime.UtcNow, DateTime.UtcNow, SessionStatus.FINISHED);
            testSession.SessionID = 1;
            var testPhoto = new Photo(testSession.SessionID, new byte[] { 0x20, 0x20, 0x20 }, DateTime.UtcNow);
            testPhoto.PhotoID = 2;

            IImageDecryptor decryptor = new ImageDecryptorTest();
            IRecognitionTaskQueue taskQueue = new RecognitionTaskQueue(new LoggerFactory());

            //Act
            PhotoController controller = CreateController(testSession, testPhoto);

            var result = (controller.UploadPhotoAndStartSession(
                new PhotoRequest("MQ==", DateTime.UtcNow),
                app.UserManager,
                new SessionTracker(new LoggerFactory(), sessionRepository, conf),
                decryptor,
                taskQueue,
                conf)
                as OkObjectResult).Value;

            var resultValue = result.GetType().GetProperty("SessionId").GetValue(result) as string;

            //Assert
            Assert.Equal<int>(0, Convert.ToInt32(resultValue));
        }
    }
}
