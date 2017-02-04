using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;

        public SessionController(UserManager<ApplicationUser> userManager,
                               ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [Route("/rest/finished")]
        [HttpPost]
        public async Task<IActionResult> FinishSession([FromBody] FinishRequest request)
        {
            using (logger.BeginScope(nameof(FinishSession)))
            {
                //TODO: Добавить логику завершения сессии

                return Ok();
            }
        }
    }
}
