using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Comptech.Backend.Data.Test")]

namespace Comptech.Backend.Data.DbEntities
{
     class DbSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; }

        public DbUser User { get; set; }
    }
}
