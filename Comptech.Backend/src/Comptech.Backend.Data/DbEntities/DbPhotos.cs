using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DbEntities
{
    public class DbPhotos
    {
        public int PhotoId { get; set; }
        public int SessionId { get; set; }
        public byte[] Image { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
