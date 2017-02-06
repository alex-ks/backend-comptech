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
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ISessionRepository sessionRepository;
        private readonly IPulseRepository pulseRepository;
        private readonly IConfiguration configuration;

        public PulseController(UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<PulseController>();
        }

        [Route("rest/bpm")]
        [HttpPost]
        public IActionResult AcceptPulse([FromBody] AcceptPulseRequest request)
        {
            using (logger.BeginScope(nameof(AcceptPulse)))
            {
                logger.LogInformation("Pulse accepted");
                logger.LogInformation("User tries to save pulse");
                try
                {
                    var session = sessionRepository.GetLastSessionForUser(request.SessionId);
                    string errorMessage;
                    if (IsInvalidSession(session, logger, out errorMessage))
                    {
                        logger.LogInformation(errorMessage);
                        return BadRequest(errorMessage);
                    }

                    Pulse pulse = new Pulse(session.SessionID, request.Pulse, request.TimeStamp);
                    pulseRepository.Add(pulse);
                    logger.LogInformation("Pulse of user with sessionId {0} stored in database", request.SessionId);
                    return Ok();

                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
            }
        }

        [Route("rest/pulse")]
        [HttpGet]
        public async Task<IActionResult> SendPulse()
        {
            using (logger.BeginScope(nameof(SendPulse)))
            {
                logger.LogInformation("Pulse sent");
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var userId = await userManager.GetUserIdAsync(user);
                    var session = sessionRepository.GetLastSessionForUser(int.Parse(userId));
                    string errorMessage;
                    if (IsInvalidSession(session, logger, out errorMessage))
                    {
                        logger.LogInformation(errorMessage);
                        return Ok(new { pulse = -1, state = "no" });
                    }

                    var lastPulse = pulseRepository.GetLastPulseInSession(session.SessionID);
                    logger.LogInformation("User {0} sent pulse", userId);
                    return Ok(new { pulse = lastPulse.BPM, state = "yes" });
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
            }
        }

        private bool IsInvalidSession(Session session, ILogger logger, out string errorMessage)
        {
            if (session == null)
            {
                logger.LogInformation("invalid arguments {0}",nameof(session));
                errorMessage = "invalid arguments";
                return true;
            }

            if (session.Status == SessionStatus.FINISHED)
            {
                logger.LogInformation("Session of user with sessionId {0} is ended", session.SessionID);
                errorMessage = "Session of user is ended";
                return true;
            }

            var sessionLenght = TimeSpan.FromSeconds(int.Parse(configuration.GetSection("SessionLength").Value));
            if ((DateTime.UtcNow - session.Start) > sessionLenght)
            {
                session.Status = SessionStatus.FINISHED;
                sessionRepository.Update(session);
                logger.LogInformation("Session of user with sessionId {0} is ended", session.SessionID);
                errorMessage = "Session of user is ended";
                return true;
            }

            errorMessage = null;
            return false;
        }
    }
}
