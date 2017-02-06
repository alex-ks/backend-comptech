using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Test
{
    public class PointTest : GenericEqualityTest<Point>
    {
        public PointTest(){
            a = new Point(0,1);
            b = new Point(0,1);
            c = new Point(0,1);
        }
    }
}
