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
        bool IsActive(int sessionId);
        Session GetSessionById(int sessionId);
        void SetLastActive(int sessionId, DateTime utcNow);
        Session GetActualById(int id);
    }
}
