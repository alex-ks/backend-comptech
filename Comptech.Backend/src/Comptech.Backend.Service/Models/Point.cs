using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Comptech.Backend.Service.Models
{
    public class Point
    {
        [JsonProperty(PropertyName = "x")]
        [DataMember(Name = "x")]
        public int X{ get; set; }

        [JsonProperty(PropertyName = "y")]
        [DataMember(Name = "y")]
        public int Y{ get; set; }

    }
}