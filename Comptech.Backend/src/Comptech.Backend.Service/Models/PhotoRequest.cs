using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Models{ 
    public class PhotoRequest
    {
        [DataMember(Name = "valid")]
        public string Valid { get; set; }

        [DataMember(Name = "photo")]
        public string Photo { get; set; }

        [DataMember(Name = "cordinates")]
        public string Cordinates { get; set;}
    }
}