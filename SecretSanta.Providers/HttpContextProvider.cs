using System.Web;
using SecretSanta.Providers.Contracts;

namespace SecretSanta.Providers
{
    public class HttpContextProvider : IHttpContextProvider
    {
        public HttpContext CurrentHttpContext
        {
            get
            {
                return HttpContext.Current;
            }
        }
    }
}
