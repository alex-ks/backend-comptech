using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class Pulse
    {
        public int SessionID { get; set; }
        public int BPM { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
