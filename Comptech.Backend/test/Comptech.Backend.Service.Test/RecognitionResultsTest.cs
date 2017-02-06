using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Test
{
    public class RecognitionResultsTest : GenericEqualityTest<RecognitionResults>
    {
        public RecognitionResultsTest()
        {
            a = new RecognitionResults(true, new Points(new Point(0, 1), new Point(2, 3)), 2);
            b = new RecognitionResults(true, new Points(new Point(0, 1), new Point(2, 3)), 2);
            c = new RecognitionResults(true, new Points(new Point(0, 1), new Point(2, 3)), 2);
        }
    }
}
