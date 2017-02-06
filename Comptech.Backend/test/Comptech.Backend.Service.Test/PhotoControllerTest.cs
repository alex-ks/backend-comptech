using Comptech.Backend.Service.Controllers;
using Comptech.Backend.Service.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Moq;
using Comptech.Backend.Data.Repositories;
using System.Linq;
using Comptech.Backend.Data.DomainEntities;
using System;

namespace Comptech.Backend.Service.Test
{
    public class PhotoControllerTest
    {
        private AspApplicationMock app;
        private byte[] image;
        private int photoId = 2;
        private RecognitionResults testResults;

        public PhotoControllerTest()
        {
            var config = new Dictionary<string, string>()
            {
                ["settings"] = "value"
            };

            app = new AspApplicationMockBuilder(config).Build();
        }

        private PhotoController CreateController()
        {
            return new PhotoController(app.UserManager,
                                       new LoggerFactory())
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = app.HttpContext
                }
            };
        }

        [Fact]
        private async Task TestGetPhoto()
        {
            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            ISessionRepository sessionRepository;
            IPhotoRepository photoRepository;
            IRecognitionResultsRepository recognitionResultsRepository;

            InstantiateRepositories(user, out sessionRepository, out photoRepository, out recognitionResultsRepository);

            PhotoController controller = CreateController();

            var result = (await controller.GetPhoto(photoRepository,
                         sessionRepository, recognitionResultsRepository) as OkObjectResult).Value;

            var resultPhoto = result.GetType().GetProperty("photo").GetValue(result) as string;
            var recognitionResults = getRecognitionResultsFromJsonResults(result, photoId);

            Assert.Equal(resultPhoto, Convert.ToBase64String(image));
            Assert.Equal(testResults.IsValid, recognitionResults.IsValid);
            Assert.Equal(testResults.PhotoID, recognitionResults.PhotoID);
            Assert.Equal(testResults.Coords.TopLeft.X, recognitionResults.Coords.TopLeft.X);
            Assert.Equal(testResults.Coords.TopLeft.Y, recognitionResults.Coords.TopLeft.Y);
            Assert.Equal(testResults.Coords.BottomRight.X, recognitionResults.Coords.BottomRight.X);
            Assert.Equal(testResults.Coords.BottomRight.Y, recognitionResults.Coords.BottomRight.Y);
        }
        
        private void InstantiateRepositories(ApplicationUser user,
                                             out ISessionRepository sessionRepository,
                                             out IPhotoRepository photoRepository,
                                             out IRecognitionResultsRepository recognitionResultsRepository)
        {
            var repository = new MockRepository(MockBehavior.Default);

            image = new byte[] { 0x20, 0x20, 0x20 };
            testResults = new RecognitionResults(true, new Points(new Point(0,1), new Point(2,3)), photoId);
            
            var timeStamp = new DateTime();
            var userId = user.Id;

            Session testSession = new Session(user.Id, timeStamp, timeStamp, SessionStatus.ACTIVE);
            testSession.SessionID = 1;
            
            Photo testPhoto = new Photo(testSession.SessionID, image, timeStamp);
            testPhoto.PhotoID = 2;


            sessionRepository = repository.Of<ISessionRepository>()
                .Where(sr => sr.GetLastSessionForUser(It.IsAny<int>()) == testSession)
                .First();

            photoRepository = repository.Of<IPhotoRepository>()
                .Where(pr => pr.GetLastPhotoInSession(It.IsAny<int>()) == testPhoto)
                .First();
                
            recognitionResultsRepository = repository.Of<IRecognitionResultsRepository>()
                .Where(rr => rr.GetRecognitionResultsByPhotoId(It.IsAny<int>()) == testResults)
                .First();
        }

        private RecognitionResults getRecognitionResultsFromJsonResults(Object result, int photoId)
        {
            Object recognitionResults = result.GetType().GetProperty("recognitionResults").GetValue(result) as Object;

            if (null == recognitionResults) return null;

            bool? isValid = recognitionResults
                            .GetType()
                            .GetProperty("valid")
                            .GetValue(recognitionResults) as bool?;
            
            Object coords = recognitionResults
                            .GetType()
                            .GetProperty("coordinates")
                            .GetValue(recognitionResults) as Object;

            Object topLeftCoords = coords
                                   .GetType()
                                   .GetProperty("topLeft")
                                   .GetValue(coords) as Object;

            Object bottomRightCoords = coords
                                       .GetType()
                                       .GetProperty("bottomRight")
                                       .GetValue(coords) as Object;
            
            int? topLeftX = topLeftCoords
                            .GetType()
                            .GetProperty("x")
                            .GetValue(topLeftCoords) as int?;

            int? topLeftY = topLeftCoords
                            .GetType()
                            .GetProperty("y")
                            .GetValue(topLeftCoords) as int?;
            
            int? bottomRightX = bottomRightCoords
                                .GetType()
                                .GetProperty("x")
                                .GetValue(bottomRightCoords) as int?;

            int? bottomRightY = bottomRightCoords
                                .GetType()
                                .GetProperty("y")
                                .GetValue(bottomRightCoords) as int?;

            return new RecognitionResults(isValid.Value,
                                          new Points(new Point(topLeftX.Value, topLeftY.Value),
                                                     new Point(bottomRightX.Value, bottomRightY.Value)),
                                          photoId);
        }
    }
}