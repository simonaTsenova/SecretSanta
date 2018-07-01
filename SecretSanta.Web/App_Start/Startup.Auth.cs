using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SecretSanta.Authentication.Managers;
using SecretSanta.Data;
using System;
using SecretSanta.Authentication;
using SecretSanta.Common;

namespace SecretSanta.Web
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString(Constants.TOKEN_ENDPOINT_PATH),
                Provider = new ApplicationOAuthProvider(Constants.PUBLIC_CLIENT_ID),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                AllowInsecureHttp = true
            };
        }

        public void ConfigureAuth(IAppBuilder app)
        { 
            app.CreatePerOwinContext(SecretSantaDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}