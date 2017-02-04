using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comptech.Backend.Service
{
    public class SessionTracker
    {
        private readonly ILogger _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly TimeSpan _sessionTimeout;
        private readonly Timer _timer;

        public SessionTracker(ILogger logger,
            ISessionRepository sessionRepository,
            UserManager<ApplicationUser> userManager,
            TimeSpan sessionTimeout,
            TimeSpan timeoutCheckInterval)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _userManager = userManager;
            _sessionTimeout = sessionTimeout;

            _timer = new Timer(t => RemoveTimedOut(), new object(), timeoutCheckInterval, timeoutCheckInterval);
        }

        public Session StartSession(int userId)
        {
            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt + _sessionTimeout;
            var session = new Session
            {
                UserID = userId,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                LastActive = DateTime.UtcNow,
                Status = SessionStatus.ACTIVE
            };
            _sessionRepository.Add(session);
            _logger.LogInformation($"SessionId:{session.SessionID} for userId:{userId}");
            return session;
        }

        private void RemoveTimedOut()
        {
            var sessionIds = _sessionRepository.GetAllTimedOut(_sessionTimeout);
            _logger.LogInformation($"Timedout: {string.Join(", ", sessionIds)}");
            foreach (var s in sessionIds)
            {
                CloseSession(s);
            }
        }

        private void SetLastActive(int sessionId)
        {
            var session = _sessionRepository.GetSessionById(sessionId);
            session.LastActive = DateTime.UtcNow;
            _sessionRepository.Update(session);
        }

        private void CloseSession(int sessionId)
        {
            var session = _sessionRepository.GetSessionById(sessionId);
            session.Status = SessionStatus.FINISHED;
            _sessionRepository.Update(session);
        }
    }
}
