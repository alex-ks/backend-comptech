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
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ISessionRepository sessionRepository;
        private readonly IRepository<Session> repository;

        public SessionController(UserManager<ApplicationUser> userManager,
                               ILoggerFactory loggerFactory,
                               ISessionRepository sessionRepository,
                               IRepository<Session> repository)
        {
            this.userManager = userManager;
            this.sessionRepository = sessionRepository;
            this.repository = repository;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [Route("/rest/finished")]
        [HttpPost]
        public async Task<IActionResult> FinishSession([FromBody] FinishRequest request)
        {
            try
            {
                using (logger.BeginScope(nameof(FinishSession)))
                {
                    //если в запросе нет id сессии, то возвращаем ошибку(?)
                    if (request.SessionId == null)
                    {
                        logger.LogError($"Attempted to finish session with null sessionId");
                        return BadRequest(new
                        {
                            message = "Error: Session ID is empty"
                        });
                    }

                    var user = await userManager.GetUserAsync(HttpContext.User);
                    var userId = await userManager.GetUserIdAsync(user);

                    var session = sessionRepository.GetLastSessionForUser(int.Parse(userId));
                    //проверяем несовпадение переданного id сессии и id сессии в базе
                    if (session.SessionID != request.SessionId)
                    {
                        logger.LogCritical($"User {userId}: Obtained session ID is incorrect or malformed.");
                        return BadRequest(new
                        {
                            message = "Error: Authentification failure. Your session ID is incorrect of malformed."
                        });
                    }

                    //если сессия уже почему-то завершена, то возвращаем 409 Conflict
                    if (session.Status == SessionStatus.FINISHED)
                    {
                        logger.LogWarning($"User {userId}: Session is already finished.");
                        return StatusCode(409, new
                        {
                            message = "Error: Session is already finished."
                        }); //Conflict
                    }
                    //обновляем статус
                    session.Status = SessionStatus.FINISHED;
                    repository.Update(session);

                    return Ok();
                }
            }
            catch(Exception e)
            {
                logger.LogCritical(e.Message);
                logger.LogTrace(e.StackTrace);
                return BadRequest(new
                {
                    message = e.Message
                });
            }
        }
    }
}
