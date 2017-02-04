using Comptech.Backend.Data.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Services.Sessions
{
    public interface ISessionStore
    {

        Session GetSessionById(int sessionId);
        Task<Session>  CreateSessionForUserName(string userName);
        Task<Session> CreateSessionForUserId(string userId);
        void CloseSession(int sessionId);
    }
}
