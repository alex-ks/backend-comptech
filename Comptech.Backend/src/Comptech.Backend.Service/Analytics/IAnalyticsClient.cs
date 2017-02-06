using Comptech.Backend.Data.DomainEntities;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Analytics
{
    public interface IAnalyticsClient
    {
        Task<string> RequestRecognitionSession(string modelName);

        Task UploadPhoto(byte[] photo, string sessionUID);

        Task<RecognitionResults> TryGetResults(string sessionUID, int photoId);
    }
}