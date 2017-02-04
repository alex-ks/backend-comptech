using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class FinishRequest
    {
        [DataMember(Name = "sessionId")]
        public int? SessionId { get; set; }
    }
} 
