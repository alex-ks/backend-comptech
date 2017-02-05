using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlSessionRepository : ISessionRepository
    {
        public bool Add(Session entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Session entity)
        {
            throw new NotImplementedException();
        }

        public Session GetLastSessionForUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
