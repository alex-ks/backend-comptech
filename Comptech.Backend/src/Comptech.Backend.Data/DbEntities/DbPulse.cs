﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Data.DbEntities
{
    public class DbPulse
    {
        public int SessionId { get; set; }
        public int Bpm { get; set; }
        public DateTime timestamp { get; set; }
    }
}
