using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.DbEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Data.Repositories.Psql
{ 
    public class PsqlPhotoRepository : IPhotoRepository
    {
        private ILogger logger;

        public PsqlPhotoRepository(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<PsqlPhotoRepository>();
        }


        public bool Add(Photo entity)
        {
            using (logger.BeginScope(nameof(this.Add)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while adding photo in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPhoto dbPhoto = entity.ToDbEntity();
                        context.Photos.Add(dbPhoto);
                        context.SaveChanges();
                        logger.LogDebug("Photo added successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while adding photo in DB");
                        return false;
                    }
                }
            }
        }

        public bool Update(Photo entity)
        {
            using (logger.BeginScope(nameof(this.Update)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while updating photo in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPhoto dbPhoto = entity.ToDbEntity();
                        context.Photos.Update(dbPhoto);
                        context.SaveChanges();
                        logger.LogDebug("Photo updated successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while updating photo in DB");
                        return false;
                    }
                }
            }
        }

        public int GetLastPhotoIdInSession(int sessionId)
        {
            using (logger.BeginScope(nameof(this.GetLastPhotoIdInSession)))
            {
                if (sessionId <= 0)
                {
                    logger.LogWarning("Ivalid Session ID");
                    throw new ArgumentException("Session ID cannot be equal zero or less");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        var photo = context.Photos.Find(sessionId);
                        if (photo == null)
                        {
                            logger.LogWarning("Photo is not found");
                            throw new Exception("Photo of session with that ID is not found");
                        }
                        logger.LogDebug("PhotoID is found");
                        return photo.PhotoId;
                    }
                    catch
                    {
                        logger.LogWarning("Error");
                        throw;
                    }
                }
            }
        }

        public Photo GetLastPhotoInSession(int sessionId)
        {
            using (logger.BeginScope(nameof(this.GetLastPhotoIdInSession)))
            {
                if (sessionId <= 0)
                {
                    logger.LogWarning("Ivalid Session ID");
                    throw new ArgumentException("Session ID cannot be equal zero or less");
                }

                using (var context = new PsqlContext())
                {
                    try
                    {
                        var photos = from p in context.Photos
                                     where p.SessionId == sessionId
                                     select p;
                        var photo = photos.OrderByDescending(d => d.Timestamp).FirstOrDefault();
                        if (photo == null)
                        {
                            logger.LogWarning("Photo is not found");
                            throw new Exception("Photo of session with that ID is not found");
                        }
                        Photo result = photo.ToDomainEntity();
                        logger.LogDebug("Photo is found");
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
