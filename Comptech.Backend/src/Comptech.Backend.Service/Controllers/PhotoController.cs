using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Comptech.Backend.Service.Decryptor;
using Microsoft.Extensions.Configuration;
using Comptech.Backend.Service.Analytics;

namespace Comptech.Backend.Service.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IPhotoRepository _photoRepository;
        private readonly ISessionRepository _sessionRepository;
        #endregion

        #region Ctor
        public PhotoController(ILoggerFactory loggerFactory,
                               IPhotoRepository photoRepository,
                               ISessionRepository sessionRepository)
        {
            _logger = loggerFactory.CreateLogger<UserController>();
            _photoRepository = photoRepository;
            _sessionRepository = sessionRepository;
        }
        #endregion

        /// <summary>
        /// This method gets encrypted photo and returns session id.
        /// </summary>
        [Route("rest/photo")]
        [HttpPost]
        public IActionResult UploadPhotoAndStartSession([FromBody] PhotoRequest photoRequest,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SessionTracker sessionTracker,
            [FromServices] IImageDecryptor imageDecryptor,
            [FromServices] RecognitionTaskQueue taskQueue,
            [FromServices] IConfiguration configuration,
            [FromServices] AnalyticsAgent agent)
        {
            using (_logger.BeginScope(nameof(UploadPhotoAndStartSession)))
            {
                //validate request
                if (photoRequest == null)
                {
                    _logger.LogError("Attempt to start a session because of null mobile photo request.");
                    return BadRequest(new { message = "Error: something in your request is empty" });
                }

                try
                {
                    //try start new session
                    var session = sessionTracker.StartSession(Convert.ToInt32(userManager.GetUserId(HttpContext.User)), photoRequest.TimeStamp);

                    //decrypt image and send to db
                    _logger.LogInformation($"Trying decrypt image...");
                    var decrypredImage = imageDecryptor.Decrypt(Convert.FromBase64String(photoRequest.Image));
                    var image = new Photo(session.SessionID, decrypredImage, photoRequest.TimeStamp);
                    _photoRepository.Add(image);
                    _logger.LogInformation($"Ecrypted image was decrypted and saved to db.");

                    //send to analytics task queue
                    _logger.LogInformation($"Trying send photoId to RecognitionTaskQueue");
                    string modelName = configuration.GetSection("ModelName").Value; // get model name for analytics
                    taskQueue.Enqueue(new RecognitionTask(modelName, image.PhotoID));
                    _logger.LogInformation("Photo was sent to RecognitionTaskQueue");

                    var sessionResponse = new SessionResponse(session.SessionID);
                    return Ok(sessionResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception caught: {0}, {1}", ex.Message, ex.StackTrace);
                    return StatusCode(409, new { message = ex.Message });
                }
            }
        }

        [Route("rest/photo")]
        [HttpGet]
        public async Task<IActionResult> GetPhoto([FromServices] UserManager<ApplicationUser> userManager,
                                                  [FromServices] IPhotoRepository photoRepository,
                                                  [FromServices] ISessionRepository sessionRepository,
                                                  [FromServices] IRecognitionResultsRepository recognitionResultsRepository)
        {
            using (_logger.BeginScope(nameof(GetPhoto)))
            {
                _logger.LogInformation("Frontend tries to get photo");
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var session =
                        sessionRepository.GetLastSessionForUser(int.Parse(await userManager.GetUserIdAsync(user)));

                    if (null == session)
                    {
                        return BadRequest("User has no sessions");
                    }

                    var photo = photoRepository.GetLastPhotoInSession(session.SessionID);
                    var recognitionResults = recognitionResultsRepository.GetRecognitionResultsByPhotoId(photo.PhotoID);

                    var recognitionResultsResponse = (null == recognitionResults)
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
                        recognitionResults = recognitionResultsResponse
                    }
                    );
                }
                catch (Exception exception)
                {
                    _logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
            }
        }

        [Route("rest/photo/result")]
        [HttpGet]
        public async Task<IActionResult> GetRecognitionResults(
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IRecognitionResultsRepository recognitionResultsRepository)
        {
            using (_logger.BeginScope(nameof(GetRecognitionResults)))
            {
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    int userId = int.Parse(await userManager.GetUserIdAsync(user));
                    Session session = _sessionRepository.GetLastSessionForUser(userId);
                    if (session == null)
                    {
                        return BadRequest("Session is not started yet.");
                    }

                    _logger.LogInformation("Trying to get photo for session {0}", session.SessionID);
                    Photo photo = _photoRepository.GetLastPhotoInSession(session.SessionID);

                    _logger.LogInformation("Trying to get recognition results for photo {0}", photo.PhotoID);
                    RecognitionResults recognitionResults =
                        recognitionResultsRepository.GetRecognitionResultsByPhotoId(photo.PhotoID);

                    if (recognitionResults == null)
                    {
                        _logger.LogInformation("Recognition results are not ready for photo {0} yet", photo.PhotoID);
                        return Ok(new { recognitionResult = (string)null });
                    }

                    _logger.LogInformation(
                        "Recognition results for photo {0} were successfully retrieved", photo.PhotoID);

                    return Ok(new
                    {
                        recognitionResult = new
                        {
                            valid = recognitionResults.IsValid,
                            coordinates = new
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
                    _logger.LogError("Exception caught: {0}, {1}", e.Message, e.StackTrace);
                    return BadRequest(e.Message);
                }
            }
        }
    }
}
