using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Comptech.Backend.Service.Decryptor;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Service;
using Microsoft.Extensions.Configuration;

namespace Comptech.Backend.Service.Controllers
{
    [Route("rest/[controller]")]
    [Authorize("Bearer")]
    [Produces("application/json")]
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
        public IActionResult GetSessionId([FromBody] PhotoRequest photoRequest,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SessionTracker sessionTracker,
            [FromServices] IImageDecryptor imageDecryptor,
            [FromServices] RecognitionTaskGueue taskQueue,
            [FromServices] IConfiguration configuration)
        {
            using (_logger.BeginScope(nameof(GetSessionId)))
            {
                //validate request
                if (photoRequest.Image == null || photoRequest.TimeStamp == null)
                {
                    _logger.LogError($"Attempt to start a session because of null request.");
                    return BadRequest(new { message = "Error: something in your request is empty" });
                }

                try
                {
                    //try start new session
                    var session = sessionTracker.StartSession(Convert.ToInt32(userManager.GetUserId(HttpContext.User)));
                    _logger.LogInformation($"Trying decrypt image...");
                    var decrypredImage = imageDecryptor.Decrypt(Convert.FromBase64String(photoRequest.Image));
                    var image = new Photo
                    {
                        Image = decrypredImage,
                        SessionID = session.SessionID,
                        TimeStamp = photoRequest.TimeStamp
                    };
                    _photoRepository.Add(image);
                    _logger.LogInformation($"Ecrypted image was decrypted and saved to db.");

                    _logger.LogInformation($"Tring send photoId to RecognitionTaskQueue");
                    string modelName = configuration.GetSection("ModelName").Value;
                    taskQueue.Enqueue(new RecognitionTask { PhotoId = image.PhotoID, modelName = modelName });
                    //photo was sent to RecognitionTasQueue
                    return Ok(session.SessionID);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception caught: {0}, {1}", ex.Message, ex.StackTrace);
                    return StatusCode(409, new { message = ex.Message });
                }
            }
        }
    }
}
