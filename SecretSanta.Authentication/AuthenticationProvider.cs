using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Authentication.Managers;
using SecretSanta.Models;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SecretSanta.Authentication
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private const string TOKEN_ENDPOINT_PATH = "/token";
        private ApplicationUserManager applicationUserManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                if (this.applicationUserManager == null)
                {
                    //return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return this.applicationUserManager;
            }
            private set
            {
                this.applicationUserManager = value;
            }
        }

        public IdentityResult RegisterUser(User user, string password)
        {
            var result = this.UserManager.Create(user, password);

            return result;
        }

        public async Task<HttpResponseMessage> GetAccessToken(string username, string password)
        {
            var requestParams = new Dictionary<string, string>()
            {
                {"grant_type", "password"},
                {"username", username},
                {"password", password},
            };
            var requestParamsEncoded = new FormUrlEncodedContent(requestParams);
            var baseAddress = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var httpClient = new HttpClient();
            var response =  await httpClient.PostAsync(baseAddress + TOKEN_ENDPOINT_PATH, requestParamsEncoded);
            return response;
        }
    }
}
