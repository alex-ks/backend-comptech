using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DbEntities;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Data
{
    static class Converter
    {
        public static DbPhotos ToDbEntity(this Photo entity)
        {
            throw new NotImplementedException();
        }

        public static DbPulse ToDbEntity(this Pulse entity)
        {
            throw new NotImplementedException();
        }

        public static DbSessions ToDbEntity(this Session entity)
        {
            throw new NotImplementedException();
        }
        public static DbResults ToDbEntity(this RecognitionResults entity)
        {
            throw new NotImplementedException();
        }
         public static Photo ToDomainEntity(this DbPhotos entity)
        {
            throw new NotImplementedException();
        }
        public static Pulse ToDomainEntity(this DbPulse entity)
        {
            throw new NotImplementedException();
        }
        public static Session ToDomainEntity(this DbSessions entity)
        {
            throw new NotImplementedException();
        }
        public static RecognitionResults ToDomainEntity(this DbResults entity)
        {
            throw new NotImplementedException();
        }
    }
}
