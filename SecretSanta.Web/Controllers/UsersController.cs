using System.Web.Http;
using System.Threading.Tasks;
using SecretSanta.Web.Models.Users;
using SecretSanta.Factories;
using System.Net;
using System.Web.Http.ModelBinding;
using SecretSanta.Web.Models;
using SecretSanta.Services.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;

namespace SecretSanta.Web.Controllers
{
    public class UsersController : ApiController
    {
        private IAuthenticationProvider authenticationProvider;
        private readonly IUserService userService;
        private readonly IUserFactory userFactory;

        public UsersController(IAuthenticationProvider authenticationProvider, IUserService userService, IUserFactory userFactory)
        {
            this.authenticationProvider = authenticationProvider;
            this.userService = userService;
            this.userFactory = userFactory;
        }

        // POST ~/users
        [HttpPost]
        [Route("api/users")]
        [AllowAnonymous]
        public IHttpActionResult Register(RegisterUserViewModel model)
        {
            if (model == null)
            {
                return this.BadRequest("User credentials must be provided");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = this.userFactory.Create(model.Email, model.UserName,
                model.DisplayName, model.FirstName, model.LastName);
            var identityResult = this.authenticationProvider.RegisterUser(user, model.Password);

            if (!identityResult.Succeeded)
            {
                if (identityResult.Errors != null)
                {
                    foreach (string error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (ModelState.IsValid)
                {
                    return this.BadRequest();
                }

                return this.Content<ModelStateDictionary>(HttpStatusCode.Conflict, ModelState);
            }

            var userModel = new DisplayUserViewModel(user.Email, user.FirstName,
                user.LastName, user.DisplayName, user.UserName);

            return this.Created("/api/users", userModel);
        }

        // POST ~/login
        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(LoginUserViewModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model not valid");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest("User data not valid");
            }

            var response = await this.authenticationProvider.GetAccessToken(model.UserName, model.Password);
            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Contains("invalid_grant"))
            {
                return this.Content(HttpStatusCode.Unauthorized, "The user name or password is incorrect.");
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsSerializer = new JavaScriptSerializer();
                var responseDeserialized =
                    jsSerializer.Deserialize<Dictionary<string, string>>(responseString);
                var authToken = responseDeserialized["access_token"];

                return this.Content(HttpStatusCode.Created, new { access_token = authToken });
            }

            return this.ResponseMessage(response);
        }

        // GET ~/users?skip={s}&take={t}&order={Asc|Desc}&search={phrase}
        [HttpGet]
        [Route("api/users")]
        public IHttpActionResult GetAllUsers([FromUri]ResultFormatViewModel formatModel)
        {
            if (formatModel == null)
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            var usersModel = this.userService
                .GetAllUsers(formatModel.Skip, formatModel.Take, formatModel.Order, formatModel.Search)
                .Select(user => new DisplayUserViewModel(user.Email, user.FirstName, user.LastName, user.DisplayName, user.UserName));

            return this.Ok(usersModel);
        }

        // GET ~/users/{username}
        [HttpGet]
        [Route("api/users/{username}")]
        public IHttpActionResult GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return this.BadRequest(Constants.REQUIRED_USERNAME);
            }

            try
            {
                var user = this.userService.GetUserByUserName(username);

                var userModel = new DisplayUserViewModel(user.Email, user.UserName, user.DisplayName, user.FirstName, user.LastName);

                return this.Ok(userModel);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
