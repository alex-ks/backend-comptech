using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Photo
    {
        public int PhotoID { get; set; }
        public int SessionID { get; set; }
        public byte[] Image { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
