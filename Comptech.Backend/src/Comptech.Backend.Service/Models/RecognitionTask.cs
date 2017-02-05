using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionTask
    {
        [DataMember(Name = "photo")]
        public string Photo { get; set; }
        [DataMember(Name = "sessionUID")]
        public string SessionUid { get; set; }
    }
}
