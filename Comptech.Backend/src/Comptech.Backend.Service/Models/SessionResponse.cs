using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models
{
    public class SessionResponse
    {
        [DataMember(Name = "SessionId")]
        public int SessionId { get; set; }
    }
}
