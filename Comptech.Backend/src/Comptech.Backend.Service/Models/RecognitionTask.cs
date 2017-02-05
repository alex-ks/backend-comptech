using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionTask
    {
        [DataMember(Name = "modelname")]
        public string modelName { get; set; }

        [DataMember(Name = "photoId")]
        public int PhotoId { get; set; }
    }
}
