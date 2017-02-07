using System;
using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionTask
    {
        [DataMember(Name = "modelName")]
        public string ModelName { get; set; }
        [DataMember(Name = "photoId")]
        public int PhotoId { get; set; }

        public RecognitionTask(string modelName, int photoId)
        {
            ModelName = modelName;
            PhotoId = photoId;
        }
    }
}
