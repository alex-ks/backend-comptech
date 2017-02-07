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
        private ILogger _logger;

        public PsqlPhotoRepository(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PsqlPhotoRepository>();
        }

        public bool Add(Photo entity)
        {
            using (_logger.BeginScope(nameof(Add)))
            {
                if (entity == null)
                {
                    _logger.LogWarning("ArgumentNullException while adding photo in DB");
                    return false;
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPhoto dbPhoto = entity.ToDbEntity();
                        context.Photos.Add(dbPhoto);
                        context.SaveChanges();
                        _logger.LogDebug("Photo added successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e.ToString() + " while adding photo in DB");
                        return false;
                    }
                }
            }
        }

        public bool Update(Photo entity)
        {
            using (_logger.BeginScope(nameof(Update)))
            {
                if (entity == null)
                {
                    _logger.LogWarning("ArgumentNullException while updating photo in DB");
                    return false;
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPhoto dbPhoto = entity.ToDbEntity();
                        context.Photos.Update(dbPhoto);
                        context.SaveChanges();
                        _logger.LogDebug("Photo updated successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e.ToString() + " while updating photo in DB");
                        return false;
                    }
                }
            }
        }

        public int GetLastPhotoIdInSession(int sessionId)
        {
            using (_logger.BeginScope(nameof(GetLastPhotoIdInSession)))
            {
                if (sessionId <= 0)
                {
                    _logger.LogWarning("Ivalid Session ID");
                    throw new ArgumentException("Session ID cannot be equal zero or less");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        var photo = context.Photos.Find(sessionId);
                        if (photo == null)
                        {
                            _logger.LogWarning("Photo is not found");
                            throw new Exception("Photo of session with that ID is not found");
                        }
                        _logger.LogDebug("PhotoID is found");
                        return photo.PhotoId;
                    }
                    catch
                    {
                        _logger.LogWarning("Error");
                        throw;
                    }
                }
            }
        }

        public Photo GetLastPhotoInSession(int sessionId)
        {
            using (_logger.BeginScope(nameof(GetLastPhotoIdInSession)))
            {
                if (sessionId <= 0)
                {
                    _logger.LogWarning("Ivalid Session ID");
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
                            _logger.LogWarning("Photo is not found");
                            throw new Exception("Photo of session with that ID is not found");
                        }
                        Photo result = photo.ToDomainEntity();
                        _logger.LogDebug("Photo is found");
                        return result;
                    }
                    catch
                    {
                        _logger.LogWarning("Error");
                        throw;
                    }
                }
            }
        }

        public Photo GetPhotoById(int photoId)
        {
            using (_logger.BeginScope(nameof(GetPhotoById)))
            {
                using (var context = new PsqlContext())
                {
                    var result = (from p in context.Photos
                                  where p.PhotoId == photoId
                                  select p).FirstOrDefault();

                    return result.ToDomainEntity();
                }
            }
        }
    }
}
