using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlPulseRepository : IPulseRepository
    {
        public bool Add(Pulse entity)
        {
            throw new NotImplementedException();
        }

        public Pulse GetLastPulseInSession(int sessionId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Pulse entity)
        {
            throw new NotImplementedException();
        }
    }
}
