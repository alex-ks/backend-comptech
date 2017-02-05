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
        public DateTime TimeStamp { get; set; }

        public DbSession Session { get; set; }
        public DbResult Result { get; set; }
    }
}
