using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SecretSanta.Authentication.Managers;
using SecretSanta.Data;
using SecretSanta.Web.Providers;
using System;

namespace SecretSanta.Web
{
    public partial class Startup
    {
        public const string TOKEN_ENDPOINT_PATH = "/token";
        public static string PUBLIC_CLIENT_ID = "self";
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString(TOKEN_ENDPOINT_PATH),
                Provider = new ApplicationOAuthProvider(PUBLIC_CLIENT_ID),
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