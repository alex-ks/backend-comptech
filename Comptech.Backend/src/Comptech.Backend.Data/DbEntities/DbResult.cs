using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Comptech.Backend.Data.Test")]

namespace Comptech.Backend.Data.DbEntities
{
     class DbResult
    {
        public bool IsValid { get; set; }
        public Nullable<int> X1 { get; set; }
        public Nullable<int> Y1 { get; set; }
        public Nullable<int> X2 { get; set; }
        public Nullable<int> Y2 { get; set; }
        public int PhotoId { get; set; }

        public DbPhoto Photo { get; set; }

    }
}
