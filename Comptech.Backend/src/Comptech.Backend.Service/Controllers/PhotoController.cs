using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
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

        [Route("rest/photo")]
        [HttpGet]
        public async Task<IActionResult> GetPhoto([FromServices] IPhotoRepository photoRepository,
                                                  [FromServices] ISessionRepository sessionRepository,
                                                  [FromServices] IRecognitionResultsRepository recognitionResultsRepository)
        {
            using (logger.BeginScope(nameof(GetPhoto)))
            {
                logger.LogInformation("Frontend tries to get photo");
                try {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var session = 
                        sessionRepository.GetLastSessionForUser(int.Parse(await userManager.GetUserIdAsync(user)));

                    if (null == session)
                    {
                        return BadRequest("User has no sessions");
                    }

                    var photo = photoRepository.GetLastPhotoInSession(session.SessionID);
                    var recognitionResults = recognitionResultsRepository.GetRecognitionResultsByPhotoId(photo.PhotoID);
                    
                    var recRes = (null == recognitionResults) 
                    ?
                    null
                    :
                    new 
                    {
                        valid = recognitionResults.IsValid,
                        coordinates = new 
                        {
                            topLeft = new 
                            {
                                x = recognitionResults.Coords.TopLeft.X,
                                y = recognitionResults.Coords.TopLeft.Y
                            },
                            bottomRight = new 
                            {
                                x = recognitionResults.Coords.BottomRight.X,
                                y = recognitionResults.Coords.BottomRight.Y
                            }
                        }
                    };

                    return Ok(new 
                        {
                            photo = System.Convert.ToBase64String(photo.Image),
                            recognitionResults = recRes
                        }
                    );
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
            }
        } 
    }
}
