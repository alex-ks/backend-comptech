﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Comptech.Backend.Data.Test")]

namespace Comptech.Backend.Data.DbEntities
{
     class DbPulse
    {
        public int SessionId { get; set; }
        public int Bpm { get; set; }
        public DateTime timestamp { get; set; }

        public DbSession Session { get; set; }

    }
}
