using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DbEntities
{
     class DbPhoto
    {
        public int PhotoId { get; set; }
        public int SessionId { get; set; }
        public byte[] Image { get; set; }
        public DateTime Timestamp { get; set; }

        public DbSession DbSession { get; set; };
    }
}
