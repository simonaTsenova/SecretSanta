using System.Web.Http;
using System.Threading.Tasks;
using SecretSanta.Web.Models.Users;
using System.Net;
using SecretSanta.Web.Models;
using SecretSanta.Services.Contracts;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common.Exceptions;
using SecretSanta.Web.Mapper;
using Constants = SecretSanta.Common.Constants;

namespace SecretSanta.Web.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IAuthenticationProvider authenticationProvider,
            IUserService userService, IMapper mapper)
        {
            this.authenticationProvider = authenticationProvider;
            this.userService = userService;
            this.mapper = mapper;
        }

        // POST ~/users
        [HttpPost]
        [Route("api/users")]
        [AllowAnonymous]
        public IHttpActionResult Register(RegisterUserViewModel model)
        {
            if (model == null)
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var identityResult = this.userService.CreateUser(model.Email, model.UserName, model.DisplayName,
                model.FirstName, model.LastName, model.Password);

            if (!identityResult.Succeeded)
            {
                this.AddModelStateErrors(identityResult);

                return this.Content(HttpStatusCode.Conflict, ModelState);
            }

            var createdUser = this.userService.GetUserByUserName(model.UserName);
            var userModel = this.mapper.MapUser(createdUser);

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
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            try
            {
                var response = await this.authenticationProvider.GetAccessToken(model.UserName, model.Password);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var authToken = await this.ExtractAccessToken(response);

                    return this.Content(HttpStatusCode.Created, new { access_token = authToken });
                }

                return this.ResponseMessage(response);
            }
            catch (InvalidLoginException loginException)
            {
                return this.Content(HttpStatusCode.Unauthorized, loginException.Message);
            }
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

            var users = this.userService
                .GetAllUsers(formatModel.Skip, formatModel.Take, formatModel.Order, formatModel.Search);

            var usersModel = this.mapper.MapUsers(users);

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

                var userModel = this.mapper.MapUser(user);

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

        private void AddModelStateErrors(IdentityResult identityResult)
        {
            if (identityResult.Errors != null)
            {
                foreach (string error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
        }

        private async Task<string> ExtractAccessToken(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            var jsSerializer = new JavaScriptSerializer();
            var responseDeserialized =
                jsSerializer.Deserialize<Dictionary<string, string>>(responseString);
            var authToken = responseDeserialized["access_token"];

            return authToken;
        }
    }
}
