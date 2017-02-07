using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionResultsResponse
    {
        [JsonProperty(PropertyName = "pointLeftUp")]
        [DataMember(Name = "pointLeftUp")]
        public Point TopLeft { get; set; }

        [JsonProperty(PropertyName = "pointRightDown")]
        [DataMember(Name = "pointRightDown")]
        public Point BottomRight { get; set;}

        [JsonProperty(PropertyName = "isValid")]
        [DataMember(Name = "isValid")]
        public bool IsValid { get; set; }
    }
}