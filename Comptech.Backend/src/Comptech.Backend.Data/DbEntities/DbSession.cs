using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DbEntities
{
     class DbSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime Start { get; set; }
        public string Status { get; set; }

        public List<DbPhoto> Photos { get; set; }
        public List<DbPulse> Pulses { get; set; }
        public DbUser User { get; set; }

    }
}
