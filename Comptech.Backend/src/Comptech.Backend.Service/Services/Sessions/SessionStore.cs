using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Service.Controllers;
using Microsoft.AspNetCore.Http;

namespace Comptech.Backend.Service.Services.Sessions
{
    public class SessionStore : ISessionStore
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly TimeSpan _sessionTimeout;
        private readonly Timer _timer;
        
        public SessionStore(ISessionRepository sessRepo, 
            UserManager<ApplicationUser> userManager,
            ILogger<SessionStore> logger,
            TimeSpan sessionTimeout,
            TimeSpan timeoutCheckInterval)
        {
            _logger = logger;
            _sessionRepository = sessRepo;
            _userManager = userManager;
            _sessionTimeout = sessionTimeout;

            _timer = new Timer(t => RemoveTimedOut(), new object(), timeoutCheckInterval, timeoutCheckInterval);
        }

        public Session GetSession(HttpContext context)
        {
            string sessionId;
            if (!context.Request.Headers.TryGetValue("sessionId", out sessionId)) //or context.Items?
            {
                throw new InvalidOperationException("This context has no sessionId header");
            }
            return GetSessionById(int.Parse(sessionId));
        }

        private void RemoveTimedOut()
        {
            var sessionIds = _sessionRepository.GetAllTimedOut(_sessionTimeout);
            _logger.LogInformation($"Timedout: {string.Join(", ", sessionIds)}");
            foreach(var s in sessionIds)
            {
                CloseSession(s);
            }
        }

        public Session GetSessionById(int id)
        {
            return _sessionRepository.GetActualById(id);
        }

        public async Task<Session> CreateSessionForUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Cannot create session: user name cannot be empty or null");
            }
            var user =  await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new ArgumentException($"Cannot create session: user {userName} not found");
            }
            var createdAt = DateTime.Now;
            var expiresAt = createdAt + _sessionTimeout;
            var session = new Session()
            {
                SessionID = 0,
                CreatedAt = DateTime.Now,
                ExpiresAt = expiresAt,
                UserID = user.Id
            };
            _sessionRepository.Add(session);
            _logger.LogInformation($"Session for {userName} has been created");
            return session;
        }

        public async Task<Session> CreateSessionForUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("Cannot create session: user id cannot be empty or null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Cannot create session: user {userId} not found");
            }
            var createdAt = DateTime.Now;
            var expiresAt = createdAt + _sessionTimeout;
            var session = new Session()
            {
                SessionID = 0,
                CreatedAt = DateTime.Now,
                ExpiresAt = expiresAt,
                UserID = Convert.ToInt32(userId)
        };
            _sessionRepository.Add(session);
            _logger.LogInformation($"Session for {userId} has been created");
            return session;
        }

        public void CloseSession(int sessionId)
        {
            var session = _sessionRepository.GetActualById(sessionId);
            session.ClosedAt = DateTime.Now;
            _sessionRepository.Update(session);

        }

    }
}
