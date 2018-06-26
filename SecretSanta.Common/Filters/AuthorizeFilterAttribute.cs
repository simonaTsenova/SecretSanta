using Autofac.Integration.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SecretSanta.Common.Filters
{
    public class AuthorizeFilterAttribute : ActionFilterAttribute, IAutofacActionFilter
    {
        public AuthorizeFilterAttribute()
        {
        }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            IEnumerable<string> authValues;
            if (!actionContext.Request.Headers.TryGetValues("Authorization", out authValues))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}
