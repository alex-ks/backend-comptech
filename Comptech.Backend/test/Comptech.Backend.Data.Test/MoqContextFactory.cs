﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;

namespace Comptech.Backend.Data
{
    public class MoqContextFactory : IContextFactory
    {
        public void CreateContext()
        {
            PsqlContext context =  new PsqlContext();
        }
    }
}
