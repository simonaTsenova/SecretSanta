using System.Net.Http;
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
using SecretSanta.Web.Models.Invitations;
using System.Collections.Generic;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private ApplicationUserManager applicationUserManager;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IInvitationService invitationService;
        private readonly IUserFactory userFactory;
        private readonly IDisplayUserViewModelFactory viewModelsFactory;
        private readonly IInvitationViewModelFactory invitationViewModelFactory;

        public UsersController(IUserService userService, IGroupService groupService, 
            ISessionService sessionService, IInvitationService invitationService,
            IUserFactory userFactory, IDisplayUserViewModelFactory viewModelsFactory,
            IInvitationViewModelFactory invitationViewModelFactory)
        {
            this.userService = userService;
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.invitationService = invitationService;
            this.userFactory = userFactory;
            this.viewModelsFactory = viewModelsFactory;
            this.invitationViewModelFactory = invitationViewModelFactory;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (this.applicationUserManager == null)
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
            var identityResult = await this.UserManager.CreateAsync(user, model.Password);

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
            if (string.IsNullOrEmpty(username))
            {
                return this.BadRequest("Username must be provided");
            }

            var user = this.userService.GetUserByUserName(username);
            if (user == null)
            {
                return this.NotFound();
            }

            var userModel = this.viewModelsFactory
                .CreateDisplayUserViewModel(user.Email, user.UserName, user.DisplayName, user.FirstName, user.LastName);

            return this.Ok(userModel);
        }

        // POST ~/usrs/{username}/invitations
        [HttpPost]
        [Route("{username}/invitations")]
        public IHttpActionResult SendInvitation([FromUri] string username, InvitationViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null || !ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var group = this.groupService.GetGroupByName(model.GroupName);
            var receiver = this.userService.GetUserByUserName(username);
            if (group == null || receiver == null)
            {
                return this.NotFound();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if(currentUser.UserName != group.Admin.UserName)
            {
                return this.Content(HttpStatusCode.Forbidden, "Only admins can send invitations for groups.");
            }

            var existingInvitation = this.invitationService.GetByGroupAndUser(model.GroupName, username);
            if(existingInvitation != null)
            {
                return this.Content(HttpStatusCode.Conflict, "This user already has an invitation for this group.");
            }

            this.invitationService.CreateInvitation(group.Id, model.SentDate, receiver.Id);
            var invitationId = this.invitationService.GetByGroupAndUser(group.Name, receiver.UserName).Id;
            var invitationModel = this.invitationViewModelFactory.Create(invitationId, model.SentDate, group.Name, receiver.UserName);

            return this.Content(HttpStatusCode.Created, invitationModel);
        }

        // GET ~/users/{username}/invitations?skip={s}&take={t}&order={A|D}
        [HttpGet]
        [Route("{username}/invitations")]
        public IHttpActionResult GetUserInvitations(string username, [FromUri]ResultFormatViewModel model)
        {
            if(string.IsNullOrEmpty(username) || model == null)
            {
                return this.BadRequest();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if(currentUser.UserName != username)
            {
                return this.Content(HttpStatusCode.Forbidden, "Users are only allowed to access their invitations.");
            }

            var invitations = this.userService.GetUserInvitations(currentUser, model.Skip, model.Take, model.Order);

            var invitationsModel = invitations
                .Select(i => this.invitationViewModelFactory.Create(i.Id, i.SentDate, i.Group.Name, i.Receiver.UserName));

            return this.Ok(invitationsModel);
        }
    }
}
