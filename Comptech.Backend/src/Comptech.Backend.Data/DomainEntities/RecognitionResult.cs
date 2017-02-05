using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DomainEntities
{
    public class RecognitionResult
    {
        public bool IsValid { get; set; }
        public Points Coords { get; set; }
        public int PhotoID { get; set; }

    }

}
