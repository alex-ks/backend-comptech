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

namespace Comptech.Backend.Service.Test
{
    public class PhotoControllerTest
    {
        private AspApplicationMock app;

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

        [Theory]
        private async Task TestGetPhoto(Session testSession,
                                        Photo testPhoto,
                                        RecognitionResults testRecognitionResults)
        {
            var repository = new MockRepository(MockBehavior.Default);

            ISessionRepository sessionRepository = repository.Of<ISessionRepository>()
                .Where(sr => sr.GetLastSessionForUser(It.IsAny<int>()) == testSession)
                .First();

            IPhotoRepository photoRepository = repository.Of<IPhotoRepository>()
                .Where(pr => pr.GetLastPhotoInSession(It.IsAny<int>()) == testPhoto)
                .First();
                
            IRecognitionResultsRepository recognitionResultsRepository = repository.Of<IRecognitionResultsRepository>()
                .Where(rr => rr.GetRecognitionResultsByPhotoId(It.IsAny<int>()) == testRecognitionResults)
                .First();


            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            PhotoController controller = CreateController();

            var result = (await controller.GetPhoto(photoRepository,
                         sessionRepository, recognitionResultsRepository) as OkObjectResult).Value; 
        } 
    }
}