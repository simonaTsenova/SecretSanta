﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/logins")]
    public class AccountController : ApiController
    {
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public AccountController(ISessionService sessionService, IUserService userService)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException("Service cannot be null");
            this.userService = userService ?? throw new ArgumentNullException("Service cannot be null");
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        // POST ~/logins
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> LoginUser(LoginUserViewModel model)
        {
            if(model == null)
            {
                return this.BadRequest("Model not valid");
            }

            if(!ModelState.IsValid)
            {
                return this.BadRequest("User data not valid");
            }

            var requestParams = new Dictionary<string, string>()
            {
                {"grant_type", "password"},
                {"username", model.UserName},
                {"password", model.Password},
            };
            var requestParamsEncoded = new FormUrlEncodedContent(requestParams);
            var baseAddress = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(baseAddress + Startup.TokenEndpointPath, requestParamsEncoded);
            var responseString = await response.Content.ReadAsStringAsync();

            if(responseString.Contains("invalid_grant"))
            {
                return this.Content(HttpStatusCode.Unauthorized, "The user name or password is incorrect.");
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsSerializer = new JavaScriptSerializer();
                var responseDeserialized =
                    jsSerializer.Deserialize<Dictionary<string, string>>(responseString);
                var authToken = responseDeserialized["access_token"];
                var userName = responseDeserialized["userName"];
                var expirationDate = responseDeserialized[".expires"];

                var user = this.userService.GetUserByUserName(userName);
                if (user == null)
                {
                    return this.NotFound();
                }

                var userSession = this.sessionService.GetSessionByUserId(user.Id);
                if (userSession != null && userSession.ExpiresOn > DateTime.Now)
                {
                    // Session already exists and has not expired
                    return this.Content(HttpStatusCode.Conflict, "User is already logged in.");
                }

                this.sessionService.CreateUserSession(user, authToken);
                // Cleanup: delete expired sessions from the database
                this.sessionService.DeleteExpiredSessions();

                return this.Content(HttpStatusCode.Created, new { access_token = authToken });
            }

            return this.ResponseMessage(response);
        }

        // DELETE ~/logins/{username}
        [HttpDelete]
        [Route("")]
        public IHttpActionResult LogoutUser()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);

            try
            {
                this.sessionService.InvalidateUserSession();
            }
            catch(ArgumentNullException)
            {
                return this.BadRequest();
            }
            catch(ObjectNotFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, "User has already logged out or their session has already expired");
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}
