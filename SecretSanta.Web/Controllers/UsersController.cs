﻿using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using SecretSanta.Web.Models.Users;
using SecretSanta.Factories;
using System.Net;
using System.Web.Http.ModelBinding;
using SecretSanta.Web.Models;
using SecretSanta.Services.Contracts;
using System.Linq;
using SecretSanta.Web.Infrastructure.Factories;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private ApplicationUserManager applicationUserManager;
        private readonly IUserService userService;
        private readonly IUserFactory userFactory;
        private readonly IDisplayUserViewModelFactory viewModelsFactory;

        public UsersController(IUserService userService, IUserFactory userFactory, IDisplayUserViewModelFactory viewModelsFactory)
        {
            this.userService = userService;
            this.userFactory = userFactory;
            this.viewModelsFactory = viewModelsFactory;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if(this.applicationUserManager == null)
                {
                    return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return this.applicationUserManager;
            }
            private set
            {
                this.applicationUserManager = value;
            }
        }

        // POST ~/users
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if(model == null)
            {
                return this.BadRequest("User credentials must be provided");
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = this.userFactory.Create(model.Email, model.UserName, 
                model.DisplayName, model.FirstName, model.LastName);
            var identityResult = await this.UserManager.CreateAsync(user, model.Password);

            if(!identityResult.Succeeded)
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
                    return BadRequest();
                }

                return this.Content<ModelStateDictionary>(HttpStatusCode.Conflict, ModelState);
            }

            var userModel = new DisplayUserViewModel(user.Email, user.FirstName, 
                user.LastName, user.DisplayName, user.UserName);

            return this.Created("/api/users", userModel);
        }

        // GET ~/users?skip={s}&take={t}&order={Asc|Desc}&search={phrase}
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllUsers([FromUri]ResultFormatViewModel formatModel)
        {
            if (formatModel == null)
            {
                return this.BadRequest();
            }

            var usersModel = this.userService
                .GetAllUsers(formatModel.Skip, formatModel.Take, formatModel.Order, formatModel.Search)
                .Select(user => this.viewModelsFactory
                    .CreateDisplayUserViewModel(user.Email, user.FirstName, user.LastName, user.DisplayName, user.UserName));

            return this.Ok(usersModel);
        }

        // GET ~/users/{username}
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult GetUser(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                return this.BadRequest("Username must be provided");
            }

            var user = this.userService.GetUserByUserName(username);
            if(user == null)
            {
                return this.NotFound();
            }

            var userModel = this.viewModelsFactory
                .CreateDisplayUserViewModel(user.Email, user.UserName, user.DisplayName, user.FirstName, user.LastName);

            return this.Ok(userModel);
        }
    }
}
