using Comptech.Backend.Data.DomainEntities;
using System;

namespace Comptech.Backend.Service.Test
{
    public class PhotoTests : GenericEqualityTest<Photo>
    {
        public PhotoTests()
        {
            a = new Photo(0, new byte[] { 0x20 }, new DateTime());
            b = new Photo(0, new byte[] { 0x20 }, new DateTime());
            c = new Photo(0, new byte[] { 0x20 }, new DateTime());
        }
    }
}