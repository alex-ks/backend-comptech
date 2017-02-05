using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Comptech.Backend.Service.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionRepository _sessionRepository;
        private readonly SessionValidator _sessionValidator;
        private readonly SessionTracker _sessionTracker;

        public SessionController(UserManager<ApplicationUser> userManager,
                               ILoggerFactory loggerFactory,
                               ISessionRepository sessionRepository,
                               SessionValidator sessionValidator,
                               SessionTracker sessionTracker)
        {
            _userManager = userManager;
            _sessionRepository = sessionRepository;
            _sessionValidator = sessionValidator;
            _sessionTracker = sessionTracker;
            _logger = loggerFactory.CreateLogger<SessionController>();
        }

        [Route("/rest/finished")]
        [HttpPost]
        public async Task<IActionResult> FinishSession([FromBody] FinishRequest request)
        {
            try
            {
                using (_logger.BeginScope(nameof(FinishSession)))
                {
                    if (request.SessionId == null)
                    {
                        _logger.LogError($"Attempted to finish session with null sessionId");
                        return BadRequest(new
                        {
                            message = "Error: Session ID is empty"
                        });
                    }

                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    var userId = await _userManager.GetUserIdAsync(user);

                    var session = _sessionRepository.GetLastSessionForUser(int.Parse(userId));
                    
                    if (session.SessionID != request.SessionId)
                    {
                        _logger.LogCritical($"User {userId}: Obtained session ID is incorrect or malformed.");
                        return BadRequest(new
                        {
                            message = "Error: Authentification failure. Your session ID is incorrect of malformed."
                        });
                    }

                    string errorMessage;
                    if (_sessionValidator.IsFinishedSession(session, out errorMessage))
                    {
                        _logger.LogWarning($"{errorMessage} for user {userId}");
                        return StatusCode(409, errorMessage);
                    }

                    _sessionTracker.CloseSession(session.SessionID);
                    return Ok();
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical(e.Message);
                _logger.LogTrace(e.StackTrace);
                return BadRequest(new
                {
                    message = e.Message
                });
            }
        }
    }
}
