//using Autofac.Integration.WebApi;
//using System.Web.Http.Filters;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Net.Http;
//using System.Net;

//namespace SecretSanta.Web.Common.Filters
//{
//    public class CustomErrorFilterAttribute : ExceptionFilterAttribute, IAutofacExceptionFilter
//    {
//        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
//        {
//            var responseMessage = "Ooops, an exception was thrown; ";
//            if(actionExecutedContext.Exception.InnerException == null)
//            {
//                actionExecutedContext.Response = actionExecutedContext.Request.
//                    CreateResponse(HttpStatusCode.InternalServerError, responseMessage + actionExecutedContext.Exception.Message);
//            }
//            else
//            {
//                actionExecutedContext.Response = actionExecutedContext.Request
//                    .CreateResponse(HttpStatusCode.InternalServerError, responseMessage + actionExecutedContext.Exception.InnerException.Message);
//            }

//            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
//        }
//    }
//}