using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service
{
    public class SessionValidator
    {
        private readonly ILogger logger;

        public SessionValidator(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<SessionValidator>();
        }

        public bool IsNullSession(Session session, out string errorMessage)
        {
            if (session == null)
            {
                logger.LogInformation("invalid arguments {0}", nameof(session));
                errorMessage = "invalid arguments";
                return true;
            }
            errorMessage = null;
            return false;
        }

        public bool IsActiveSession(Session session, out string errorMessage)
        {
            if (session.Status == SessionStatus.ACTIVE)
            {
                logger.LogInformation("Session of user with sessionId {0} has already been started", session.SessionID);
                errorMessage = "Session with user has already been started";
                return true;
            }
            errorMessage = null;
            return false;
        }
        public bool IsFinishedSession(Session session, out string errorMessage)
        {
            if (session.Status == SessionStatus.FINISHED)
            {
                logger.LogInformation("Session of user with sessionId {0} is finished", session.SessionID);
                errorMessage = "Session of user is finished";
                return true;
            }
            errorMessage = null;
            return false;
        }

        public bool IsNullOrFinishedSession(Session session, out string errorMessage)
        {
            return IsNullSession(session, out errorMessage) || IsFinishedSession(session, out errorMessage);
        }
    }
}
