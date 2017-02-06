using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Controllers
{

    [Authorize]
    public class PhotoController : Controller
    {
        private readonly ILogger logger;
        private readonly IPhotoRepository photoRepository;
        private readonly IRecognitionResultsRepository recognitionResultsRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public PhotoController(IPhotoRepository photoRepository, ISessionRepository sessionRepository,
            IRecognitionResultsRepository recognitionResultsRepository, UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory)
        {
            this.photoRepository = photoRepository;
            this.sessionRepository = sessionRepository;
            this.recognitionResultsRepository = recognitionResultsRepository;
            this.userManager = userManager;
            this.logger = loggerFactory.CreateLogger<PhotoController>();
        }

        [Route("rest/faces")]
        [HttpGet]
        public async Task<IActionResult> GetPhoto()
        {
            using (logger.BeginScope(nameof(GetPhoto)))
            {
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    int userId = int.Parse(await userManager.GetUserIdAsync(user));
                    Session session = sessionRepository.GetLastSessionForUser(userId);
                    Photo photo = photoRepository.GetLastPhotoInSession(session.SessionID);
                    RecognitionResults recognitionResults =
                        recognitionResultsRepository.GetRecognitionResultsByPhotoId(photo.PhotoID);

                    return Ok(new
                    {
                        foto = Convert.ToBase64String(photo.Image),
                        recognitionResult = new
                        {
                            valid = recognitionResults.IsValid,
                            cordinates = new
                            {
                                topLeft = new
                                {
                                    x = recognitionResults.Coords.TopLeft.X,
                                    y = recognitionResults.Coords.TopLeft.Y,
                                },
                                bottomRight = new
                                {
                                    x = recognitionResults.Coords.BottomRight.X,
                                    y = recognitionResults.Coords.BottomRight.Y,
                                }
                            }
                        }
                    });
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
