using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class RecognitionResultsResponse
    {
        [DataMember(Name = "pointLeftUp")]
        public Point TopLeft { get; set; }

        [DataMember(Name = "pointRightDown")]
        public Point BottomRight { get; set;}

        [DataMember(Name = "isValid")]
        public bool IsValid { get; set; }
    }
}