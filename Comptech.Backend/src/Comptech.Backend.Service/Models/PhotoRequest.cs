using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models
{
    public class PhotoRequest
    {
        [DataMember(Name="image")]
        public string Image { get; set; }

        [DataMember(Name ="timestamp")]
        public DateTime TimeStamp { get; set; }

        public PhotoRequest(string image, DateTime timeStamp)
        {
            Image = image;
            TimeStamp = timeStamp;
        }
    }
}
