using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.DbEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlRecognitionResultsRepository : IRecognitionResultsRepository
    {
        private ILogger logger;
        public PsqlRecognitionResultsRepository(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<PsqlRecognitionResultsRepository>();
        }


        public bool Add(RecognitionResult entity)
        {
            using (logger.BeginScope(nameof(this.Add)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while adding Recognition Results in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbResult recognitionResult = entity.ToDbEntity();
                        context.Results.Add(recognitionResult);
                        context.SaveChanges();
                        logger.LogDebug("Recognition Results added successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while adding Recognition Result in DB");
                        return false;
                    }
                }
            }
        }

        public bool Update(RecognitionResult entity)
        {
            using (logger.BeginScope(nameof(this.Update)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while updating Recognition Result in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbResult dbResult = entity.ToDbEntity();
                        context.Results.Update(dbResult);
                        context.SaveChanges();
                        logger.LogDebug("RecognitionResult updated successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while updating pulse in DB");
                        return false;
                    }
                }
            }
        }

        public RecognitionResult GetRecognitionResultsByPhotoId(int photoId)
        {
            if (photoId <= 0)
            {
                logger.LogWarning("Ivalid Photo ID");
                throw new ArgumentException("Photo ID cannot be equal zero or less");
            }

            using (var context = new PsqlContext())
            {
                try
                {
                    var recResult = context.Results.
                        Where(p => p.PhotoId == photoId).FirstOrDefault();
                    if (recResult == null)
                    {
                        logger.LogWarning("RecognitionResults is not found");
                        throw new Exception("RecognitionResults with that Photo ID is not found");
                    }
                    RecognitionResult result = recResult.ToDomainEntity();
                    logger.LogDebug("RecognitionResults is found");
                    return result;
                }
                catch
                {
                    logger.LogWarning("Error");
                    throw;
                }
            }
        }
    }
    }
}
