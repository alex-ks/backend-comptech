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
    [Authorize]
    public class PulseController : Controller
    {
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;

        public PulseController(UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<PulseController>();
        }

        [Route("rest/bpm")]
        [HttpPost]
        public async Task<IActionResult> AcceptPulseFromMobile([FromBody] AcceptPulseRequest request,[FromServices] ISessionRepository sessionRepository)
        {
            using (logger.BeginScope(nameof(AcceptPulseFromMobile)))
            {
                logger.LogInformation("Pulse accepted");
                logger.LogInformation("User tries to get name");
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var userId = await userManager.GetUserIdAsync(user);
                    var session = sessionRepository.GetLastSessionForUser(userId);
                }
                catch(Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
                return Ok();
            }
        }
    }
}
