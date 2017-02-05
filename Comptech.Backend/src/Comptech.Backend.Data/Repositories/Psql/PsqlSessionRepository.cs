using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.DbEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlSessionRepository : ISessionRepository
    {
        private ILogger logger;

        public PsqlSessionRepository(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<PsqlSessionRepository>();
        }

        public bool Add(Session entity)
        {
            using (logger.BeginScope(nameof(this.Add)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while adding session in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbSession dbSession = entity.ToDbEntity();
                        context.Sessions.Add(dbSession);
                        context.SaveChanges();
                        logger.LogDebug("Session added successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while adding session in DB");
                        return false;
                    }
                }
            }
        }

        public bool Update(Session entity)
        {
            using (logger.BeginScope(nameof(this.Update)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while updating session in DB");
                    throw new ArgumentNullException("Entity cannot be null");
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbSession dbSession = entity.ToDbEntity();
                        context.Sessions.Update(dbSession);
                        context.SaveChanges();
                        logger.LogDebug("Session updated successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while updating session in DB");
                        return false;
                    }
                }
            }
        }

        public Session GetLastSessionForUser(int userId)
        {
            using (logger.BeginScope(nameof(this.GetLastSessionForUser)))
            {
                if (userId <= 0)
                {
                    logger.LogWarning("Ivalid User ID");
                    throw new ArgumentException("User ID cannot be equal zero or less");
                }

                using (var context = new PsqlContext())
                {
                    try
                    {
                        var sessions = from s in context.Sessions
                                     where s.UserId == userId
                                     select s;
                        var session = sessions.OrderByDescending(d => d.Start).FirstOrDefault();
                        if (session == null)
                        {
                            logger.LogWarning("Session is not found");
                            throw new Exception("User sessions with that ID is not found");
                        }
                        Session result = session.ToDomainEntity();
                        logger.LogDebug("Session is found");
                        return result;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning($"Exception: {e.StackTrace}");
                        throw;
                    }
                }
            }
        }
    }
}
