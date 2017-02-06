﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comptech.Backend.Data.DomainEntities;
using Microsoft.Extensions.Logging;

namespace Comptech.Backend.Data.Repositories.Psql
{
    public class PsqlSessionRepository : ISessionRepository
    {
        private ILogger _logger;

        public PsqlSessionRepository(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<PsqlSessionRepository>();
        }

        public bool Add(Session entity)
        {
            using (_logger.BeginScope(nameof(Add)))
            {
                if(entity == null)
                {
                    _logger.LogWarning("Entity is null, Add failed");
                    return false;
                }

                var dbEntity = entity.ToDbEntity();

                using (var context = new PsqlContext())
                {
                    context.Sessions.Add(dbEntity);

                    try
                    {
                        context.SaveChanges();                        
                    }
                    catch(Exception e)
                    {
                        _logger.LogWarning($"Exception from {e.Source}:{e.Message}, Add failed");
                        return false;
                    }
                }

                _logger.LogDebug("Add succeeded");
                return true;
            }
        }

        public bool Update(Session entity)
        {
            using (_logger.BeginScope(nameof(Update)))
            {
                if(entity == null)
                {
                    _logger.LogWarning("Entity is null, Update failed");
                    return false;
                }

                var dbEntity = entity.ToDbEntity();

                using (var context = new PsqlContext())
                {
                    var oldEntity = (from session in context.Sessions
                                     where session.SessionId == dbEntity.SessionId
                                     select session).FirstOrDefault();

                    if(oldEntity == null)
                    {
                        _logger.LogWarning("Entity to update was not found, Update failed");
                        return false;
                    }

                    oldEntity.Photos = dbEntity.Photos;
                    oldEntity.Pulses = dbEntity.Pulses;
                    oldEntity.Start = dbEntity.Start;
                    oldEntity.Status = dbEntity.Status;

                    try
                    {
                        context.SaveChanges();
                    }
                    catch(Exception e)
                    {
                        _logger.LogWarning($"Exception from {e.Source}:{e.Message}, Update failed");
                        return false;
                    }                                       
                }

                _logger.LogDebug("Update succeeded");
                return true;
            }
        }

        public Session GetLastSessionForUser(int userId)
        {
            using (_logger.BeginScope(nameof(GetLastSessionForUser)))
            {
                using (var context = new PsqlContext())
                {
                    var result = (from session in context.Sessions
                                  where session.UserId == userId
                                  select session).FirstOrDefault();

                    if(result == null)
                    {
                        _logger.LogWarning($"No sessions found for user id {userId}, null session returned");
                        return null;
                    }

                    _logger.LogDebug($"Last session successfully found for user id {userId}");

                    return result.ToDomainEntity();
                }
            }
        }
    }
}
