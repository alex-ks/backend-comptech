using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Controllers;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class PhotoControllerTests
    {
        private AspApplicationMock app;
        private IConfiguration config;

        public PhotoControllerTests()
        {
            config = (IConfiguration)new Dictionary<string, string>()
            {
                ["SessionTimeout"] = "\"00:01:00\"",
                ["TimeoutCheckInterval"] = "\"00:00:01\""
            };
        }

        private PhotoController CreatePhotoController()
        {

        }

        [Fact]
        public async void GetSessionId()
        {
            //Arrange
            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            PhotoController controller = CreatePhotoController();

            var result = (await controller.GetSessionId(new
                PhotoRequest("1", DateTime.UtcNow),
                app.UserManager,
                new SessionTracker(new LoggerFactory(), sessionRepository, config), decryptor, taskQueue
                ) as OkObjectResult).Value;
            // result has anonymous type so we can only use reflection to get value
            string name = result.GetType().GetProperty("sessionId").GetValue(result) as string;
            Assert.Equal(user.UserName, name);
        }
    }
}
