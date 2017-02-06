using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.Repositories.Psql;
using Comptech.Backend.Data.DomainEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.DirtyTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = MakeLoggerFactory();

            var sessionRepo = new PsqlSessionRepository(factory);
            var photoRepo = new PsqlPhotoRepository(factory);
            var pulseRepo = new PsqlPulseRepository(factory);
            var resultRepo = new PsqlRecognitionResultsRepository(factory);

            var session = new Session
            {
                SessionID = 12,
                Start = DateTime.Now,
                Status = SessionStatus.ACTIVE,
                UserID = 1
            };

            var photo = new Photo
            {
                PhotoID = 1000,
                Image = new byte[] { },
                SessionID = 12,
                TimeStamp = DateTime.Now
            };

            var pulse = new Pulse
            {
                SessionID = 12,
                BPM = 500,
                TimeStamp = DateTime.Now
            };

            var result = new RecognitionResult
            {
                PhotoID = 10001,
                IsValid = false,
                Coords = null
            };

            if(!sessionRepo.Add(session))
                throw new ArgumentException("Session");

            if (!pulseRepo.Add(pulse))
                throw new ArgumentException("Pulse");

            if (!photoRepo.Add(photo))
                throw new ArgumentException("Photo");

            if (!resultRepo.Add(result))
                throw new ArgumentException("Result");
        }

        private static ILoggerFactory MakeLoggerFactory()
        {
            return new LoggerFactory().AddConsole();
        }
    }
}
