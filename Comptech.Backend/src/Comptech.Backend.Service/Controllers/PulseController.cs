using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Data;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPulseRepository _pulseRepository;
        private readonly IConfiguration _configuration;
        private readonly SessionValidator _sessionValidator;

        public PulseController(UserManager<ApplicationUser> userManager, 
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            SessionValidator sessionValidator,
            IPulseRepository pulseRepository,
            ISessionRepository sessionRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _sessionValidator = sessionValidator;
            _pulseRepository = pulseRepository;
            _sessionRepository = sessionRepository;
            _logger = loggerFactory.CreateLogger<PulseController>();
        }

        [Route("rest/bpm")]
        [HttpPost]
        public IActionResult AcceptPulse([FromBody] AcceptPulseRequest request)
        {
            using (_logger.BeginScope(nameof(AcceptPulse)))
            {
                if (request == null)
                {
                    _logger.LogInformation("request is null");
                    return BadRequest("request is null");
                }
                _logger.LogInformation("Pulse accepted. User tries to save pulse");
                try
                {
                    var session = _sessionRepository.GetLastSessionForUser(request.SessionId);
                    string errorMessage;
                    if (_sessionValidator.IsNullOrFinishedSession(session, out errorMessage))
                    {
                        _logger.LogInformation(errorMessage);
                        return BadRequest(errorMessage);
                    }

                    var pulse = new Pulse(session.SessionID, request.Pulse, request.TimeStamp);
                    _pulseRepository.Add(pulse);
                    _logger.LogInformation($"Pulse of user with sessionId {request.SessionId} stored in database");
                    return Ok();

                }
                catch (Exception exception)
                {
                    _logger.LogError($"Exception caught: {exception.Message}, {exception.StackTrace}");
                    return BadRequest(exception.Message);
                }
            }
        }

        [Route("rest/pulse")]
        [HttpGet]
        public async Task<IActionResult> SendPulse()
        {
            using (_logger.BeginScope(nameof(SendPulse)))
            {
                _logger.LogInformation("Pulse sent");
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    var userId = await _userManager.GetUserIdAsync(user);
                    var session = _sessionRepository.GetLastSessionForUser(int.Parse(userId));

                    string errorMessage;
                    if (_sessionValidator.IsNullOrFinishedSession(session, out errorMessage))
                    {
                        _logger.LogInformation(errorMessage);
                        return Ok(new { pulse = -1, state = "no" });
                    }

                    var lastPulse = _pulseRepository.GetLastPulseInSession(session.SessionID);
                    _logger.LogInformation($"User {userId} sent pulse");
                    return Ok(new { pulse = lastPulse.BPM, state = "yes" });
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Exception caught: {exception.Message}, {exception.StackTrace}");
                    return BadRequest(exception.Message);
                }
            }
        }
    }
}
