using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Session
    {
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime? ClosedAt { get; set; }

        [NotMapped]
        public bool IsActive => ExpiresAt > DateTime.Now && !ClosedAt.HasValue;
    
    }

}
