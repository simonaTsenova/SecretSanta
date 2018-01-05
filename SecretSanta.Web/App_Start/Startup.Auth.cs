using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SecretSanta.Data;
using SecretSanta.Web.Providers;
using System;

namespace SecretSanta.Web
{
    public partial class Startup
    {
        public const string TokenEndpointPath = "/token";
        public static string PublicClientId { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            PublicClientId = "self";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString(TokenEndpointPath),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                AllowInsecureHttp = true
            };
        }

        public void ConfigureAuth(IAppBuilder app)
        { 
            // Configure the db context, user manager to use a single instance per request
            app.CreatePerOwinContext(SecretSantaDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}