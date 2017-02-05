using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comptech.Backend.Service
{
    public class SessionTracker : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISessionRepository _sessionRepository;
        private readonly SessionValidator _sessionValidator;

        private readonly TimeSpan _sessionTimeout;
        private readonly TimeSpan _timeoutCheckInterval;
        private readonly Timer _timer;

        private bool _isDisposed;

        public SessionTracker(ILoggerFactory loggerFactory,
            ISessionRepository sessionRepository,
            IConfiguration configuration,
            SessionValidator sessionValidator)
        {
            _logger = loggerFactory.CreateLogger<SessionTracker>();
            _sessionRepository = sessionRepository;
            _sessionValidator = sessionValidator;
            _sessionTimeout = TimeSpan.Parse(configuration.GetSection("SessionTimeout").Value);
            _timeoutCheckInterval = TimeSpan.Parse(configuration.GetSection("TimeoutCheckInterval").Value);

            _timer = new Timer(t => RemoveTimedOut(), null, _timeoutCheckInterval, _timeoutCheckInterval);
        }

        public Session StartSession(int userId, DateTime timeStamp)
        {
            var testSession = _sessionRepository.GetLastSessionForUser(userId);
            string errorMessage;
            if (_sessionValidator.IsActiveSession(testSession, out errorMessage))
            {
                throw new Exception($"{errorMessage}");
            }

            var createdAt = timeStamp;
            var expiresAt = createdAt + _sessionTimeout;
            var session = new Session(userId, createdAt, expiresAt, SessionStatus.ACTIVE);
            _sessionRepository.Add(session);
            _logger.LogInformation($"Start sessionId:{session.SessionID} for userId:{userId}");
            return session;
        }

        private void RemoveTimedOut()
        {
            var sessionIds = _sessionRepository.GetAllExpired();
            _logger.LogInformation($"Timedout: {string.Join(", ", sessionIds)}");
            foreach (var s in sessionIds)
            {
                CloseSession(s);
            }
        }

        public void CloseSession(int sessionId)
        {
            var session = _sessionRepository.GetSessionById(sessionId);
            session.Status = SessionStatus.FINISHED;
            _sessionRepository.Update(session);
            _logger.LogInformation($"Session {sessionId} is finished");
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _timer.Dispose();
            _isDisposed = true;
        }

        ~SessionTracker()
        {
            Dispose();
        }
    }
}
