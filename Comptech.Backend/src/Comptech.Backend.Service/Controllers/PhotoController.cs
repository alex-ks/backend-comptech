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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPhotoRepository _photoRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IImageDecryptor _imageDecryptor;
        private readonly SessionTracker _sessionTracker;
        private readonly RecognitionTaskGueue _taskQueue;
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor
        public PhotoController(UserManager<ApplicationUser> userManager,
                               ILoggerFactory loggerFactory,
                               IPhotoRepository photoRepository,
                               ISessionRepository sessionRepository,
                               IImageDecryptor imageDecryptor,
                               SessionTracker sessionTracker,
                               RecognitionTaskGueue taskQueue,
                               IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<UserController>();
            _photoRepository = photoRepository;
            _sessionRepository = sessionRepository;
            _imageDecryptor = imageDecryptor;
            _sessionTracker = sessionTracker;
            _taskQueue = taskQueue;
            _configuration = configuration;
        }
        #endregion

        /// <summary>
        /// This method gets encrypted photo and returns session id.
        /// </summary>
        [Route("rest/photo")]
        [HttpPost]
        public IActionResult GetSessionId([FromBody] PhotoRequest photoRequest)
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
                    var session = _sessionTracker.StartSession(Convert.ToInt32(_userManager.GetUserId(HttpContext.User)));
                    _logger.LogInformation($"Trying decrypt image...");
                    var decrypredImage = _imageDecryptor.Decrypt(Convert.FromBase64String(photoRequest.Image));
                    var image = new Photo
                    {
                        Image = decrypredImage,
                        SessionID = session.SessionID,
                        TimeStamp = photoRequest.TimeStamp
                    };
                    _photoRepository.Add(image);
                    _logger.LogInformation($"Ecrypted image was decrypted and saved to db.");

                    _logger.LogInformation($"Tring send photoId to RecognitionTaskQueue");
                    string modelName = _configuration.GetSection("ModelName").Value;
                    _taskQueue.Enqueue(new RecognitionTask { PhotoId = image.PhotoID, modelName = modelName });
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
