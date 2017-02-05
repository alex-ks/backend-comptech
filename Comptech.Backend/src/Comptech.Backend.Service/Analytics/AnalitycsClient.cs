using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Analytics
{
    public class AnalyticsAgent : IAnalyticsClient
    {
        public bool RequestRecognitionSession(string modelName, out string sessionUID)
        {
            sessionUID = null;
            return false;
        }

        public bool UploadPhoto(byte[] photo, string sessionUID)
        {
            return false;
        }

        public bool TryGetResults(string sessionUID, out RecognitionResults recognitionResults)
        {
            recognitionResults = null;
            return false;
        }
    }
}