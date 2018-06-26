using Autofac.Integration.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace SecretSanta.Common.Filters
{
    public class CustomErrorFilterAttribute : ExceptionFilterAttribute, IAutofacExceptionFilter
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var responseMessage = "Ooops, an exception was thrown; ";
            if (actionExecutedContext.Exception.InnerException == null)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.
                    CreateResponse(HttpStatusCode.InternalServerError, responseMessage + actionExecutedContext.Exception.Message);
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request
                    .CreateResponse(HttpStatusCode.InternalServerError, responseMessage + actionExecutedContext.Exception.InnerException.Message);
            }

            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}
