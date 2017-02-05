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

        private readonly TimeSpan _sessionTimeout;
        private readonly TimeSpan _timeoutCheckInterval;
        private readonly Timer _timer;

        private bool _isDisposed;

        public SessionTracker(ILoggerFactory loggerFactory,
            ISessionRepository sessionRepository,
            IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<SessionTracker>();
            _sessionRepository = sessionRepository;
            _sessionTimeout = TimeSpan.Parse(configuration.GetSection("SessionTimeout").Value);
            _timeoutCheckInterval = TimeSpan.Parse(configuration.GetSection("TimeoutCheckInterval").Value);

            _timer = new Timer(t => RemoveTimedOut(), new object(), _timeoutCheckInterval, _timeoutCheckInterval);
        }

        public Session StartSession(int userId)
        {
            var testSession = _sessionRepository.GetLastSessionForUser(userId);
            if (testSession.Status.Equals(SessionStatus.ACTIVE))
            {
                throw new Exception($"Session {testSession.SessionID} has already been started.");
            }

            try
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
                _logger.LogInformation($"Start sessionId:{session.SessionID} for userId:{userId}");
                return session;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
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

        public void SetLastActive(int sessionId)
        {
            var session = _sessionRepository.GetSessionById(sessionId);
            session.LastActive = DateTime.UtcNow;
            _logger.LogInformation($"Session {sessionId} is active");
            _sessionRepository.Update(session);
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
