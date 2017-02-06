using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Comptech.Backend.Data.DbEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlPulseRepository : IPulseRepository
    {
        private ILogger logger;

        public PsqlPulseRepository(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<PsqlPulseRepository>();
        }

        public bool Add(Pulse entity)
        {
            using (logger.BeginScope(nameof(this.Add)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while adding pulse in DB");
                    return false;
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPulse dbPulse = entity.ToDbEntity();
                        context.Pulse.Add(dbPulse);
                        context.SaveChanges();
                        logger.LogDebug("Pulse added successfully");
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e.ToString() + " while adding pulse in DB");
                        return false;
                    }
                }
            }
        }

        public bool Update(Pulse entity)
        {
            using (logger.BeginScope(nameof(this.Update)))
            {
                if (entity == null)
                {
                    logger.LogWarning("ArgumentNullException while updating pulse in DB");
                    return false;
                }
                using (var context = new PsqlContext())
                {
                    try
                    {
                        DbPulse dbPulse = entity.ToDbEntity();
                        context.Pulse.Update(dbPulse);
                        context.SaveChanges();
                        logger.LogDebug("Pulse updated successfully");
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

        public Pulse GetLastPulseInSession(int sessionId)
        {
            using (logger.BeginScope(nameof(this.GetLastPulseInSession)))
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
                        var pulses = from p in context.Pulse
                                     where p.SessionId == sessionId
                                     select p;
                        var pulse = pulses.OrderByDescending(d => d).FirstOrDefault();
                        if (pulse == null)
                        {
                            logger.LogWarning("Pulse is not found");
                            throw new Exception("Pulse of session with that ID is not found");
                        }
                        Pulse result = pulse.ToDomainEntity();
                        logger.LogDebug("Pulse is found");
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
