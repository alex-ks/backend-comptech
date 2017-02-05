﻿using Comptech.Backend.Data.DomainEntities;
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

        public PulseController(UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            logger = loggerFactory.CreateLogger<PulseController>();
        }

        [Route("rest/bpm")]
        [HttpPost]
        public async Task<IActionResult> AcceptPulse([FromBody] AcceptPulseRequest request,
                                                     [FromServices] IConfiguration configuration)
        {
            using (logger.BeginScope(nameof(AcceptPulse)))
            {
                logger.LogInformation("Pulse accepted");
                logger.LogInformation("User tries to save pulse");
                try
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var userId = await userManager.GetUserIdAsync(user);
                    var session = sessionRepository.GetLastSessionForUser(int.Parse(userId));
                    if (session.Status == SessionStatus.ACTIVE)
                    {
                        var sessionLenght = TimeSpan.FromSeconds(int.Parse(
                                configuration.GetSection("SessionLenght").Value));
                        if ((DateTime.Now - session.Start) < sessionLenght)
                        {
                            Pulse pulse = new Pulse();
                            pulse.SessionID = session.SessionID;
                            pulse.TimeStamp = request.TimeStamp;
                            pulse.BPM = request.Pulse;
                            pulseRepository.Add(pulse);
                            logger.LogInformation("User {0} sent pulse", userId);
                            return Ok();
                        }
                        else
                        {
                            logger.LogInformation("Users {0} session is ended", userId);
                            session.Status = SessionStatus.FINISHED;
                            sessionRepository.Update(session);
                            return Forbid();
                        }
                    }
                    else
                    {
                        logger.LogInformation("Users {0} session is ended", userId);
                        return Forbid();
                    }

                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    return BadRequest(exception.Message);
                }
            }
        }
        /*
        [Route("rest/pulse")]
        [HttpGet]
        public async Task<IActionResult> SendPulse()
        {
            using (logger.BeginScope(nameof(SendPulse)))
            {
                logger.LogInformation("Pulse sent");
            }
        }*/
    }
}
