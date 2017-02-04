using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class FinishRequest
    {
        [DataMember(Name = "finished")]
        public int Finished { get; set; }

        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }
} 
