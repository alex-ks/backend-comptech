using System.Runtime.Serialization;

namespace Comptech.Backend.Service.Models
{
    public class Point
    {
        [DataMember(Name = "x")]
        public int X{ get; set; }

        [DataMember(Name = "y")]
        public int Y{ get; set; }

    }
}