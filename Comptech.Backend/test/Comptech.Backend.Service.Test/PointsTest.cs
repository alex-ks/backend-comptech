using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Test
{
    public class PointsTests : GenericEqualityTest<Points>
    {
        public PointsTests()
        {
            a = new Points(new Point(0, 1), new Point(2, 3));
            b = new Points(new Point(0, 1), new Point(2, 3));
            c = new Points(new Point(0, 1), new Point(2, 3));
        }
    }
}
