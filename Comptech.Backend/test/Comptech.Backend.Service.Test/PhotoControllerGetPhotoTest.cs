using Comptech.Backend.Service.Controllers;
using Microsoft.Extensions.Logging;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Data.DomainEntities;
using System;
using Moq;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;



namespace Comptech.Backend.Service.Test
{
    public class PhotoControllerGetPhotoTest
    {
        private AspApplicationMock app;

        public PhotoControllerGetPhotoTest()
        {
            app = new AspApplicationMockBuilder(null).Build();
        }

        private PhotoController CreateController(Session testSession, Photo testPhoto)
        {
            var repository = new MockRepository(MockBehavior.Default);

            ISessionRepository sessionRepository = repository.Of<ISessionRepository>()
                .Where(sr => sr.GetLastSessionForUser(It.IsAny<int>()) == testSession)
                .First();

            IPhotoRepository photoRepository = repository.Of<IPhotoRepository>()
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


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetPhotoAndRecognitionResultsTest(bool recognitionResultsArePresent)
        {

            var user = new ApplicationUser
            {
                UserName = recognitionResultsArePresent ? "UserTest" : "UserTest2",
                Email = "test@mail.ru"
            };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            DateTime timeStamp = new DateTime();
            var image = new byte[] { 0x20, 0x20, 0x20 };
            var sessionId = 1;
            var photoId = 2;
            var userId = user.Id;
            Points coords = new Points(new Point(0, 1), new Point(2, 3));
            bool testIsValid = true;

            RecognitionResults testRecognitionResults
                = recognitionResultsArePresent ? new RecognitionResults(testIsValid, coords, photoId) : null;

            Session testSession = new Session(userId, timeStamp, timeStamp, SessionStatus.ACTIVE);
            testSession.SessionID = sessionId;

            Photo testPhoto = new Photo(testSession.SessionID, image, timeStamp);
            testPhoto.PhotoID = photoId;

            var repository = new MockRepository(MockBehavior.Default);

            IRecognitionResultsRepository recognitionResultsRepository = repository.Of<IRecognitionResultsRepository>()
                .Where(rr => rr.GetRecognitionResultsByPhotoId(It.IsAny<int>()) == testRecognitionResults)
                .First();

            PhotoController photoController = CreateController(testSession, testPhoto);

            var result = (await photoController.GetPhotoAndRecognitionResults(
                app.UserManager, recognitionResultsRepository) as OkObjectResult).Value;

            string resultPhoto = getPhotoFromJsonResult(result);

            RecognitionResults requestRecognitionResults = getRecognitionResultsFromJsonResult(result, photoId);

            Assert.Equal(resultPhoto, Convert.ToBase64String(image));
            if (recognitionResultsArePresent)
            {
                Assert.Equal(testRecognitionResults, requestRecognitionResults);
            }
            else
            {
                Assert.Null(requestRecognitionResults);
            }


        }

        private string getPhotoFromJsonResult(Object result)
        {
            return result.GetType().GetProperty("photo").GetValue(result) as string;
        }

        private RecognitionResults getRecognitionResultsFromJsonResult(Object result, int photoId)
        {
            Object recognitionResult = result.GetType().GetProperty("recognitionResult").GetValue(result) as Object;

            if (recognitionResult == null)
            {
                return null;
            }

            bool? isValid = recognitionResult.GetType().GetProperty("valid").GetValue(recognitionResult) as bool?;

            Object cordinates = recognitionResult.GetType().GetProperty("coordinates").GetValue(recognitionResult)
                as Object;

            Object topLeft = cordinates.GetType().GetProperty("topLeft").GetValue(cordinates) as Object;
            int? topLeftX = topLeft.GetType().GetProperty("x").GetValue(topLeft) as int?;
            int? topLeftY = topLeft.GetType().GetProperty("y").GetValue(topLeft) as int?;

            Object bottomRight = cordinates.GetType().GetProperty("bottomRight").GetValue(cordinates) as Object;
            int? bottomRightX = topLeft.GetType().GetProperty("x").GetValue(bottomRight) as int?;
            int? bottomRightY = topLeft.GetType().GetProperty("y").GetValue(bottomRight) as int?;

            return new RecognitionResults(isValid.Value,
                new Points(new Point(topLeftX.Value, topLeftY.Value),
                new Point(bottomRightX.Value, bottomRightY.Value)),
                photoId);
        }

    }
}
