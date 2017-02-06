using Comptech.Backend.Data.DomainEntities;
using System;

namespace Comptech.Backend.Service.Test
{
    public class SessionTest : GenericEqualityTest<Session>
    {
        public SessionTest()
        {
            a = new Session(1, new DateTime(), new DateTime().AddDays(1), SessionStatus.ACTIVE);
            b = new Session(1, new DateTime(), new DateTime().AddDays(1), SessionStatus.ACTIVE);
            c = new Session(1, new DateTime(), new DateTime().AddDays(1), SessionStatus.ACTIVE);
        }
    }
}
