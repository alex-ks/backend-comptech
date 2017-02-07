using Comptech.Backend.Service.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Test
{
    public class AnalyticsClientMock : IAnalyticsClient
    {
        public Task<string> RequestRecognitionSession(string modelName)
        {
            return Task.Run(() => { return "aaa"; });
        }

        public Task<RecognitionResults> TryGetResults(string sessionUID, int photoId)
        {
            Points points = new Points(new Point(0, 0), new Point(10, 10));
            RecognitionResults results = new RecognitionResults(true, points, 2);
            return Task.Run(() => { return results; });
        }

        public Task UploadPhoto(byte[] photo, string sessionUID)
        {
            return Task.Run(() => { return; });
        }
    }
}
