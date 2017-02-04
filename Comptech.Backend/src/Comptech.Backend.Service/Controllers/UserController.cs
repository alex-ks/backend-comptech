using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Controllers
{
    [Authorize] // All methods except AllowAnonymous will require authorization token
    public class UserController : Controller
    {
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        // route differs from /rest/... in order to match ASP.NET identity token route:
        // /connect/token
        [Route("connect/register")]
        [HttpPost]
        [AllowAnonymous] // This method does not require authorization token
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            using (logger.BeginScope(nameof(Register)))
            {
                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };

                var result = await userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User {0} created", request.UserName);
                    return Ok();
                }
                logger.LogError("User {0} registration failed", request.UserName);
                return NotFound(result.Errors);
            }
        }

        // conventional method route:
        // /rest/<resource>/...
        [Route("rest/user/name")]
        [HttpGet]
        public async Task<IActionResult> GetUserName()
        {
            using (logger.BeginScope(nameof(Register)))
            {
                logger.LogInformation($"User tries to get name");
                var user = await userManager.GetUserAsync(HttpContext.User);
                // use anonymous type to produce json
                return Ok(new { userName = await userManager.GetUserNameAsync(user) });
            }
        }

        [Route("connect/logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            using (logger.BeginScope(nameof(Logout)))
            {
                try
                {
                    await signInManager.SignOutAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    logger.LogError("Exception caught: {0}, {1}", e.Message, e.StackTrace);
                    return BadRequest(e.Message);
                }
            }
        }
    }
}
