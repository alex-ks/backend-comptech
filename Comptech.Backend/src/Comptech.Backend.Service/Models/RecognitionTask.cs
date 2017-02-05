using System;
using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionTask
    {
        [DataMember(Name = "modelName")]
        public string ModelName { get; set; }
        [DataMember(Name = "photo")]
        public string Photo { get; set; }
    }
}
