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

namespace Comptech.Backend.Service.Test
{
    /// <summary>
    /// Sample of controllers test
    /// </summary>
    public class UsersTest
    {
        private AspApplicationMock app;

        public UsersTest()
        {
            var config = new Dictionary<string, string>()
            {
                ["SettingsExamle"] = "value"
            };
            app = new AspApplicationMockBuilder(config).Build();
            // Use builder to configure services:
            // builder.Services.AddTransient<...>();
            // app = builder.Build();
        }

        private UserController CreateController()
        {
            return new UserController(app.UserManager,
                                      app.SignInManager,
                                      new LoggerFactory())
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = app.HttpContext
                }
            };
        }

        [Fact]
        public void TestConfiguration()
        {
            var conf = app.ServiceProvider.GetRequiredService<IConfiguration>();
            Assert.Equal("value", conf.GetSection("SettingsExamle").Value);
        }

        [Fact]
        public async Task TestGetUserName()
        {
            var user = new ApplicationUser { UserName = "UserTest", Email = "test@mail.ru" };
            await app.UserManager.CreateAsync(user, "UserTest@123");
            await app.SetUser(user);

            UserController controller = CreateController();
            
            var result = (await controller.GetUserName() as OkObjectResult).Value;
            // result has anonymous type so we can only use reflection to get value
            string name = result.GetType().GetProperty("userName").GetValue(result) as string;
            Assert.Equal(user.UserName, name);
        }
    }
}
