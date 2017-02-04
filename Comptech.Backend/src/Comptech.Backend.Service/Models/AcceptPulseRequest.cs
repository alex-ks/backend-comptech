using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models
{
    public class AcceptPulseRequest
    {
        [DataMember(Name = "bpm")]
        public int Pulse { get; set; }
        [DataMember(Name = "timestamp")]
        public DateTime TimeStamp { get; set; }
    }
}
