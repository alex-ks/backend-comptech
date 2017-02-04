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

namespace Comptech.Backend.Service.Controllers
{
    [Route("rest/[controller]")]
    [Authorize("Bearer")]
    [Produces("application/json")]
    public class PhotoController : Controller
    {
        #region Fields
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPhotoRepository _photoRepository;
        private readonly ISessionStore _sessionStore;
        private readonly ICryptographyService _cryptoService;
        #endregion

        #region Ctor
        public PhotoController(UserManager<ApplicationUser> userManager,
                               ILoggerFactory loggerFactory,
                               IPhotoRepository photoRepository)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<UserController>();
            _photoRepository = photoRepository;
            _sessionStore = sessionStore;
            _cryptoService = cryptoService;
        }
        #endregion

        /// <summary>
        /// This method gets encrypted photo and returns session id.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetSessionId([FromBody] PhotoRequest photoRequest)
        {
            using (logger.BeginScope(nameof(GetSessionId)))
            {
                //Decrypt photo and save to database
                var image = _cryptoService.Decrypt(Convert.FromBase64String(photoRequest.Image));
                _photoRepository.Add(new Backend.Data.DomainEntities.Photo
                {
                    PhotoID = 0,
                    Image = image,
                    TimeStamp = photoRequest.TimeStamp,
                    SessionID = 0
                });

                //Get current user
                var user = await userManager.GetUserAsync(HttpContext.User);
                try
                {
                    var session = await _sessionStore.CreateSessionForUserName(await userManager.GetUserNameAsync(user));
                    return Ok(session.SessionID);
                }
                catch(ArgumentException argEx)
                {
                    return BadRequest();
                }
                catch(Exception ex)
                {
                    return BadRequest();
                }
            }
        }

    }
}
