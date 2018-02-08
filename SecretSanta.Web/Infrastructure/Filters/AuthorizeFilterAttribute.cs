using Autofac.Integration.WebApi;
using SecretSanta.Services.Contracts;
using System.Web.Http.Filters;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System;
using System.Net.Http;

namespace SecretSanta.Web.Infrastructure.Filters
{
    public class AuthorizeFilterAttribute : ActionFilterAttribute, IAutofacActionFilter
    {
        private readonly ISessionService sessionService;

        public AuthorizeFilterAttribute(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            IEnumerable<string> authValues;
            if (!actionContext.Request.Headers.TryGetValues("Authorization", out authValues))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var userSession = this.sessionService.GetCurrentSession();
            if(userSession == null)
            {
                // User does not have current session with this token
                actionContext.Response = actionContext.ControllerContext.Request
                    .CreateErrorResponse(HttpStatusCode.Unauthorized, "Session token is not valid.");
                return;
            }

            if(userSession.ExpiresOn < DateTime.Now)
            {
                // User does have a session but it has expired
                this.sessionService.DeleteExpiredSessions();
                actionContext.Response = actionContext.ControllerContext.Request
                    .CreateErrorResponse(HttpStatusCode.Unauthorized, "Session token expried.");
                return;
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}