using Comptech.Backend.Data.DomainEntities;

namespace Comptech.Backend.Service.Analytics
{
    public interface IAnalyticsClient
    {
        bool RequestRecognitionSession(string modelName, out string sessionUID);

        bool UploadPhoto(byte[] photo, string sessionUID);

        bool TryGetResults(string sessionUID, out RecognitionResults recognitionResults);
    }
}