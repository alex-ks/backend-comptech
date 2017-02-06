using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Service.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

                    var response = 
                        await client.PostAsync("rest/request_recognition",
                        new StringContent(JsonConvert.SerializeObject(new { modelName = modelName })));
                    
                    if (response.StatusCode.Equals(HttpStatusCode.Conflict)) return null;
                
                    if (!response.StatusCode.Equals(HttpStatusCode.OK)) throw new HttpRequestException();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RecognitionRequest>(stringResponse).SessionUid;
            }
        }

        //throws an exception if the status code falls outside the range 200–299
        public async Task UploadPhoto(byte[] photo, string sessionUid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = analyticsServerUri;
                
                var response = 
                    await client.PostAsync("rest/start_recognition", 
                    new StringContent(JsonConvert.SerializeObject(
                        new 
                        { 
                            photo = System.Convert.ToBase64String(photo), 
                            sessionUID = sessionUid
                        }
                    )));

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
                
                if (response.StatusCode.Equals(HttpStatusCode.Conflict)) return null;

                if (!response.StatusCode.Equals(HttpStatusCode.OK)) throw new HttpRequestException();

                var stringResponse = await response.Content.ReadAsStringAsync();
                var responseResults = JsonConvert.DeserializeObject<RecognitionResultsResponse>(stringResponse);
                
                var recognitionResults = new RecognitionResults(
                    responseResults.IsValid,
                    new Points (
                        new Comptech.Backend.Data.DomainEntities.Point(
                            responseResults.TopLeft.X,
                            responseResults.TopLeft.Y
                        ),
                        new Comptech.Backend.Data.DomainEntities.Point(
                            responseResults.BottomRight.X,
                            responseResults.BottomRight.Y                            
                        )
                    ),
                    photoId
                );
                return recognitionResults;
            }
        }
    }
}