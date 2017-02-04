using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        IEnumerable<int> GetAllTimedOut(TimeSpan sessionTimeout);
        Session GetLastSessionForUser(int userId);
        Session GetSessionById(int sessionId);
    }
}
