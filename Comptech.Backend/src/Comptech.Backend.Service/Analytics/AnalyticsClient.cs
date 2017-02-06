using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Service.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Analytics
{
    public class AnalyticsClient : IAnalyticsClient
    {
        private Uri analyticsServerUri;

        public AnalyticsClient (string analyticsServerUri)
        {
            this.analyticsServerUri = new Uri(analyticsServerUri);
        }

        //throws an exception if the status code falls outside the range 200–299
        public async Task<string> RequestRecognitionSession(string modelName)
        {
            using (var client = new HttpClient())
            {
                    client.BaseAddress = analyticsServerUri;

                    var response = await client.PostAsJsonAsync("rest/request_recognition", new { modelName = modelName });
                    response.EnsureSuccessStatusCode();

                    var sessionUid = await response.Content.ReadAsAsync<string>();

                    return sessionUid;
            }
        }

        //throws an exception if the status code falls outside the range 200–299
        public async void UploadPhoto(byte[] photo, string sessionUid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = analyticsServerUri;
                
                var response = 
                    await client.PostAsJsonAsync("rest/start_recognition", new { photo = photo, sessionUID = sessionUid });

                response.EnsureSuccessStatusCode();                
            }
        }

        //throws an exception if the status code falls outside the range 200–299
        public async Task<RecognitionResults> TryGetResults(string sessionUid, int photoId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = analyticsServerUri;
                
                var response = await client.GetAsync("rest/result");
                response.EnsureSuccessStatusCode();

                var responceResults = await response.Content.ReadAsAsync<RecogntionResultsResponse>();
                var recognitionResults = new RecognitionResults();
            
                recognitionResults.Coords = new Points();

                recognitionResults.Coords.BottomRight = new Comptech.Backend.Data.DomainEntities.Point();
                recognitionResults.Coords.TopLeft = new Comptech.Backend.Data.DomainEntities.Point();

                recognitionResults.Coords.BottomRight.X = responceResults.BottomRight.X;
                recognitionResults.Coords.BottomRight.Y = responceResults.BottomRight.Y;
                recognitionResults.Coords.TopLeft.X = responceResults.TopLeft.X;
                recognitionResults.Coords.TopLeft.Y = responceResults.TopLeft.Y;

                recognitionResults.IsValid = responceResults.IsValid;
                recognitionResults.PhotoID = photoId;

                return recognitionResults;
            }
        }
    }
}