using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Comptech.Backend.Data.Repositories;
using Comptech.Backend.Service.Services.Sessions;
using Microsoft.AspNetCore.Http;

namespace Comptech.Backend.Service.Core.Sessions
{
    public class SessionFilter : IActionFilter
    {
        private readonly ISessionStore _sessionStore;
        private readonly ISessionRepository _sessionRepo;

        public SessionFilter(ISessionStore sessionStore, ISessionRepository sessionRepo)
        {
            _sessionStore = sessionStore;
            _sessionRepo = sessionRepo;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HasValidateSessionAttribute(context.ActionDescriptor)) return;
            
            if (!context.HttpContext.Request.Headers.TryGetSessionId())
            {
                throw new InvalidOperationException($"No sessionId in the claims");
            }

            if (!_sessionRepo.IsActive(sessionId))
            {
                throw new InvalidOperationException($"Session with id {sessionId} is not active");
            }
            _sessionRepo.SetLastActive(sessionId, DateTime.UtcNow);
        }

        private bool HasValidateSessionAttribute(ActionDescriptor actionDescriptor)
        {
            return (((actionDescriptor as ControllerActionDescriptor).MethodInfo.
                GetCustomAttributes<ValidateSessionAttribute>().FirstOrDefault()) != null) ? true : false;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    public static class SessionExtensions
    {
        public static bool TryGetSessionId(this IHeaderDictionary headers)
        {
            return headers.ContainsKey("sessionId");
        }
    }
}
