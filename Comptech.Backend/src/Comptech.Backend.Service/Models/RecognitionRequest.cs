using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionRequest
    {
        [DataMember(Name = "sessionUID")]
        public string SessionUid { get; set; }
    }
}