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
    public class PhotoController : Controller
    {
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        
        public PhotoController(UserManager<ApplicationUser> userManager,
                                ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [Route("rest/faces")]
        [HttpGet]
        public IActionResult GetPhoto()
        {
            using (logger.BeginScope(nameof(GetPhoto)))
            {
                logger.LogInformation("Send Photo");
                return Ok();
            }
        } 
    }
}
