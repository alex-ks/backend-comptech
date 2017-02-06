using Comptech.Backend.Data.DomainEntities;
using System;

namespace Comptech.Backend.Service.Test
{
    public class PulseTest : GenericEqualityTest<Pulse>
    {
        public PulseTest()
        {
            a = new Pulse(0, 1, new DateTime());
            b = new Pulse(0, 1, new DateTime());
            c = new Pulse(0, 1, new DateTime());
        }
    }
}