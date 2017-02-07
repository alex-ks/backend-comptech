using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Analytics
{
    [Authorize]
    public class AnalyticsAgent : IDisposable
    {
        private readonly RecognitionTaskQueue queue;
        private readonly IAnalyticsClient analyticsClient;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly IPhotoRepository photoRepository;
        private CancellationTokenSource cancellationTokenSource;

        public AnalyticsAgent(RecognitionTaskQueue queue,
                       IAnalyticsClient analyticsClient,
                       ILoggerFactory loggerFactory,
                       IPhotoRepository photoRepository,
                       IConfiguration configuration)
        {
            this.queue = queue;
            this.analyticsClient = analyticsClient;
            this.photoRepository = photoRepository;
            this.configuration = configuration;
            logger = loggerFactory.CreateLogger<AnalyticsAgent>();
        }

        public void PollingTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => Run(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public void CancelPolling()
        {
            cancellationTokenSource.Cancel();
        }


        public void Dispose()
        {
            CancelPolling();
        }

        private async void Run(CancellationToken token)
        {
            bool isInterrupted = false;
            int pollingTimoutBetweenTasks = int.Parse(
                configuration.GetSection("AnalyticsPollingTimeoutBetweenTasks").Value);
            while (!isInterrupted)
            {
                try
                {
                    RecognitionTask recognitionTask = queue.Dequeue();
                    await RunTaskForRecognition(recognitionTask);
                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException exception)
                {
                    logger.LogError("Tasks is cancelled");
                    isInterrupted = true;
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                }
                Thread.Sleep(pollingTimoutBetweenTasks);
            }
        }

        private async Task RunTaskForRecognition(RecognitionTask recognitionTask)
        {
            string recognitionSessionUID = null;
            int pollingTimeout = int.Parse(configuration.GetSection("AnalyticsPollingTimeout").Value);
            while (recognitionSessionUID == null)
            {
                try
                {
                    recognitionSessionUID = await analyticsClient.RequestRecognitionSession(recognitionTask.ModelName);
                    if (recognitionSessionUID != null)
                        break;
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                }
                Thread.Sleep(pollingTimeout);
            }
            
            Photo photo = photoRepository.GetPhotoById(recognitionTask.PhotoId);

            bool isPhotoUploaded = false;
            int photoUploadTryCount = int.Parse(configuration.GetSection("PhotoUploadTryCount").Value);
            for (int i = 0; i < photoUploadTryCount; i++)
            {
                try
                {
                    await analyticsClient.UploadPhoto(photo.Image, recognitionSessionUID);
                    isPhotoUploaded = true;
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                    Thread.Sleep(pollingTimeout);
                }
            }
            if (!isPhotoUploaded)
            {
                queue.Enqueue(recognitionTask);
                return;
            }

            RecognitionResults recognitionResults = null;
            while (recognitionResults == null)
            {
                try
                {
                    recognitionResults = await analyticsClient.TryGetResults(recognitionSessionUID, recognitionTask.PhotoId);
                    if (recognitionResults != null)
                        break;
                }
                catch (Exception exception)
                {
                    logger.LogError("Exception caught: {0}, {1}", exception.Message, exception.StackTrace);
                }
                Thread.Sleep(pollingTimeout);
            }
        }
    }
}
